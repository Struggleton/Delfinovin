using System;
using System.Threading.Tasks;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace DelfinovinnUI
{
	internal class GamecubeController
	{
		private GamecubeCalibration _currentCalibration;
		private GamecubeInputState _currentState;
		private ControllerSettings _settings;
		private IXbox360Controller _controller;

		private int _vibrationMotor;
		private bool _rumbleEnabled;

		public bool RumbleChanged
		{
			get { return _rumbleEnabled; }
			set
			{
				_rumbleEnabled = value;
				if (!_rumbleEnabled)
				{
					Task.Delay(1000).ContinueWith(delegate
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
			_currentCalibration = new GamecubeCalibration();
			_settings = new ControllerSettings();

			Init();
		}

		private void Init()
		{
			// if (_settings.EnableRumble) // This might cause problems if someone changes the settings after initialization
			_controller.FeedbackReceived += FeedbackReceived;
			_controller.AutoSubmitReport = false;

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
			_controller.FeedbackReceived -= FeedbackReceived;
			_controller.Disconnect();
			IsConnected = false;
		}

		public void UpdateInput(GamecubeInputState inputState)
		{
			if (!_currentState.IsEqual(inputState) && IsConnected)
			{
				_controller.SetButtonState(Xbox360Button.A, _settings.SwapAB ? inputState.BUTTON_B : inputState.BUTTON_A);
				_controller.SetButtonState(Xbox360Button.B, _settings.SwapAB ? inputState.BUTTON_A : inputState.BUTTON_B);
				_controller.SetButtonState(Xbox360Button.X, _settings.SwapXY ? inputState.BUTTON_Y : inputState.BUTTON_X);
				_controller.SetButtonState(Xbox360Button.Y, _settings.SwapXY ? inputState.BUTTON_X : inputState.BUTTON_Y);
				_controller.SetButtonState(Xbox360Button.Left, inputState.DPAD_LEFT);
				_controller.SetButtonState(Xbox360Button.Right, inputState.DPAD_RIGHT);
				_controller.SetButtonState(Xbox360Button.Up, inputState.DPAD_UP);
				_controller.SetButtonState(Xbox360Button.Down, inputState.DPAD_DOWN);
				_controller.SetButtonState(Xbox360Button.Start, inputState.BUTTON_START);

				if (_settings.EnableDigitalPress)
				{
					bool leftShoulderPress = inputState.ANALOG_LEFT / 255f > _settings.TriggerDeadzone;
					bool rightShoulderPress = inputState.ANALOG_RIGHT / 255f > _settings.TriggerDeadzone;
					int zButtonPress = (inputState.BUTTON_Z ? 255 : 0);

					_controller.SetButtonState(Xbox360Button.LeftShoulder, leftShoulderPress);
					_controller.SetButtonState(Xbox360Button.RightShoulder, rightShoulderPress);
					_controller.SetSliderValue(Xbox360Slider.RightTrigger, (byte)zButtonPress);
				}

				else
				{
					_controller.SetSliderValue(Xbox360Slider.LeftTrigger, ClampTriggers(inputState.ANALOG_LEFT, _settings.TriggerDeadzone, _settings.TriggerThreshold));
					_controller.SetSliderValue(Xbox360Slider.RightTrigger, ClampTriggers(inputState.ANALOG_RIGHT, _settings.TriggerDeadzone, _settings.TriggerThreshold));
					_controller.SetButtonState(Xbox360Button.RightShoulder, inputState.BUTTON_Z);
				}

				double LeftStickX = inputState.LEFT_STICK_X;
				double LeftStickY = inputState.LEFT_STICK_Y;
				double CStickX = inputState.C_STICK_X;
				double CStickY = inputState.C_STICK_Y;

				if (_currentCalibration.CurrentStatus == CalibrationStatus.Calibrated)
				{
					LeftStickX = Math.Round(LeftStickX * _currentCalibration.LeftStickCalibration[0] - _currentCalibration.LeftStickCalibration[2]);
					LeftStickY = Math.Round(LeftStickY * _currentCalibration.LeftStickCalibration[1] - _currentCalibration.LeftStickCalibration[3]);
					CStickX = Math.Round(CStickX * _currentCalibration.CStickCalibration[0] - _currentCalibration.CStickCalibration[2]);
					CStickY = Math.Round(CStickY * _currentCalibration.CStickCalibration[1] - _currentCalibration.CStickCalibration[3]);
				}

				else if (_currentCalibration.CurrentStatus == CalibrationStatus.Centered)
				{
					LeftStickX += _currentCalibration.StickCenters[0];
					LeftStickY += _currentCalibration.StickCenters[1];
					CStickX += _currentCalibration.StickCenters[2];
					CStickY += _currentCalibration.StickCenters[3];
				}
				LeftStickX = Extensions.ByteToShort((byte)Extensions.Clamp(LeftStickX, 0.0, 255.0), false);
				LeftStickY = Extensions.ByteToShort((byte)Extensions.Clamp(LeftStickY, 0.0, 255.0), false);
				CStickX = Extensions.ByteToShort((byte)Extensions.Clamp(CStickX, 0.0, 255.0), false);
				CStickY = Extensions.ByteToShort((byte)Extensions.Clamp(CStickY, 0.0, 255.0), false);

				if (CheckRadialDeadzone((float)LeftStickX, (float)LeftStickY, _settings.LeftStickDeadzone))
				{
					LeftStickX = 0.0;
					LeftStickY = 0.0;
				}
				if (CheckRadialDeadzone((float)CStickX, (float)CStickY, _settings.RightStickDeadzone))
				{
					CStickX = 0.0;
					CStickY = 0.0;
				}

				_controller.SetAxisValue(Xbox360Axis.LeftThumbX, (short)LeftStickX);
				_controller.SetAxisValue(Xbox360Axis.LeftThumbY, (short)LeftStickY);
				_controller.SetAxisValue(Xbox360Axis.RightThumbX, (short)CStickX);
				_controller.SetAxisValue(Xbox360Axis.RightThumbY, (short)CStickY);
				_controller.SubmitReport();
			}
		}

		public void UpdateSettings(ControllerSettings controllerSettings)
		{
			_settings = controllerSettings;
		}

		public void UpdateCalibration(GamecubeCalibration controllerCalibration)
		{
			_currentCalibration = controllerCalibration;
		}

		private static byte ClampTriggers(byte triggerSlider, float triggerDeadzone, float triggerThreshold)
		{
			float compare = triggerSlider / 255f;
			if (triggerDeadzone > compare)
				return 0;

			if (triggerThreshold < compare)
				return 255;
			return triggerSlider;
		}

		private bool CheckRadialDeadzone(float x, float y, float deadzone)
		{
			if (deadzone <= 0f)
				return false;

			float rad = 32767f * deadzone;
			if ((x - 0f) * (x - 0f) + (y - 0f) * (y - 0f) < rad * rad)
				return true;

			return false;
		}

		private void FeedbackReceived(object sender, Xbox360FeedbackReceivedEventArgs e)
		{
			_vibrationMotor = e.LargeMotor;
			RumbleChanged = _vibrationMotor > 0;
		}
	}
}
