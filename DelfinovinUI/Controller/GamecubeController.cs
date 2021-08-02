using System;
using System.Threading.Tasks;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace DelfinovinUI
{
	internal class GamecubeController
	{
		private IXbox360Controller _controller;
		private GamecubeInputState _currentState;

		private int _vibrationMotor;
		private bool _rumbleEnabled;

		public GamecubeCalibration Calibration;
		public ControllerSettings Settings;
		public CalibrationStatus CalibrationStatus;
		public bool IsCentered;
		public bool IsConnected;
		public int CalibrationAttempt; 

		public bool RumbleChanged
		{
			get { return _rumbleEnabled; }
			set
			{
				_rumbleEnabled = value;

				// If the rumble continues for 1 second without
				// changes in strength, end the rumble
				if (!_rumbleEnabled)
				{
					Task.Delay(1000).ContinueWith(delegate
					{
						_vibrationMotor = 0;
					});
				}
			}
		}

		public GamecubeController(ViGEmClient client)
		{
			// Create a new controller device
			_controller = client.CreateXbox360Controller();

			// Initialize settings using default settings/calibrations
			Calibration = new GamecubeCalibration();
			Settings = new ControllerSettings();
			
			Init();
		}

		private void Init()
		{
			// if (_settings.EnableRumble) // This might cause problems if someone changes the settings after initialization
			_controller.FeedbackReceived += FeedbackReceived;

			// Disable AutoSubmitReport - we want to fill out all the buttons
			// and send the reports ourself. This setting will send the report
			// when the properties are set.
			_controller.AutoSubmitReport = false;

			_currentState = new GamecubeInputState();
			CalibrationStatus = CalibrationStatus.Uncalibrated;
			
			// Initialize this for calibration attempts
			CalibrationAttempt = 0;
		}

		public void Connect()
		{
			// Create the XInput
			_controller.Connect();

			UpdateInput(new GamecubeInputState());

			// Everything is connected, set this to true.
			IsConnected = true;
		}

		public void Disconnect()
		{
			// Unsubscribe from the rumble event.
			_controller.FeedbackReceived -= FeedbackReceived;
			CalibrationStatus = CalibrationStatus.Uncalibrated;
			CalibrationAttempt = 0;

			// Disconnect the XInput device
			_controller.Disconnect();
			IsConnected = false;
			IsCentered = false;
		}

		public void UpdateInput(GamecubeInputState inputState)
		{
			if (!_currentState.IsEqual(inputState) && IsConnected)
			{
				// Set all the buttons based on the currently set controller profile
				_controller.SetButtonState(Xbox360Button.A, Settings.SwapAB ? inputState.BUTTON_B : inputState.BUTTON_A);
				_controller.SetButtonState(Xbox360Button.B, Settings.SwapAB ? inputState.BUTTON_A : inputState.BUTTON_B);
				_controller.SetButtonState(Xbox360Button.X, Settings.SwapXY ? inputState.BUTTON_Y : inputState.BUTTON_X);
				_controller.SetButtonState(Xbox360Button.Y, Settings.SwapXY ? inputState.BUTTON_X : inputState.BUTTON_Y);

				_controller.SetButtonState(Xbox360Button.Left, inputState.DPAD_LEFT);
				_controller.SetButtonState(Xbox360Button.Right, inputState.DPAD_RIGHT);
				_controller.SetButtonState(Xbox360Button.Up, inputState.DPAD_UP);
				_controller.SetButtonState(Xbox360Button.Down, inputState.DPAD_DOWN);
				_controller.SetButtonState(Xbox360Button.Start, inputState.BUTTON_START);

				// The user set EnableDigitalPress
				if (Settings.EnableDigitalPress)
				{
					// This means we use digital values + triggers instead of analog values
					bool leftShoulderPress = inputState.ANALOG_LEFT / 255f > Settings.TriggerDeadzone;
					bool rightShoulderPress = inputState.ANALOG_RIGHT / 255f > Settings.TriggerDeadzone;

					// Convert the Z button press into analog so we can use the slider value
					int zButtonPress = (inputState.BUTTON_Z ? 255 : 0);

					_controller.SetButtonState(Xbox360Button.LeftShoulder, leftShoulderPress);
					_controller.SetButtonState(Xbox360Button.RightShoulder, rightShoulderPress);
					_controller.SetSliderValue(Xbox360Slider.RightTrigger, (byte)zButtonPress);
				}

				else
				{
					_controller.SetSliderValue(Xbox360Slider.LeftTrigger, ClampTriggers(inputState.ANALOG_LEFT, Settings.TriggerDeadzone, Settings.TriggerThreshold));
					_controller.SetSliderValue(Xbox360Slider.RightTrigger, ClampTriggers(inputState.ANALOG_RIGHT, Settings.TriggerDeadzone, Settings.TriggerThreshold));
					_controller.SetButtonState(Xbox360Button.RightShoulder, inputState.BUTTON_Z);
				}

				double LeftStickX = inputState.LEFT_STICK_X;
				double LeftStickY = inputState.LEFT_STICK_Y;
				double CStickX = inputState.C_STICK_X;
				double CStickY = inputState.C_STICK_Y;

				// The formula goes like 
				// round(stick_value * stick_mul - stick_offset)
				if (CalibrationStatus == CalibrationStatus.Calibrated)
				{
					LeftStickX = (127 - Calibration.StickOrigins[0]) + Math.Round((LeftStickX * Calibration.LeftStickCalibration[0] - Calibration.LeftStickCalibration[2]));
					LeftStickY = (127 - Calibration.StickOrigins[1]) + Math.Round((LeftStickY * Calibration.LeftStickCalibration[1] - Calibration.LeftStickCalibration[3]));
					CStickX = (127 - Calibration.StickOrigins[2]) + Math.Round((CStickX * Calibration.CStickCalibration[0] - Calibration.CStickCalibration[2]));
					CStickY = (127 - Calibration.StickOrigins[3]) + Math.Round((CStickY * Calibration.CStickCalibration[1] - Calibration.CStickCalibration[3]));
				}

				// Use the stick origins gathered from before to center the stick
				// to a normal 127 stick value.
				else if (CalibrationStatus == CalibrationStatus.Centered)
				{
					LeftStickX += 127 - Calibration.StickOrigins[0];
					LeftStickY += 127 - Calibration.StickOrigins[1];
					CStickX += 127 - Calibration.StickOrigins[2];
					CStickY += 127 - Calibration.StickOrigins[3];
				}

				// Scale the sticks to a signed short value and clamp the values
				// to make sure they fit within the range of a byte.
				LeftStickX = Extensions.ByteToShort((byte)Extensions.Clamp(LeftStickX, 0.0, 255.0), false);
				LeftStickY = Extensions.ByteToShort((byte)Extensions.Clamp(LeftStickY, 0.0, 255.0), false);
				CStickX = Extensions.ByteToShort((byte)Extensions.Clamp(CStickX, 0.0, 255.0), false);
				CStickY = Extensions.ByteToShort((byte)Extensions.Clamp(CStickY, 0.0, 255.0), false);
				
				// Compare the stick values to the deadzone. If they are within the range
				// of the deadzone, set it to the center.
				if (CheckRadialDeadzone((float)LeftStickX, (float)LeftStickY, Settings.LeftStickDeadzone))
				{
					LeftStickX = 0.0;
					LeftStickY = 0.0;
				}

				if (CheckRadialDeadzone((float)CStickX, (float)CStickY, Settings.RightStickDeadzone))
				{
					CStickX = 0.0;
					CStickY = 0.0;
				}

				// Set the axis values.
				_controller.SetAxisValue(Xbox360Axis.LeftThumbX, (short)LeftStickX);
				_controller.SetAxisValue(Xbox360Axis.LeftThumbY, (short)LeftStickY);
				_controller.SetAxisValue(Xbox360Axis.RightThumbX, (short)CStickX);
				_controller.SetAxisValue(Xbox360Axis.RightThumbY, (short)CStickY);

				// Submit the report to ViGEmBus
				_controller.SubmitReport();
			}
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
