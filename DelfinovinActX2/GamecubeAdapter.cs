using Nefarius.ViGEm.Client;
using System;

namespace DelfinovinActX2
{
    class GamecubeAdapter
    {
        private GamecubeInputState[] _inputStates = new GamecubeInputState[4];
        private ViGEmClient[] _vgmClient = new ViGEmClient[4];

        public GamecubeController[] Controllers;

        private bool[] _previousRumbleStates;
        public bool RumbleChanged;

        public GamecubeAdapter()
        {
            Controllers = new GamecubeController[4];
            _previousRumbleStates = new bool[4];

            for (int i = 0; i < 4; i++)
            {
                _vgmClient[i] = new ViGEmClient();
                Controllers[i] = new GamecubeController(_vgmClient[i]);
                _inputStates[i] = new GamecubeInputState();
            }
        }

        public void UpdateStates(byte[] controllerData)
        {
            if (controllerData[0] != 0x21)
                throw new Exception("Magic missing exception");

            for (int port = 0; port < 4; port++)
            {
                byte workingByte = controllerData[((port * 9) + 1)];
                _inputStates[port].IsPowered = Extensions.GetBit(workingByte, 2);
                _inputStates[port].NormalType = Extensions.GetBit(workingByte, 4);
                _inputStates[port].WavebirdType = Extensions.GetBit(workingByte, 5);

                workingByte = controllerData[((port * 9) + 2)];
                _inputStates[port].BUTTON_A = Extensions.GetBit(workingByte, 0);
                _inputStates[port].BUTTON_B = Extensions.GetBit(workingByte, 1);
                _inputStates[port].BUTTON_X = Extensions.GetBit(workingByte, 2);
                _inputStates[port].BUTTON_Y = Extensions.GetBit(workingByte, 3);
                _inputStates[port].DPAD_LEFT = Extensions.GetBit(workingByte, 4);
                _inputStates[port].DPAD_RIGHT = Extensions.GetBit(workingByte, 5);
                _inputStates[port].DPAD_DOWN = Extensions.GetBit(workingByte, 6);
                _inputStates[port].DPAD_UP = Extensions.GetBit(workingByte, 7);

                workingByte = controllerData[((port * 9) + 3)];
                _inputStates[port].BUTTON_START = Extensions.GetBit(workingByte, 0);
                _inputStates[port].BUTTON_Z = Extensions.GetBit(workingByte, 1);
                _inputStates[port].BUTTON_R = Extensions.GetBit(workingByte, 2);
                _inputStates[port].BUTTON_L = Extensions.GetBit(workingByte, 3);

                _inputStates[port].LEFT_STICK_X = controllerData[((port * 9) + 4)];
                _inputStates[port].LEFT_STICK_Y = controllerData[((port * 9) + 5)];
                _inputStates[port].C_STICK_X = controllerData[((port * 9) + 6)];
                _inputStates[port].C_STICK_Y = controllerData[((port * 9) + 7)];

                _inputStates[port].ANALOG_LEFT = controllerData[((port * 9) + 8)];
                _inputStates[port].ANALOG_RIGHT = controllerData[((port * 9) + 9)];
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
                        if (ApplicationSettings.CalibrateCenter)
                        {
                            Controllers[i].SetStickCenters(_inputStates[i]);
                        }
                    }
                        
                    Controllers[i].UpdateInput(_inputStates[i]);
                    RumbleChanged |= _previousRumbleStates[i] != Controllers[i].RumbleChanged;
                    _previousRumbleStates[i] = Controllers[i].RumbleChanged;
                }

                else
                {
                    if (Controllers[i].IsConnected)
                    {
                        _inputStates[i] = new GamecubeInputState(); // Reset to default before disconnecting
                        Controllers[i].UpdateInput(_inputStates[i]);
                        Controllers[i].SetStickCenters(_inputStates[i]);
                        Controllers[i].ResetCalibration();
                        Controllers[i].Disconnect();
                    }
                }
            }
        }

        public void UpdateCalibrations()
        {
            for (int i = 0; i < 4; i++)
            {
                if (_inputStates[i].IsPlugged())
                {
                    Controllers[i].SetMinMax(_inputStates[i]);
                    Controllers[i].GenerateCalibrations();
                }
            }
        }

        public void PrintStates()
        {
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine(Controllers[i].Calibration.CurrentStatus);
                Console.WriteLine(_inputStates[i].ToString());
            }
        }
    }
}