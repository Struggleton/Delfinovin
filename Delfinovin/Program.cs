using LibUsbDotNet;
using LibUsbDotNet.Main;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client.Targets;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Delfinovin
{
    public class Delfinovin
    {
        public static UsbDevice MyUsbDevice;
        public static UsbDeviceFinder MyUsbFinder = new UsbDeviceFinder(0x057E, 0x0337);

        private static ViGEmClient[] _controllerClients;
        private static IXbox360Controller[] _XInputControllers;
        public static Adapter _controllerAdapter = new Adapter();

        public static async Task Main(string[] args)
        {
            bool menuContinue = true;
            ApplicationSettings.LoadSettings();

            while (menuContinue)
            {
                string input = PrintMenu();
                if (int.TryParse(input, out int numInput))
                {
                    switch (numInput)
                    {
                        case 1:
                            BeginControllerLoop();
                            break;
                        case 2:
                            await CalibrateControllers();
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

        private static async void WaitForInput(CancellationTokenSource cts)
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                    {
                        cts.Cancel();
                        break;
                    }
                }
            }
        }

        public static async Task CalibrateControllers()
        {
            InitializeUSBDevice();
            UsbEndpointReader reader = MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);

            foreach (int port in ApplicationSettings.PortsEnabled)
            {
                Console.WriteLine(string.Format(Strings.MENU_CALIBRATION, port));

                var cts = new CancellationTokenSource();
                var token = cts.Token;

                Task.Run(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        _controllerAdapter.UpdateAdapter(ReadBytes(reader, 37));
                        _controllerAdapter.Controllers[port].UpdateMinMax();
                    }

                }, token);

                WaitForInput(cts);

                Console.WriteLine(Strings.MENU_CALIBRATION_COMPLETE);
            }

            CloseUSBDevice();
        }


        public static string PrintMenu()
        {
            Console.WriteLine(Strings.PROGRAM_NAME);
            Console.WriteLine(Strings.MENU_DIVIDER);
            Console.WriteLine(Strings.MENU_OPTIONS);

            string input = Console.ReadLine();
            Console.WriteLine();

            return input;
        }

        public static void BeginControllerLoop()
        {
            ErrorCode ec = ErrorCode.None;
            try
            {
                InitializeUSBDevice();
                Console.Clear();

                Console.WriteLine(Strings.MENU_LOOP_BEGINNING);
                UsbEndpointReader reader = MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
                ec = ReadControllerData(reader, ec);

                Console.WriteLine(Strings.MENU_LOOP_COMPLETE);
            }

            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine((ec != ErrorCode.None ? ec + ":" : String.Empty) + ex.Message);
            }

            finally
            {
                CloseUSBDevice();
            }
        }

        public static void InitializeControllers()
        {
            try
            {
                _controllerClients = new ViGEmClient[4];
                _XInputControllers = new IXbox360Controller[4];

                foreach (int port in ApplicationSettings.PortsEnabled)
                {
                    _controllerClients[port] = new ViGEmClient();
                    _XInputControllers[port] = _controllerClients[port].CreateXbox360Controller();
                    _XInputControllers[port].Connect();
                }
            }
            
            catch (Exception ex)
            {
                
                // TODO - Fill out the rest of the possible exceptions while initializing 
                if (ex is VigemBusNotFoundException)
                {
                    throw new Exception(Strings.EXCEPTION_EMBUSNOTFOUND);
                }
            }
        }

        public static ErrorCode ReadControllerData(UsbEndpointReader reader, ErrorCode ec = ErrorCode.None)
        {
            InitializeControllers();

            while (ec == ErrorCode.None)
            {
                byte[] controllerData = ReadBytes(reader, 37, ec);
                _controllerAdapter.UpdateAdapter(controllerData);
                
                foreach (int port in ApplicationSettings.PortsEnabled)
                {
                    _controllerAdapter.Controllers[port].UpdateController(_XInputControllers[port]);
                    if (ApplicationSettings.EnableRawPrint)
                    {
                        PrintControllerInfo();
                    }
                }

                if (ApplicationSettings.EnableRawPrint)
                {
                    Console.Write(BitConverter.ToString(controllerData));
                    Console.WriteLine();
                }
            }

            return ec;
        }

        public static void PrintControllerInfo()
        {
            Console.SetCursorPosition(0, 1);
            Console.CursorVisible = false;
            foreach (int port in ApplicationSettings.PortsEnabled)
            {
                Console.WriteLine($"Port {port + 1}");
                Console.WriteLine(Strings.MENU_DIVIDER);

                Console.Write(string.Format(Strings.INFO_STICK,
                    _controllerAdapter.Controllers[port].LEFT_STICK_X.ToString(),
                    _controllerAdapter.Controllers[port].LEFT_STICK_Y.ToString()));
                Console.WriteLine();

                Console.Write(string.Format(Strings.INFO_CSTICK,
                        _controllerAdapter.Controllers[port].C_STICK_X.ToString(),
                        _controllerAdapter.Controllers[port].C_STICK_Y.ToString()));
                Console.WriteLine();

                Console.Write(string.Format(Strings.INFO_FACEBUTTONS,
                        ConvertYesNo(_controllerAdapter.Controllers[port].BUTTON_A),
                        ConvertYesNo(_controllerAdapter.Controllers[port].BUTTON_B),
                        ConvertYesNo(_controllerAdapter.Controllers[port].BUTTON_X),
                        ConvertYesNo(_controllerAdapter.Controllers[port].BUTTON_Y),
                        ConvertYesNo(_controllerAdapter.Controllers[port].BUTTON_START)));
                Console.WriteLine();

                Console.Write(string.Format(Strings.INFO_TRIGGERS,
                        _controllerAdapter.Controllers[port].ANALOG_LEFT.ToString(),
                        _controllerAdapter.Controllers[port].ANALOG_RIGHT.ToString(),
                        ConvertYesNo(_controllerAdapter.Controllers[port].BUTTON_Z)
                        ));
                Console.WriteLine();

                Console.Write(string.Format(Strings.INFO_DPAD,
                        ConvertYesNo(_controllerAdapter.Controllers[port].DPAD_LEFT),
                        ConvertYesNo(_controllerAdapter.Controllers[port].DPAD_RIGHT),
                        ConvertYesNo(_controllerAdapter.Controllers[port].DPAD_UP),
                        ConvertYesNo(_controllerAdapter.Controllers[port].DPAD_DOWN)));
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        public static void InitializeUSBDevice()
        {
            MyUsbDevice = UsbDevice.OpenUsbDevice(MyUsbFinder);
            if (MyUsbDevice == null)
            {
                throw new Exception(Strings.ERROR_ADAPTERNOTFOUND);
            }

            IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
            if (!ReferenceEquals(wholeUsbDevice, null))
            {
                wholeUsbDevice.SetConfiguration(1);
                wholeUsbDevice.ClaimInterface(0);
            }
        }

        public static void CloseUSBDevice()
        {
            if (MyUsbDevice != null)
            {
                if (MyUsbDevice.IsOpen)
                {
                    IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        wholeUsbDevice.ReleaseInterface(0);
                        Console.WriteLine(Strings.INFO_INTERFACERELEASE);
                    }
                    MyUsbDevice.Close();
                }

                MyUsbDevice = null;
                UsbDevice.Exit();
            }
        }

        public static byte[] ReadBytes(UsbEndpointReader reader, int amount, ErrorCode ec = ErrorCode.None)
        {
            byte[] data = new byte[amount];
            int bytesRead = 0;
            ec = reader.Read(data, 5000, out bytesRead);

            if (ec != ErrorCode.None)
                throw new Exception(string.Format(Strings.ERROR_GENERIC, ec));
            if (bytesRead == 0)
                throw new Exception(string.Format(Strings.ERROR_NOBYTES, bytesRead));
            return data;
        }

        public static string ConvertYesNo(bool input)
        {
            return input ? "Yes" : "No";
        }
    }
}