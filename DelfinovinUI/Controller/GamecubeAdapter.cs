using System;
using Nefarius.ViGEm.Client;

namespace DelfinovinUI
{
	internal class GamecubeAdapter
	{
		private GamecubeInputState[] _inputStates = new GamecubeInputState[4];
		private ViGEmClient[] _vgmClient = new ViGEmClient[4];
		private GamecubeCalibration[] _previousStickCenters;
		private bool[] _previousRumbleStates;

		public GamecubeController[] Controllers;
		public bool ControllerInserted;
		public bool ControllerDisconnected;
		public bool RumbleChanged;

		public GamecubeAdapter()
		{
			// Initialize all of our classes to be used here
			Controllers = new GamecubeController[4];
			_previousStickCenters = new GamecubeCalibration[4];
			_previousRumbleStates = new bool[4];

			for (int i = 0; i < 4; i++)
			{
				// Create 4 clients. I'm actually not sure if this is necessary.
				_vgmClient[i] = new ViGEmClient();
				Controllers[i] = new GamecubeController(_vgmClient[i]);

				_previousStickCenters[i] = new GamecubeCalibration();
				_inputStates[i] = new GamecubeInputState();
			}
		}

		public void UpdateStates(byte[] controllerData)
		{
			// The first byte is a magic byte. If
			// this is not found, the data is invalid.
			if (controllerData[0] != 0x21)
				throw new Exception("Error! Gamecube magic header not found!");

			for (int port = 0; port < 4; port++)
			{
				// Get a byte and work on each bit. Fill out each inputState
				// using the data from each 36 bytes.
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
			// Set this up for checking rumble later.
			RumbleChanged = false;

			// Iterate through all controller ports.
			for (int i = 0; i < 4; i++)
			{
				if (_inputStates[i].IsPlugged())
				{
					// Newly inserted controller, do controller init
					if (!Controllers[i].IsConnected)
					{
						ControllerInserted = true;
						Controllers[i].Connect();
					}

					// Only center sticks when fist plugged in
					if (!Controllers[i].IsCentered) 
					{
						CalibrateCenter(i);
					}

					// This gets set by the UI 
					else if (Controllers[i].CalibrationStatus == CalibrationStatus.Calibrating) 
                    {
						Controllers[i].Calibration.SetMinMax(_inputStates[i]);
						Controllers[i].Calibration.GenerateCalibrations();
					}

					// Update each controller's inputs using the inputStates
					Controllers[i].UpdateInput(_inputStates[i]);

					// Check if any of the controllers had their RumbleChanged property set.
					// If so, this will prompt the mainWindow's code to send rumble commands.
					RumbleChanged |= _previousRumbleStates[i] != Controllers[i].RumbleChanged;

					// Set the previous to the current one.
					_previousRumbleStates[i] = Controllers[i].RumbleChanged;
				}

				// This means that the controller is still connected even 
				// though the input state says the controller is unplugged.
				else if (Controllers[i].IsConnected)
				{
					// Create default calibrations to reset the controller to.
					_inputStates[i] = new GamecubeInputState();
					Controllers[i].Calibration = new GamecubeCalibration();

					// Send the default calibrations and disconnect the controller.
					Controllers[i].UpdateInput(_inputStates[i]);
					Controllers[i].Disconnect();

					ControllerDisconnected = true;
				}
			}
		}

		// This function is to verify sticks use proper values.
		// When first initializing the adapter, the adapter can send
		// malformed packets and give incorrect stick values to calibrate from.
		private void CalibrateCenter(int port)
        {
			// Assume the current stick values are the center.
			Controllers[port].Calibration.SetStickOrigins(_inputStates[port]);

			// If the previous stick values are the same as the current ones,
			// Increment the calibrationAttempt value
			if (_previousStickCenters[port].CompareStickOrigins(Controllers[port].Calibration))
			{
				Controllers[port].CalibrationAttempt++;
			}

			// The stickOrigins moved during calibration 
			else
			{
				// Reset the attempt counter and update the previous stickCenters
				// with the new ones.
				Controllers[port].CalibrationAttempt = 0;
				_previousStickCenters[port] = Controllers[port].Calibration;
			}

			// If the calibrationAttempt counter passes 5, these are the proper stick origins.
			if (Controllers[port].CalibrationAttempt >= 5)
			{
				Controllers[port].IsCentered = true;
				Controllers[port].CalibrationStatus = CalibrationStatus.Centered;
			}
		}

		public void UpdateSettings(ControllerSettings controllerSettings, int port)
		{
			// Update the GamecubeController with the new settings.
			Controllers[port].Settings = controllerSettings;

			// Save the previous stick origins.
			int[] stickCenters = Controllers[port].Calibration.StickOrigins;

			// Copy current stick centers to a new calibration.
			Controllers[port].Calibration = new GamecubeCalibration();
			Controllers[port].Calibration.StickOrigins = stickCenters;

			// We want to generate new calibrations based on the settings passed
			GamecubeInputState input = new GamecubeInputState();

			// Get the maximum values based on what the controller profile ranges provide
			input.LEFT_STICK_X = (byte)Math.Floor(255f * controllerSettings.LeftStickRange); 
			input.LEFT_STICK_Y = (byte)Math.Floor(255f * controllerSettings.LeftStickRange);
			input.C_STICK_X = (byte)Math.Floor(255f * controllerSettings.RightStickRange);
			input.C_STICK_Y = (byte)Math.Floor(255f * controllerSettings.RightStickRange);

			// Send our inputState
			Controllers[port].Calibration.SetMinMax(input);

			// Get the minimum values based on what the controller profile ranges provide
			input.LEFT_STICK_X = (byte)Math.Floor(255f * (1f - controllerSettings.LeftStickRange));
			input.LEFT_STICK_Y = (byte)Math.Floor(255f * (1f - controllerSettings.LeftStickRange));
			input.C_STICK_X = (byte)Math.Floor(255f * (1f - controllerSettings.RightStickRange));
			input.C_STICK_Y = (byte)Math.Floor(255f * (1f - controllerSettings.RightStickRange));

			// Send our inputState
			Controllers[port].Calibration.SetMinMax(input);

			// Generate the new calibration coeffients + and update the current 
			// controller calibration status to "Calibrated."
			Controllers[port].Calibration.GenerateCalibrations();
			Controllers[port].CalibrationStatus = CalibrationStatus.Calibrated;
		}

		public void UpdateDialog(GamecubeDialog controllerControl, int port)
		{
			// Update the gamecube controller UI based on the current input state. 
			controllerControl.UpdateDialog(_inputStates[port], Controllers[port].Settings, Controllers[port].Calibration);
		}
	}
}
