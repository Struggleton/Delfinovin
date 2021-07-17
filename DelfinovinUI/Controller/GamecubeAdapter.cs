using System;
using Nefarius.ViGEm.Client;

namespace DelfinovinUI
{
	internal class GamecubeAdapter
	{
		private GamecubeInputState[] _inputStates = new GamecubeInputState[4];
		private ViGEmClient[] _vgmClient = new ViGEmClient[4];
		private bool[] _previousRumbleStates;

		public GamecubeController[] Controllers;
		public GamecubeCalibration[] Calibrations;
		public ControllerSettings[] Settings;
		
		public bool ControllerInserted;
		public bool RumbleChanged;

		public GamecubeAdapter()
		{
			Controllers = new GamecubeController[4];
			Calibrations = new GamecubeCalibration[4];
			Settings = new ControllerSettings[4];
			_previousRumbleStates = new bool[4];
			for (int i = 0; i < 4; i++)
			{
				_vgmClient[i] = new ViGEmClient();
				Controllers[i] = new GamecubeController(_vgmClient[i]);
				Calibrations[i] = new GamecubeCalibration();
				Settings[i] = new ControllerSettings();
				_inputStates[i] = new GamecubeInputState();
			}
		}

		public void UpdateStates(byte[] controllerData)
		{
			if (controllerData[0] != 0x21)
			{
				throw new Exception("Error! Gamecube magic header not found!");
			}

			for (int port = 0; port < 4; port++)
			{
				byte workingByte = controllerData[port * 9 + 1];
				_inputStates[port].IsPowered = Extensions.GetBit(workingByte, 2);
				_inputStates[port].NormalType = Extensions.GetBit(workingByte, 4);
				_inputStates[port].WavebirdType = Extensions.GetBit(workingByte, 5);
				workingByte = controllerData[port * 9 + 2];
				_inputStates[port].BUTTON_A = Extensions.GetBit(workingByte, 0);
				_inputStates[port].BUTTON_B = Extensions.GetBit(workingByte, 1);
				_inputStates[port].BUTTON_X = Extensions.GetBit(workingByte, 2);
				_inputStates[port].BUTTON_Y = Extensions.GetBit(workingByte, 3);
				_inputStates[port].DPAD_LEFT = Extensions.GetBit(workingByte, 4);
				_inputStates[port].DPAD_RIGHT = Extensions.GetBit(workingByte, 5);
				_inputStates[port].DPAD_DOWN = Extensions.GetBit(workingByte, 6);
				_inputStates[port].DPAD_UP = Extensions.GetBit(workingByte, 7);
				workingByte = controllerData[port * 9 + 3];
				_inputStates[port].BUTTON_START = Extensions.GetBit(workingByte, 0);
				_inputStates[port].BUTTON_Z = Extensions.GetBit(workingByte, 1);
				_inputStates[port].BUTTON_R = Extensions.GetBit(workingByte, 2);
				_inputStates[port].BUTTON_L = Extensions.GetBit(workingByte, 3);
				_inputStates[port].LEFT_STICK_X = controllerData[port * 9 + 4];
				_inputStates[port].LEFT_STICK_Y = controllerData[port * 9 + 5];
				_inputStates[port].C_STICK_X = controllerData[port * 9 + 6];
				_inputStates[port].C_STICK_Y = controllerData[port * 9 + 7];
				_inputStates[port].ANALOG_LEFT = controllerData[port * 9 + 8];
				_inputStates[port].ANALOG_RIGHT = controllerData[port * 9 + 9];
			}
		}

		public void UpdateControllers()
		{
			RumbleChanged = false;
			for (int i = 0; i < 4; i++)
			{
				if (_inputStates[i].IsPlugged())
				{
					if (!Controllers[i].IsConnected)
					{
						Controllers[i].Connect();
						ControllerInserted = true;
						if (ApplicationSettings.CalibrateCenter)
						{
							Calibrations[i].SetStickCenters(_inputStates[i]);
							Controllers[i].UpdateCalibration(Calibrations[i]);
						}
					}
					Controllers[i].UpdateInput(_inputStates[i]);
					RumbleChanged |= _previousRumbleStates[i] != Controllers[i].RumbleChanged;
					_previousRumbleStates[i] = Controllers[i].RumbleChanged;
				}

				else if (Controllers[i].IsConnected)
				{
					_inputStates[i] = new GamecubeInputState();
					Calibrations[i] = new GamecubeCalibration();
					Controllers[i].UpdateInput(_inputStates[i]);
					Controllers[i].UpdateCalibration(Calibrations[i]);
					Controllers[i].Disconnect();
				}
			}
		}

		public void UpdateCalibrations(int port)
		{
			if (_inputStates[port].IsPlugged())
			{
				Calibrations[port].SetMinMax(_inputStates[port]);
				Calibrations[port].GenerateCalibrations();
				Controllers[port].UpdateCalibration(Calibrations[port]);

				int breakPoint = 9;
			}
		}

		public void UpdateSettings(ControllerSettings controllerSettings, int port)
		{
			Settings[port] = controllerSettings;
			int[] stickCenters = Calibrations[port].StickCenters;
			Calibrations[port] = new GamecubeCalibration();
			Calibrations[port].StickCenters = stickCenters; // Copy current stick centers

			GamecubeInputState input = new GamecubeInputState(); // Generate new calibrations based on settings
			input.LEFT_STICK_X = (byte)Math.Floor(255f * Settings[port].LeftStickRange); // Maximum range calibration
			input.LEFT_STICK_Y = (byte)Math.Floor(255f * Settings[port].LeftStickRange);
			input.C_STICK_X = (byte)Math.Floor(255f * Settings[port].RightStickRange);
			input.C_STICK_Y = (byte)Math.Floor(255f * Settings[port].RightStickRange);
			Calibrations[port].SetMinMax(input);

			// Minimum range calibration
			input.LEFT_STICK_X = (byte)Math.Floor(255f * (1f - Settings[port].LeftStickRange));
			input.LEFT_STICK_Y = (byte)Math.Floor(255f * (1f - Settings[port].LeftStickRange));
			input.C_STICK_X = (byte)Math.Floor(255f * (1f - Settings[port].RightStickRange));
			input.C_STICK_Y = (byte)Math.Floor(255f * (1f - Settings[port].RightStickRange));
			Calibrations[port].SetMinMax(input);

			Calibrations[port].GenerateCalibrations();
			Controllers[port].UpdateSettings(Settings[port]);
			Controllers[port].UpdateCalibration(Calibrations[port]);
		}

		public void UpdateDialog(GamecubeDialog controllerControl, int port)
		{
			controllerControl.UpdateDialog(_inputStates[port], Settings[port]);
		}
	}
}
