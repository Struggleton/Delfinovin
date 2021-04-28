using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using System;
using System.Threading.Tasks;

namespace DelfinovinActX2
{
    class GamecubeController
    {
        private IXbox360Controller _controller;
        private GamecubeInputState _currentState;

        private int _vibrationMotor;
        private bool _rumbleEnabled;

        public GamecubeCalibration Calibration;

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
            Calibration = new GamecubeCalibration();
            Init();
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

            double LeftStickX = inputState.LEFT_STICK_X;
            double LeftStickY = inputState.LEFT_STICK_Y;
            double CStickX = inputState.C_STICK_X;
            double CStickY = inputState.C_STICK_Y;

            if (Calibration.CurrentStatus == CalibrationStatus.Calibrated)
            {
                LeftStickX = Math.Round(LeftStickX * Calibration.LeftStickCalibration[0] - Calibration.LeftStickCalibration[2]);
                LeftStickY = Math.Round(LeftStickY * Calibration.LeftStickCalibration[1] - Calibration.LeftStickCalibration[3]);
                CStickX = Math.Round(CStickX * Calibration.CStickCalibration[0] - Calibration.CStickCalibration[2]);
                CStickY = Math.Round(CStickY * Calibration.CStickCalibration[1] - Calibration.CStickCalibration[3]);
            }

            else if (Calibration.CurrentStatus == CalibrationStatus.Centered)
            {
                LeftStickX += Calibration.StickCenters[0];
                LeftStickY += Calibration.StickCenters[1];
                CStickX += Calibration.StickCenters[2];
                CStickY += Calibration.StickCenters[3];
            }

            LeftStickX = Extensions.ByteToShort((byte)Extensions.Clamp(LeftStickX, 0, 255), false); // ensure stick values are within byte range
            LeftStickY = Extensions.ByteToShort((byte)Extensions.Clamp(LeftStickY, 0, 255), false);

            CStickX = Extensions.ByteToShort((byte)Extensions.Clamp(CStickX, 0, 255), false);
            CStickY = Extensions.ByteToShort((byte)Extensions.Clamp(CStickY, 0, 255), false);

            _controller.SetAxisValue(Xbox360Axis.LeftThumbX, (short)LeftStickX);
            _controller.SetAxisValue(Xbox360Axis.LeftThumbY, (short)LeftStickY);
                                                              
            _controller.SetAxisValue(Xbox360Axis.RightThumbX, (short)CStickX);
            _controller.SetAxisValue(Xbox360Axis.RightThumbY, (short)CStickY);

            _controller.SubmitReport();
        }

        public void SetStickCenters(GamecubeInputState controllerInput)
        {
            Calibration.SetStickCenters(controllerInput);
        }

        public void SetMinMax(GamecubeInputState controllerInput)
        {
            Calibration.GetMinMax(controllerInput);
        }

        public void GenerateCalibrations()
        {
            Calibration.GenerateCalibrations();
        }

        public void ResetCalibration()
        {
            Calibration = new GamecubeCalibration();
        }

        private void FeedbackReceived(object sender, Xbox360FeedbackReceivedEventArgs e)
        {
            _vibrationMotor = e.LargeMotor;
            RumbleChanged = _vibrationMotor > 0;
        }
    }
}
