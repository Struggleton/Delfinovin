using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using System.Threading.Tasks;

namespace DelfinovinActX2
{
    class GamecubeController
    {
        private IXbox360Controller _controller;
        private GamecubeInputState _currentState;

        private int _vibrationMotor;
        private bool _rumbleEnabled;

        private int[] _StickCenters;

        public bool RumbleChanged
        {
            get { return _rumbleEnabled; }
            set
            {
                _rumbleEnabled = value;
                if (!_rumbleEnabled)
                {
                    // If the rumble goes on for more than a second without any changes in strength, end it
                    Task.Delay(1000).ContinueWith(x =>
                    {
                        _vibrationMotor = 0;
                    });
                }
            }
        }

        public bool IsConnected;

        public GamecubeController(ViGEmClient client)
        {
            _controller = client.CreateXbox360Controller();
            _StickCenters = new int[4];
            Init();
        }

        public void SetStickCenters(GamecubeInputState inputState)
        {
            _StickCenters[0] = inputState.LEFT_STICK_X;
            _StickCenters[1] = inputState.LEFT_STICK_Y;
            _StickCenters[2] = inputState.C_STICK_X;
            _StickCenters[3] = inputState.C_STICK_Y;
        }

        public void UpdateInput(GamecubeInputState inputState)
        {
            if (_currentState.IsEqual(inputState) || !IsConnected)
                return;

            _controller.SetButtonState(Xbox360Button.A, inputState.BUTTON_B);
            _controller.SetButtonState(Xbox360Button.B, inputState.BUTTON_A);
            _controller.SetButtonState(Xbox360Button.X, inputState.BUTTON_Y);
            _controller.SetButtonState(Xbox360Button.Y, inputState.BUTTON_X);
            _controller.SetButtonState(Xbox360Button.Left, inputState.DPAD_LEFT);
            _controller.SetButtonState(Xbox360Button.Right, inputState.DPAD_RIGHT);
            _controller.SetButtonState(Xbox360Button.Up, inputState.DPAD_UP);
            _controller.SetButtonState(Xbox360Button.Down, inputState.DPAD_DOWN);
            _controller.SetButtonState(Xbox360Button.Start, inputState.BUTTON_START);

            if (ApplicationSettings.EnableDigitalPress)
            {
                if ((inputState.ANALOG_LEFT / 255f) > ApplicationSettings.TriggerDeadzone)
                    _controller.SetButtonState(Xbox360Button.LeftShoulder, true);
                else
                    _controller.SetButtonState(Xbox360Button.LeftShoulder, false);

                if ((inputState.ANALOG_RIGHT / 255f) > ApplicationSettings.TriggerDeadzone)
                    _controller.SetButtonState(Xbox360Button.RightShoulder, true);
                else
                    _controller.SetButtonState(Xbox360Button.RightShoulder, false);

                if (inputState.BUTTON_Z)
                    _controller.SetSliderValue(Xbox360Slider.RightTrigger, 255);
                else
                    _controller.SetSliderValue(Xbox360Slider.RightTrigger, 0);
            }

            // Use normal triggerThreshold/deadzone values
            else
            {
                _controller.SetSliderValue(Xbox360Slider.LeftTrigger, GamecubeDeadzones.ClampTriggers(inputState.ANALOG_LEFT));
                _controller.SetSliderValue(Xbox360Slider.RightTrigger, GamecubeDeadzones.ClampTriggers(inputState.ANALOG_RIGHT));
                _controller.SetButtonState(Xbox360Button.RightShoulder, inputState.BUTTON_Z);
            }

            byte LeftStickX = inputState.LEFT_STICK_X;
            byte LeftStickY = inputState.LEFT_STICK_Y;
            byte CStickX = inputState.C_STICK_X;
            byte CStickY = inputState.C_STICK_Y;

            if (ApplicationSettings.CalibrateCenter)
            {
                LeftStickX += (byte)(127 - _StickCenters[0]);
                LeftStickY += (byte)(127 - _StickCenters[1]);
                CStickX += (byte)(127 - _StickCenters[2]);
                CStickY += (byte)(127 - _StickCenters[3]);
            }

            _controller.SetAxisValue(Xbox360Axis.LeftThumbX, Extensions.ByteToShort(LeftStickX, false));
            _controller.SetAxisValue(Xbox360Axis.LeftThumbY, Extensions.ByteToShort(LeftStickY, false));
            
            _controller.SetAxisValue(Xbox360Axis.RightThumbX, Extensions.ByteToShort(CStickX, false));
            _controller.SetAxisValue(Xbox360Axis.RightThumbY, Extensions.ByteToShort(CStickY, false));
           
            _controller.SubmitReport();
        }

        private void Init()
        {
            if (ApplicationSettings.EnableRumble)
                _controller.FeedbackReceived += FeedbackReceived;
            _currentState = new GamecubeInputState();
        }

        public void Connect()
        {
            _controller.Connect();
            UpdateInput(new GamecubeInputState());
            IsConnected = true;
        }

        public void Disconnect()
        {
            if (ApplicationSettings.EnableRumble)
                _controller.FeedbackReceived -= FeedbackReceived;

            _controller.Disconnect();
            IsConnected = false;
        }

        private void FeedbackReceived(object sender, Xbox360FeedbackReceivedEventArgs e)
        {
            _vibrationMotor = e.LargeMotor;
            RumbleChanged = _vibrationMotor > 0;
        }
    }
}
