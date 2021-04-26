using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Threading;

namespace DelfinovinActX2
{
    class Program
    {
        private const int _VendorID = 0x057E;
        private const int _ProductID = 0x0337;

        private static UsbDevice _usbDevice;
        private static UsbDeviceFinder _usbDeviceFinder;

        private static UsbEndpointReader _controllerReader;
        private static UsbEndpointWriter _controllerWriter;

        private static bool _IsRunning;

        private static GamecubeAdapter _gamecubeAdapter;
        private static byte[] _rumbleCommand;

        public static void Main(string[] args)
        {
            ApplicationSettings.LoadSettings();
            bool menuContinue = true;

            // Continue taking input until menuContinue is not set
            while (menuContinue)
            {
                string input = PrintMenu();
                if (int.TryParse(input, out int numInput))
                {
                    // Clear menu after taking input
                    switch (numInput)
                    {
                        case 1:
                            BeginControllerLoop();
                            break;
                        case 2:
                            //calibrateControllers
                            break;
                        case 3:
                            Console.WriteLine(Strings.MENU_CREDITS);
                            break;
                        case 4:
                            Console.WriteLine(Strings.MENU_SETUP);
                            break;
                        case 5:
                            menuContinue = false;
                            break;
                        default:
                            Console.WriteLine(Strings.ERROR_SELECTIONINVALID);
                            break;
                    }
                }

                else
                {
                    Console.WriteLine(Strings.ERROR_SELECTIONINVALID);
                }
            }
        }

        /*
        private static void GetRestingPosition()
        {
            byte[] readBuffer = new byte[37];
            ErrorCode ecRead = _controllerReader.SubmitAsyncTransfer(readBuffer, 0, readBuffer.Length, 100, out UsbTransfer readTransfer);

            WaitHandle.WaitAll(new WaitHandle[] { readTransfer.AsyncWaitHandle }, 200, false);
            if (!readTransfer.IsCompleted) readTransfer.Cancel();

            ecRead = readTransfer.Wait(out int transferredIn);
            _gamecubeAdapter.UpdateStates(readBuffer);
            //_gamecubeAdapter.SaveRestingState();
        }
        */

        private static void BeginControllerLoop()
        {
            InitializeUSB();
            InitializeAdapter();

            _IsRunning = true;
            _controllerReader.DataReceived += CtrlrDataReceived;
            //DeinitializeUSB();
        }

        private static void InitializeAdapter()
        {
            _gamecubeAdapter = new GamecubeAdapter();
            _rumbleCommand = new byte[] { 0x11, 0x00, 0x00, 0x00, 0x00 }; // set up rumble command

            _controllerWriter.Write(new byte[] { 0x13 }, 5000, out int transferLength); // send start command
        }
        private static void InitializeUSB()
        {
            _usbDeviceFinder = new UsbDeviceFinder(_VendorID, _ProductID);
            try
            {
                _usbDevice = UsbDevice.OpenUsbDevice(_usbDeviceFinder);
                if (_usbDevice == null)
                {
                    throw new Exception(Strings.ERROR_ADAPTERNOTFOUND); // couldn't find the gc adapter
                }

                IUsbDevice wholeUsbDevice = _usbDeviceFinder as IUsbDevice;
                if (!ReferenceEquals(wholeUsbDevice, null))
                {
                    wholeUsbDevice.SetConfiguration(1);
                    wholeUsbDevice.ClaimInterface(0);
                }
                
                // Setup for using events
                _controllerReader = _usbDevice.OpenEndpointReader(ReadEndpointID.Ep01, 37);
                _controllerReader.ReadThreadPriority = ThreadPriority.Highest;
                _controllerReader.DataReceivedEnabled = true;

                _controllerWriter = _usbDevice.OpenEndpointWriter(WriteEndpointID.Ep02);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void DeinitializeUSB()
        {
            if (_usbDevice != null && _usbDevice.IsOpen)
            {
                IUsbDevice wholeUsbDevice = _usbDevice as IUsbDevice;
                if (!ReferenceEquals(wholeUsbDevice, null))
                {
                    wholeUsbDevice.ReleaseInterface(0);
                }
                _usbDevice.Close();
            }


            _usbDevice = null;
            UsbDevice.Exit();

            _controllerReader.Dispose();
            _controllerWriter.Dispose();
        }

        private static void CtrlrDataReceived(object sender, EndpointDataEventArgs e)
        {

            _gamecubeAdapter.UpdateStates(e.Buffer);
            _gamecubeAdapter.UpdateControllers();

            if (_gamecubeAdapter.RumbleChanged && ApplicationSettings.EnableRumble)
                SendRumble();
            if (ApplicationSettings.EnableRawPrint)
                PrintDebugInfo();
        }

        private static void SendRumble()
        {
            for (int i = 0; i < 4; i++)
            {
                byte currentRumbleState = Extensions.BoolToByte(_gamecubeAdapter.Controllers[i].RumbleChanged);
                _rumbleCommand[i + 1] = currentRumbleState;
            }
            _controllerWriter.Write(_rumbleCommand, 5000, out int transferLength);
        }
        private static void PrintDebugInfo()
        {
            if (ApplicationSettings.EnableRawPrint)
                _gamecubeAdapter.PrintStates();

            Console.SetCursorPosition(0, 1);
            Console.CursorVisible = false;
        }
        private static string PrintMenu()
        {
            Console.WriteLine(Strings.PROGRAM_NAME);
            Console.WriteLine(Strings.MENU_DIVIDER);
            Console.WriteLine(Strings.MENU_OPTIONS);

            string input = Console.ReadLine();
            Console.WriteLine();

            return input;
        }
    }
}
