using LibUsbDotNet;
using LibUsbDotNet.Main;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client.Targets;
using System;

namespace Delfinovin
{
    public class Delfinovin
    {
        public static UsbDevice MyUsbDevice;
        public static UsbDeviceFinder MyUsbFinder = new UsbDeviceFinder(0x057E, 0x0337);

        private static ViGEmClient[] _controllerClients;
        private static IXbox360Controller[] _XInputControllers;

        public static void Main(string[] args)
        {
            bool menuContinue = true;
            ApplicationSettings.LoadSettings();

            while (menuContinue)
            {
                Console.WriteLine(Strings.PROGRAM_NAME);
                Console.WriteLine(Strings.MENU_DIVIDER);
                Console.WriteLine(Strings.MENU_OPTIONS);

                string input = Console.ReadLine();
                Console.WriteLine();

                if (int.TryParse(input, out int numInput))
                {
                    switch (numInput)
                    {
                        case 1:
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

        public static void BeginControllerLoop()
        {
            
            ErrorCode ec = ErrorCode.None;

            try
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

                Console.Clear();
                Console.WriteLine(Strings.MENU_BEGINNING);
                UsbEndpointReader reader = MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
                ec = ReadControllerData(reader, ec);

                Console.WriteLine(Strings.MENU_COMPLETE);
            }

            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine((ec != ErrorCode.None ? ec + ":" : String.Empty) + ex.Message);
            }

            finally
            {
                if (MyUsbDevice != null)
                {
                    if (MyUsbDevice.IsOpen)
                    {
                        IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
                        if (!ReferenceEquals(wholeUsbDevice, null))
                            wholeUsbDevice.ReleaseInterface(0);

                        MyUsbDevice.Close();
                    }

                    MyUsbDevice = null;
                    UsbDevice.Exit();
                }
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
            // controller data is 37 bytes between 4 ports
            byte[] controllerData = new byte[37];

            InitializeControllers();
            while (ec == ErrorCode.None)
            {
                int bytesRead;
                ec = reader.Read(controllerData, 5000, out bytesRead);

                if (bytesRead == 0)
                {
                    throw new Exception(string.Format("", ec));
                }
                    

                Adapter adapter = new Adapter(controllerData);
                
                foreach (int port in ApplicationSettings.PortsEnabled)
                {
                    adapter.Controllers[port].UpdateController(_XInputControllers[port]);
                    if (ApplicationSettings.EnableRawPrint)
                    {
                        PrintControllerInfo(adapter, controllerData);
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

        public static void PrintControllerInfo(Adapter adapter, byte[] rawData)
        {
            Console.SetCursorPosition(0, 1);
            Console.CursorVisible = false;
            foreach (int port in ApplicationSettings.PortsEnabled)
            {
                Console.WriteLine($"Port {port + 1}");
                Console.WriteLine(Strings.MENU_DIVIDER);

                Console.Write(string.Format(Strings.INFO_STICK,
                    adapter.Controllers[port].LEFT_STICK_X.ToString(),
                    adapter.Controllers[port].LEFT_STICK_Y.ToString()));
                Console.WriteLine();

                Console.Write(string.Format(Strings.INFO_CSTICK,
                        adapter.Controllers[port].C_STICK_X.ToString(),
                        adapter.Controllers[port].C_STICK_Y.ToString()));
                Console.WriteLine();

                Console.Write(string.Format(Strings.INFO_FACEBUTTONS,
                        ConvertYesNo(adapter.Controllers[port].BUTTON_A),
                        ConvertYesNo(adapter.Controllers[port].BUTTON_B),
                        ConvertYesNo(adapter.Controllers[port].BUTTON_X),
                        ConvertYesNo(adapter.Controllers[port].BUTTON_Y),
                        ConvertYesNo(adapter.Controllers[port].BUTTON_START)));
                Console.WriteLine();

                Console.Write(string.Format(Strings.INFO_TRIGGERS,
                        adapter.Controllers[port].ANALOG_LEFT.ToString(),
                        adapter.Controllers[port].ANALOG_RIGHT.ToString(),
                        ConvertYesNo(adapter.Controllers[port].BUTTON_Z)
                        ));
                Console.WriteLine();

                Console.Write(string.Format(Strings.INFO_DPAD,
                        ConvertYesNo(adapter.Controllers[port].DPAD_LEFT),
                        ConvertYesNo(adapter.Controllers[port].DPAD_RIGHT),
                        ConvertYesNo(adapter.Controllers[port].DPAD_UP),
                        ConvertYesNo(adapter.Controllers[port].DPAD_DOWN)));
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        public static string ConvertYesNo(bool input)
        {
            return input ? "Yes" : "No";
        }
    }
}