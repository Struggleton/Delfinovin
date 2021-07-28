﻿using System;
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

		public GamecubeCalibration Calibration;
		public ControllerSettings Settings;
		public CalibrationStatus CalibrationStatus;

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
			Calibration = new GamecubeCalibration();
			Settings = new ControllerSettings();

			CalibrationStatus = CalibrationStatus.Uncalibrated;

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
			CalibrationStatus = CalibrationStatus.Uncalibrated;
			_controller.Disconnect();
			IsConnected = false;
		}

		public void UpdateInput(GamecubeInputState inputState)
		{
			if (!_currentState.IsEqual(inputState) && IsConnected)
			{
				_controller.SetButtonState(Xbox360Button.A, Settings.SwapAB ? inputState.BUTTON_B : inputState.BUTTON_A);
				_controller.SetButtonState(Xbox360Button.B, Settings.SwapAB ? inputState.BUTTON_A : inputState.BUTTON_B);
				_controller.SetButtonState(Xbox360Button.X, Settings.SwapXY ? inputState.BUTTON_Y : inputState.BUTTON_X);
				_controller.SetButtonState(Xbox360Button.Y, Settings.SwapXY ? inputState.BUTTON_X : inputState.BUTTON_Y);
				_controller.SetButtonState(Xbox360Button.Left, inputState.DPAD_LEFT);
				_controller.SetButtonState(Xbox360Button.Right, inputState.DPAD_RIGHT);
				_controller.SetButtonState(Xbox360Button.Up, inputState.DPAD_UP);
				_controller.SetButtonState(Xbox360Button.Down, inputState.DPAD_DOWN);
				_controller.SetButtonState(Xbox360Button.Start, inputState.BUTTON_START);

				if (Settings.EnableDigitalPress)
				{
					bool leftShoulderPress = inputState.ANALOG_LEFT / 255f > Settings.TriggerDeadzone;
					bool rightShoulderPress = inputState.ANALOG_RIGHT / 255f > Settings.TriggerDeadzone;
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

				if (CalibrationStatus == CalibrationStatus.Calibrated)
				{
					LeftStickX = Math.Round((LeftStickX * Calibration.LeftStickCalibration[0] - Calibration.LeftStickCalibration[2]) + Calibration.StickCenters[0]);
					LeftStickY = Math.Round((LeftStickY * Calibration.LeftStickCalibration[1] - Calibration.LeftStickCalibration[3]) + Calibration.StickCenters[1]);
					CStickX = Math.Round((CStickX * Calibration.CStickCalibration[0] - Calibration.CStickCalibration[2]) + Calibration.StickCenters[2]);
					CStickY = Math.Round((CStickY * Calibration.CStickCalibration[1] - Calibration.CStickCalibration[3]) + Calibration.StickCenters[3]);
				}

				else if (CalibrationStatus == CalibrationStatus.Centered)
				{
					LeftStickX += Calibration.StickCenters[0];
					LeftStickY += Calibration.StickCenters[1];
					CStickX += Calibration.StickCenters[2];
					CStickY += Calibration.StickCenters[3];
				}

				LeftStickX = Extensions.ByteToShort((byte)Extensions.Clamp(LeftStickX, 0.0, 255.0), false);
				LeftStickY = Extensions.ByteToShort((byte)Extensions.Clamp(LeftStickY, 0.0, 255.0), false);
				CStickX = Extensions.ByteToShort((byte)Extensions.Clamp(CStickX, 0.0, 255.0), false);
				CStickY = Extensions.ByteToShort((byte)Extensions.Clamp(CStickY, 0.0, 255.0), false);

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

				_controller.SetAxisValue(Xbox360Axis.LeftThumbX, (short)LeftStickX);
				_controller.SetAxisValue(Xbox360Axis.LeftThumbY, (short)LeftStickY);
				_controller.SetAxisValue(Xbox360Axis.RightThumbX, (short)CStickX);
				_controller.SetAxisValue(Xbox360Axis.RightThumbY, (short)CStickY);
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
