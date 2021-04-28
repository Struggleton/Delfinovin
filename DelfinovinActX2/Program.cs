using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        private static bool _IsCalibrating;

        private static GamecubeAdapter _gamecubeAdapter;
        private static byte[] _rumbleCommand;

        private static CancellationTokenSource _cancelTokenSrc = new CancellationTokenSource();
        private static CancellationToken keyInputToken = _cancelTokenSrc.Token;
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
                            menuContinue = false;
                            BeginControllerLoop();
                            break;
                        case 2:
                            Console.WriteLine(Strings.MENU_CREDITS);
                            break;
                        case 3:
                            Console.WriteLine(Strings.MENU_SETUP);
                            break;
                        case 4:
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
            Console.Clear();

            _IsRunning = true;
            _controllerReader.DataReceived += CtrlrDataReceived;

            Task.Run(() => WaitForInput(), keyInputToken);
            //DeinitializeUSB();
        }

        private static void InitializeAdapter()
        {
            try
            {
                _gamecubeAdapter = new GamecubeAdapter();
                _rumbleCommand = new byte[] { 0x11, 0x00, 0x00, 0x00, 0x00 }; // set up rumble command

                ErrorCode ec = _controllerWriter.Write(new byte[] { 0x13 }, 5000, out int transferLength); // send start command
            }

            catch (Exception ex)
            {
                throw new Exception(Strings.ERROR_WRITEFAILED);
            }
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
            if (_IsRunning)
            {
                Console.WriteLine("In order to calibrate your controllers, press enter and rotate the control sticks on the connected controllers.");
                Console.WriteLine("Press backspace to stop calibrating.");
                Console.WriteLine();

                _gamecubeAdapter.UpdateStates(e.Buffer);
                _gamecubeAdapter.UpdateControllers();

                if (_IsCalibrating)
                    _gamecubeAdapter.UpdateCalibrations();

                if (_gamecubeAdapter.RumbleChanged && ApplicationSettings.EnableRumble)
                    SendRumble();
                if (ApplicationSettings.EnableRawPrint)
                    PrintDebugInfo();
            }
        }

        private static async void WaitForInput()
        {
            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    _IsCalibrating = true;
                }

                if (Console.ReadKey(true).Key == ConsoleKey.Backspace)
                {
                    _IsCalibrating = false;
                }
            }
        }

        private static void SendRumble()
        {
            for (int i = 0; i < 4; i++)
            {
                byte currentRumbleState = Extensions.BoolToByte(_gamecubeAdapter.Controllers[i].RumbleChanged);
                _rumbleCommand[i + 1] = currentRumbleState;
            }
            
            ErrorCode ec = _controllerWriter.Write(_rumbleCommand, 5000, out int transferLength);

            if (ec != ErrorCode.None)
                throw new Exception(Strings.ERROR_WRITEFAILED);
        }
        private static void PrintDebugInfo()
        {
            if (ApplicationSettings.EnableRawPrint)
                _gamecubeAdapter.PrintStates();

            Console.SetCursorPosition(0, 0);
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