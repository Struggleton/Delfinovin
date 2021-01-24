using BitStreams;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using System;
using System.Threading.Tasks;

namespace Delfinovin
{
    // Datasheet here: https://github.com/gemarcano/GCN_Adapter-Driver/blob/58b09b851d7211204dd0153c10d702d7a8aab669/docs/Gamecube%20controller%20data%20layout.xlsx
    public class Controller
    {
        // Byte 1
        public bool IsPowered; // bit 2, If the adapter is plugged in
        public bool NormalType; // bit 4, whether or not the controller is wireless or not.
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

        // Bytes 4 through 9
        public byte LEFT_STICK_X; //  4
        public byte LEFT_STICK_Y; // 5
        public byte C_STICK_X; // 6
        public byte C_STICK_Y; // 7

        public byte ANALOG_LEFT; // 8
        public byte ANALOG_RIGHT; // 9

        
        private ControllerCalibration _Calibration;
        public bool IsPlugged;
        private int _VibrationMotor;

        private bool _RumbleEnabled;
        public bool RumbleChanged
        {
            get { return _RumbleEnabled; }
            set
            {
                _RumbleEnabled = value;
                if (!_RumbleEnabled)
                {
                    // If the rumble goes on for more than a second without any changes, end it
                    Task.Delay(1000).ContinueWith(x =>
                    {
                        _VibrationMotor = 0;
                    });
                }
            }
        }

        public Controller()
        {
            _Calibration = new ControllerCalibration();
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

            // If neither are set, there is no controller plugged in.
            IsPlugged = NormalType || WavebirdType; 
        }

        public void UpdateCenter()
        {
            if (ApplicationSettings.CalibrateCenter && _Calibration.CurrentState == CalibrationState.Uncalibrated)
            {
                // Current instance of Controller
                _Calibration.GetCenterScales(this);
            }
        }

        public void UpdateMinMax()
        {
            // prompt min-max update using current values 
            _Calibration.UpdateMinMax(this);
        }

        public void UpdateController(IXbox360Controller controller)
        {
            // Only update controller if it is plugged in.
            if (IsPlugged)
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
                controller.SetButtonState(Xbox360Button.RightShoulder, BUTTON_Z);

                // If enabled, any press becomes a full press.
                if (ApplicationSettings.EnableDigitalPress)
                {
                    // Check if analog trigger values are greater than deadzone. If so, set greatest possible value.
                    // If not, set the value to 0 - no press.
                    if ((ANALOG_LEFT / 255f) > ApplicationSettings.TriggerDeadzone)
                        controller.SetSliderValue(Xbox360Slider.LeftTrigger, 255);
                    else
                        controller.SetSliderValue(Xbox360Slider.LeftTrigger, 0);
                    if ((ANALOG_RIGHT / 255f) > ApplicationSettings.TriggerDeadzone)
                        controller.SetSliderValue(Xbox360Slider.RightTrigger, 255);
                    else
                        controller.SetSliderValue(Xbox360Slider.RightTrigger, 0);
                }

                // Use normal triggerThreshold/deadzone values
                else
                {
                    controller.SetSliderValue(Xbox360Slider.LeftTrigger, ANALOG_LEFT);
                    controller.SetSliderValue(Xbox360Slider.RightTrigger, ANALOG_RIGHT);
                }

                // Check if calibration has been done. If not generate it
                if (_Calibration.CurrentState == CalibrationState.Calibrating)
                {
                    _Calibration.GenerateCalibrations();
                }

                // Full calibration scales generated
                if (_Calibration.CurrentState == CalibrationState.Calibrated)
                {
                    double newX = Math.Round(LEFT_STICK_X * _Calibration.LeftStickCalibration[0] - _Calibration.LeftStickCalibration[2]);
                    double newY = Math.Round(LEFT_STICK_Y * _Calibration.LeftStickCalibration[1] - _Calibration.LeftStickCalibration[3]);

                    controller.SetAxisValue(Xbox360Axis.LeftThumbX, Extensions.ByteToShort((byte)Extensions.Clamp(newX, 0, 255)));
                    controller.SetAxisValue(Xbox360Axis.LeftThumbY, Extensions.ByteToShort((byte)Extensions.Clamp(newY, 0, 255)));


                    newX = Math.Round(C_STICK_X * _Calibration.CStickCalibration[0] - _Calibration.CStickCalibration[2]);
                    newY = Math.Round(C_STICK_Y * _Calibration.CStickCalibration[1] - _Calibration.CStickCalibration[3]);

                    controller.SetAxisValue(Xbox360Axis.RightThumbX, Extensions.ByteToShort((byte)Extensions.Clamp(newX, 0, 255)));
                    controller.SetAxisValue(Xbox360Axis.RightThumbY, Extensions.ByteToShort((byte)Extensions.Clamp(newY, 0, 255)));
                }

                // Stick centers generated
                else if (_Calibration.CurrentState == CalibrationState.SticksCentered)
                {
                    controller.SetAxisValue(Xbox360Axis.LeftThumbX, Extensions.ByteToShort((byte)(LEFT_STICK_X + _Calibration.StickCenters[0])));
                    controller.SetAxisValue(Xbox360Axis.LeftThumbY, Extensions.ByteToShort((byte)(LEFT_STICK_Y + _Calibration.StickCenters[1])));

                    controller.SetAxisValue(Xbox360Axis.RightThumbX, Extensions.ByteToShort((byte)(C_STICK_X + _Calibration.StickCenters[2])));
                    controller.SetAxisValue(Xbox360Axis.RightThumbY, Extensions.ByteToShort((byte)(C_STICK_Y + _Calibration.StickCenters[3])));

                }

                // No calibration done
                else
                {
                    controller.SetAxisValue(Xbox360Axis.LeftThumbX, Extensions.ByteToShort(LEFT_STICK_X));
                    controller.SetAxisValue(Xbox360Axis.LeftThumbY, Extensions.ByteToShort(LEFT_STICK_Y));

                    controller.SetAxisValue(Xbox360Axis.RightThumbX, Extensions.ByteToShort(C_STICK_X));
                    controller.SetAxisValue(Xbox360Axis.RightThumbY, Extensions.ByteToShort(C_STICK_Y));
                }

                controller.FeedbackReceived += Controller_FeedbackReceived;
                controller.SubmitReport();
            }
        }

        private void Controller_FeedbackReceived(object sender, Xbox360FeedbackReceivedEventArgs e)
        {
            // Set whether or not the value increased
            RumbleChanged = _VibrationMotor > 0;
            _VibrationMotor = e.LargeMotor;
        }

        private byte CompareDeadzones(byte input)
        {
            // convert trigger byte to float
            float compare = input / 255f;

            // If the trigger deadzone is greater, return no input.
            if (ApplicationSettings.TriggerDeadzone > compare)
                return 0;
            // If the trigger threshold is met, send a full input.
            else if (ApplicationSettings.TriggerThreshold < compare)
                return 255;

            // return normal value if between TriggerDeadzone/Threshold bounds
            return input;
        }
    }
}
