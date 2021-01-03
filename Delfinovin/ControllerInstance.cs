using BitStreams;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

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

        private bool IsPlugged;
        private bool IsCalibrated = false;

        private int LeftScaleFactorX = 0;
        private int LeftScaleFactorY = 0;
        private int CScaleFactorX = 0;
        private int CScaleFactorY = 0;

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

            ANALOG_LEFT = AnalogDeadzone(bStream.ReadByte());
            ANALOG_RIGHT = AnalogDeadzone(bStream.ReadByte());

            IsPlugged = NormalType || WavebirdType;
        }

        public void CalibrateController()
        {
            if (IsPlugged && ApplicationSettings.CalibrateCenter && !IsCalibrated)
            {
                LeftScaleFactorX = 127 - LEFT_STICK_X;
                LeftScaleFactorY = 127 - LEFT_STICK_Y;
                CScaleFactorX = 127 - C_STICK_X;
                CScaleFactorY = 127 - C_STICK_Y;

                IsCalibrated = true;
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

            controller.SetSliderValue(Xbox360Slider.LeftTrigger, ANALOG_LEFT);
            controller.SetSliderValue(Xbox360Slider.RightTrigger, ANALOG_RIGHT);

            controller.SetAxisValue(Xbox360Axis.LeftThumbX, ByteToShort((byte)(LEFT_STICK_X + LeftScaleFactorX)));
            controller.SetAxisValue(Xbox360Axis.LeftThumbY, ByteToShort((byte)(LEFT_STICK_Y + LeftScaleFactorY)));

            controller.SetAxisValue(Xbox360Axis.RightThumbX, ByteToShort((byte)(C_STICK_X + CScaleFactorX)));
            controller.SetAxisValue(Xbox360Axis.RightThumbY, ByteToShort((byte)(C_STICK_Y + CScaleFactorY)));

            controller.SubmitReport();
        }

        private static short ByteToShort(byte b)
        {
            return (short)(((b << 8) | b) ^ 0x8000);
        }

        private byte AnalogDeadzone(byte input)
        {
            double compare = input / 255f;

            if (ApplicationSettings.TriggerDeadzone > compare)
                return 0;
            else if (ApplicationSettings.TriggerThreshold < compare)
                return 255;
            return input;
        }
    }
}
