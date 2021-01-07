using BitStreams;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using System;
using System.Numerics;

namespace Delfinovin
{
    // Datasheet here: https://github.com/gemarcano/GCN_Adapter-Driver/blob/58b09b851d7211204dd0153c10d702d7a8aab669/docs/Gamecube%20controller%20data%20layout.xlsx
    public class ControllerInstance
    {
        // Byte 1
        public bool IsPowered; // bit 2
        public bool NormalType; // bit 4
        public bool WavebirdType; // bit 5

        // Byte 2
        public bool BUTTON_A; // bit 0
        public bool BUTTON_B;
        public bool BUTTON_X;
        public bool BUTTON_Y;
        public bool DPAD_LEFT;
        public bool DPAD_RIGHT;
        public bool DPAD_DOWN;
        public bool DPAD_UP; // bit 7

        // Byte 3
        public bool BUTTON_START; // bit 0
        public bool BUTTON_Z;
        public bool BUTTON_R;
        public bool BUTTON_L; // bit 3

        public byte LEFT_STICK_X; // 4
        public byte LEFT_STICK_Y; // 5
        public byte C_STICK_X; // 6
        public byte C_STICK_Y; // 7

        public byte ANALOG_LEFT; // 8
        public byte ANALOG_RIGHT; // 9

        private bool _IsPlugged;
        private CalibrationState _Calibration; // -1 - uncalibrated, 1 - stick centered only, 2 - full calibration, 3 - full calibration, values generated

        private byte[] LeftStickMinMax = new byte[4] { 127, 127, 127, 127 }; // x_min, x_max, y_min, y_max
        private byte[] CStickMinMax = new byte[4] { 127, 127, 127, 127 };

        private float[] LeftStickCalibration = new float[4]; // x-mult, y-mult, x-off, y-off
        private float[] CStickCalibration = new float[4];

        private int[] StickCenters = new int[4];
        enum CalibrationState
        {
            Uncalibrated,
            SticksCentered,
            Calibrating,
            Calibrated
        }

        public ControllerInstance()
        {

        }

        public void ReadControllerData(BitStream bStream)
        {
            bStream.ReadBits(2);
            IsPowered = bStream.ReadBit();
            bStream.ReadBit();
            NormalType = bStream.ReadBit();
            WavebirdType = bStream.ReadBit();
            bStream.ReadBits(2);

            BUTTON_A = bStream.ReadBit();
            BUTTON_B = bStream.ReadBit();
            BUTTON_X = bStream.ReadBit();
            BUTTON_Y = bStream.ReadBit();
            DPAD_LEFT = bStream.ReadBit();
            DPAD_RIGHT = bStream.ReadBit();
            DPAD_DOWN = bStream.ReadBit();
            DPAD_UP = bStream.ReadBit();

            BUTTON_START = bStream.ReadBit();
            BUTTON_Z = bStream.ReadBit();
            BUTTON_R = bStream.ReadBit();
            BUTTON_L = bStream.ReadBit();
            bStream.ReadBits(4);

            LEFT_STICK_X = bStream.ReadByte();
            LEFT_STICK_Y = bStream.ReadByte();
            C_STICK_X = bStream.ReadByte();
            C_STICK_Y = bStream.ReadByte();

            ANALOG_LEFT = CompareDeadzones(bStream.ReadByte());
            ANALOG_RIGHT = CompareDeadzones(bStream.ReadByte());

            _IsPlugged = NormalType || WavebirdType;
        }

        public void UpdateCenter()
        {
            if (ApplicationSettings.CalibrateCenter && _Calibration == CalibrationState.Uncalibrated)
            {
                StickCenters[0] = 127 - LEFT_STICK_X;
                StickCenters[1] = 127 - LEFT_STICK_Y;
                StickCenters[2] = 127 - C_STICK_X;
                StickCenters[3] = 127 - C_STICK_Y;

                _Calibration = CalibrationState.SticksCentered;
            }
        }

        
        public void UpdateController(IXbox360Controller controller)
        {
            controller.SetButtonState(Xbox360Button.A, BUTTON_A);
            controller.SetButtonState(Xbox360Button.B, BUTTON_B);
            controller.SetButtonState(Xbox360Button.X, BUTTON_X);
            controller.SetButtonState(Xbox360Button.Y, BUTTON_Y);
            controller.SetButtonState(Xbox360Button.Left, DPAD_LEFT);
            controller.SetButtonState(Xbox360Button.Right, DPAD_RIGHT);
            controller.SetButtonState(Xbox360Button.Up, DPAD_UP);
            controller.SetButtonState(Xbox360Button.Down, DPAD_DOWN);
            controller.SetButtonState(Xbox360Button.Start, BUTTON_START);
            controller.SetButtonState(Xbox360Button.Back, BUTTON_Z); // Set Z as a select button - where are my buttons

            controller.SetSliderValue(Xbox360Slider.LeftTrigger, ANALOG_LEFT);
            controller.SetSliderValue(Xbox360Slider.RightTrigger, ANALOG_RIGHT);

            if (ApplicationSettings.EnableAnalogPress)
            {
                // figure out how to implement this into AnalogDeadzone()
                if ((ANALOG_LEFT / 255f) > ApplicationSettings.TriggerDeadzone)
                {
                    controller.SetButtonState(Xbox360Button.LeftShoulder, true);
                }

                if ((ANALOG_RIGHT / 255f) > ApplicationSettings.TriggerDeadzone)
                {
                    controller.SetButtonState(Xbox360Button.RightShoulder, true);
                }
            }

            else
            {
                controller.SetButtonState(Xbox360Button.LeftShoulder, BUTTON_L);
                controller.SetButtonState(Xbox360Button.RightShoulder, BUTTON_R);
            }

            // Check if the calibration settings have been set and if calibration has been done in the menu
            if (_Calibration == CalibrationState.Calibrating && LeftStickCalibration[0] == 0)
            {
                LeftStickCalibration = GenerateCalibration(LeftStickMinMax);
                CStickCalibration = GenerateCalibration(CStickMinMax);

                _Calibration = CalibrationState.Calibrated;
            }

            // this is so ugly, please find a way to condense this
            if (_Calibration == CalibrationState.Calibrated)
            {
                controller.SetAxisValue(Xbox360Axis.LeftThumbX, Extensions.ByteToShort((byte)Extensions.Clamp(Math.Round(LEFT_STICK_X * LeftStickCalibration[0] - LeftStickCalibration[2]), 0, 255)));
                controller.SetAxisValue(Xbox360Axis.LeftThumbY, Extensions.ByteToShort((byte)Extensions.Clamp(Math.Round(LEFT_STICK_Y * LeftStickCalibration[1] - LeftStickCalibration[3]), 0, 255)));

                controller.SetAxisValue(Xbox360Axis.RightThumbX, Extensions.ByteToShort((byte)Extensions.Clamp(Math.Round((C_STICK_X * CStickCalibration[0] - CStickCalibration[2])), 0, 255)));
                controller.SetAxisValue(Xbox360Axis.RightThumbY, Extensions.ByteToShort((byte)Extensions.Clamp(Math.Round((C_STICK_Y * CStickCalibration[1] - CStickCalibration[3])), 0, 255)));
            }

            else if (_Calibration == CalibrationState.SticksCentered)
            {
                

                controller.SetAxisValue(Xbox360Axis.LeftThumbX, Extensions.ByteToShort((byte)(LEFT_STICK_X + StickCenters[0])));
                controller.SetAxisValue(Xbox360Axis.LeftThumbY, Extensions.ByteToShort((byte)(LEFT_STICK_Y + StickCenters[1])));

                controller.SetAxisValue(Xbox360Axis.RightThumbX, Extensions.ByteToShort((byte)(C_STICK_X + StickCenters[2])));
                controller.SetAxisValue(Xbox360Axis.RightThumbY, Extensions.ByteToShort((byte)(C_STICK_Y + StickCenters[3])));

            }

            else
            {
                controller.SetAxisValue(Xbox360Axis.LeftThumbX, Extensions.ByteToShort(LEFT_STICK_X));
                controller.SetAxisValue(Xbox360Axis.LeftThumbY, Extensions.ByteToShort(LEFT_STICK_Y));

                controller.SetAxisValue(Xbox360Axis.RightThumbX, Extensions.ByteToShort(C_STICK_X));
                controller.SetAxisValue(Xbox360Axis.RightThumbY, Extensions.ByteToShort(C_STICK_Y));
            }

            controller.SubmitReport();
        }

        private byte CompareDeadzones(byte input)
        {
            double compare = input / 255f;

            if (ApplicationSettings.TriggerDeadzone > compare)
                return 0;
            else if (ApplicationSettings.TriggerThreshold < compare)
                return 255;
            return input;
        }

        private static float[] GenerateCalibration(byte[] minMax)
        {
            float[] stickCalibration = new float[4];

            stickCalibration[0] = 256f / (minMax[1] - minMax[0]);
            stickCalibration[1] = 256f / (minMax[3] - minMax[2]);
            stickCalibration[2] = 127f * stickCalibration[0] - 127f;
            stickCalibration[3] = 127f * stickCalibration[1] - 127f;

            return stickCalibration;
        }

        public void UpdateMinMax()
        {
            if (LeftStickMinMax[0] > LEFT_STICK_X)
                LeftStickMinMax[0] = LEFT_STICK_X;

            if (LeftStickMinMax[1] < LEFT_STICK_X)
                LeftStickMinMax[1] = LEFT_STICK_X;

            if (LeftStickMinMax[2] > LEFT_STICK_Y)
                LeftStickMinMax[2] = LEFT_STICK_Y;

            if (LeftStickMinMax[3] < LEFT_STICK_X)
                LeftStickMinMax[3] = LEFT_STICK_Y;


            if (CStickMinMax[0] > C_STICK_X)
                CStickMinMax[0] = C_STICK_X;

            if (CStickMinMax[1] < C_STICK_X)
                CStickMinMax[1] = C_STICK_X;

            if (CStickMinMax[2] > C_STICK_Y)
                CStickMinMax[2] = C_STICK_Y;

            if (CStickMinMax[3] < C_STICK_X)
                CStickMinMax[3] = C_STICK_Y;

            _Calibration = CalibrationState.Calibrating;
        }
    }
}
