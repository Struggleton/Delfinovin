using LibUsbDotNet;
using LibUsbDotNet.Main;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client.Targets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Delfinovin
{
    public class Delfinovin
    {
        private static UsbDeviceFinder _usbDeviceFinder = new UsbDeviceFinder(0x057E, 0x0337); // Gamecube adapter firmware uses 0x057E-0x337 as well as any clones that emulate it.
        private static UsbDevice _usbDevice;

        // These will allow us to read/write commands from the adapter after initializing the device.
        private static UsbEndpointReader _controllerReader;
        private static UsbEndpointWriter _controllerWriter;

        private static ViGEmClient[] _controllerClients;
        private static IXbox360Controller[] _XInputControllers;

        public static Adapter ControllerAdapter = new Adapter(); // Initialize a public instance of Adapter so we can use it all throughout.

        public static async Task Main(string[] args)
        {
            bool menuContinue = true;
            // Load application settings from file OR create if nonexistent
            ApplicationSettings.LoadSettings();

            // Continue taking input until menuContinue is not set
            while (menuContinue)
            {
                string input = PrintMenu();
                if (int.TryParse(input, out int numInput))
                {
                    // Clear menu after taking input
                    Console.Clear();
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
                    // cancel token if Enter is inputted
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
            // Init all relevant USB device interfaces so we can read from the controllers
            InitializeUSBDevice();

            foreach (int port in ApplicationSettings.PortsEnabled) // Do this for each controller port
            {
                Console.WriteLine(string.Format(Strings.MENU_CALIBRATION, port + 1));

                // create a cancellationToken so we can exit controller polling when input is received
                var cts = new CancellationTokenSource();
                var token = cts.Token;

                // poll controller asynchronously while waiting for command line input
                Task.Run(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        // Pass adapter data and gather min-max values for calibration at runtime
                        ControllerAdapter.UpdateAdapter(ReadBytes(37));
                        ControllerAdapter.Controllers[port].UpdateMinMax();
                    }
                }, token);

                // take user input from console 
                WaitForInput(cts);
                Console.WriteLine(Strings.MENU_CALIBRATION_COMPLETE);
            }

            // close USB interfaces cleanly
            CloseUSBDevice();
        }

        // Print main menu while returning commandline input
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
            ErrorCode ec = ErrorCode.None; // init error code
            try
            {
                // Init all relevant USB device interfaces so we can read from the controllers
                InitializeUSBDevice();
                Console.WriteLine(Strings.MENU_LOOP_BEGINNING);
                ec = ReadControllerData(ec); // Enter the controller read loop here and return any error codes if it fails
                Console.WriteLine(Strings.MENU_LOOP_COMPLETE);
            }

            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine((ec != ErrorCode.None ? ec + ":" : String.Empty) + ex.Message);
            }

            finally
            {
                CloseUSBDevice(); // We should actually never be able to reach this point
            }
        }

        public static void InitializeControllers()
        {
            try
            {
                // Create XInput joystick clients
                _controllerClients = new ViGEmClient[4];
                _XInputControllers = new IXbox360Controller[4];

                // If the port is enabled, create the XInput device 
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

        public static ErrorCode ReadControllerData(ErrorCode ec = ErrorCode.None)
        {
            InitializeControllers(); // Create controllers for updating
            byte[] rumbleData = new byte[] { 0x11, 0x00, 0x00, 0x00, 0x00 }; // Create rumble data command for later

            // Begin the controller loop as long as nothing errors
            while (ec == ErrorCode.None)
            {
                // read controller data from adapter interface and pass it to ControllerAdapter
                byte[] controllerData = ReadBytes(37, ec);
                ControllerAdapter.UpdateAdapter(controllerData);

                foreach (int port in ApplicationSettings.PortsEnabled)
                {
                    // Pass the xinput interface for the Controller class to update
                    ControllerAdapter.Controllers[port].UpdateController(_XInputControllers[port]);

                    // Print the raw data from the adapter if enabled
                    if (ApplicationSettings.EnableRawPrint)
                    {
                        PrintControllerInfo();
                    }

                    // If the rumble has changed, set the corresponding port to rumble in the rumble command
                    if (ControllerAdapter.Controllers[port].RumbleChanged && ApplicationSettings.EnableRumble)
                        rumbleData[port + 1] = 0x01;
                    else
                        rumbleData[port + 1] = 0x00;
                }

                // If enabled, print the raw hexadecimal string to the console so we can use it for debugging etc
                if (ApplicationSettings.EnableRawPrint)
                {
                    Console.Write(BitConverter.ToString(controllerData));
                    Console.WriteLine();
                }

                // send the rumble command to the adapter if enabled.
                if (ApplicationSettings.EnableRumble)
                    _controllerWriter.Write(rumbleData, 5000, out int transferLength);
            }

            // Return any error codes if it errors.
            return ec;
        }

        public static void PrintControllerInfo()
        {
            // Set the cursor so we can rewrite the console
            Console.SetCursorPosition(0, 1);
            Console.CursorVisible = false;

            foreach (int port in ApplicationSettings.PortsEnabled)
            {
                Console.WriteLine($"Port {port + 1}");
                Console.WriteLine(Strings.MENU_DIVIDER);

                Console.Write(string.Format(Strings.INFO_STICK,
                    ControllerAdapter.Controllers[port].LEFT_STICK_X.ToString(),
                    ControllerAdapter.Controllers[port].LEFT_STICK_Y.ToString()));
                Console.WriteLine();

                Console.Write(string.Format(Strings.INFO_CSTICK,
                        ControllerAdapter.Controllers[port].C_STICK_X.ToString(),
                        ControllerAdapter.Controllers[port].C_STICK_Y.ToString()));
                Console.WriteLine();

                Console.Write(string.Format(Strings.INFO_FACEBUTTONS,
                        ConvertYesNo(ControllerAdapter.Controllers[port].BUTTON_A),
                        ConvertYesNo(ControllerAdapter.Controllers[port].BUTTON_B),
                        ConvertYesNo(ControllerAdapter.Controllers[port].BUTTON_X),
                        ConvertYesNo(ControllerAdapter.Controllers[port].BUTTON_Y),
                        ConvertYesNo(ControllerAdapter.Controllers[port].BUTTON_START)));
                Console.WriteLine();

                Console.Write(string.Format(Strings.INFO_TRIGGERS,
                        ControllerAdapter.Controllers[port].ANALOG_LEFT.ToString(),
                        ControllerAdapter.Controllers[port].ANALOG_RIGHT.ToString(),
                        ConvertYesNo(ControllerAdapter.Controllers[port].BUTTON_Z)
                        ));
                Console.WriteLine();

                Console.Write(string.Format(Strings.INFO_DPAD,
                        ConvertYesNo(ControllerAdapter.Controllers[port].DPAD_LEFT),
                        ConvertYesNo(ControllerAdapter.Controllers[port].DPAD_RIGHT),
                        ConvertYesNo(ControllerAdapter.Controllers[port].DPAD_UP),
                        ConvertYesNo(ControllerAdapter.Controllers[port].DPAD_DOWN)));
                Console.WriteLine(); 
                Console.WriteLine();

                /* Rumble debug information
                Console.WriteLine(ConvertYesNo((ControllerAdapter.Controllers[port].RumbleChanged)));
                Console.WriteLine(ControllerAdapter.Controllers[port].LargeMotor);
                Console.WriteLine(ControllerAdapter.Controllers[port].SmallMotor);

                Console.WriteLine();
                */
            }
        }

        public static byte[] ReadBytes(int amount, ErrorCode ec = ErrorCode.None)
        {
            byte[] data = new byte[amount];
            int bytesRead = -1;
            ec = _controllerReader.Read(data, 5000, out bytesRead);

            // We ran out of bytes to read, throw an exception.
            if (bytesRead == 0)
                throw new Exception(string.Format(Strings.ERROR_NOBYTES, bytesRead));
            // Something else errored? Print the generic error code and throw an exception.
            else if (ec != ErrorCode.None)
                throw new Exception(string.Format(Strings.ERROR_GENERIC, ec));
            
            return data;
        }

        public static void InitializeUSBDevice()
        {
            // Use the USB device finder to find our Gamecube adapter
            _usbDevice = UsbDevice.OpenUsbDevice(_usbDeviceFinder);
            if (_usbDevice == null)
            {
                // If the gamecube adapter isn't plugged in OR is being used by another application, throw an exception.
                // Is there even a way to take control over the adapter from another device?
                throw new Exception(Strings.ERROR_ADAPTERNOTFOUND);
            }

            IUsbDevice wholeUsbDevice = _usbDevice as IUsbDevice;
            if (!ReferenceEquals(wholeUsbDevice, null))
            {
                wholeUsbDevice.SetConfiguration(1); // Set the adapter to use config one 
                wholeUsbDevice.ClaimInterface(0); // Claim interface zero
            }

            // Set reader to read from endpoint 1 (apparently GCN adapters use endpoint 0x81, but this works?)
            _controllerReader = _usbDevice.OpenEndpointReader(ReadEndpointID.Ep01);

            // Write to endpoint 2
            _controllerWriter = _usbDevice.OpenEndpointWriter(WriteEndpointID.Ep02);

            // begin polling command - https://gbatemp.net/threads/wii-u-gamecube-adapter-reverse-engineering-cont.388169/
            _controllerWriter.Write(Convert.ToByte(0x13), 5000, out int transferLength);
        }

        public static void CloseUSBDevice()
        {
            if (_usbDevice != null)
            {
                if (_usbDevice.IsOpen)
                {
                    IUsbDevice wholeUsbDevice = _usbDevice as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        // Close the USB interface if we still have it open.
                        wholeUsbDevice.ReleaseInterface(0);
                        Console.WriteLine(Strings.INFO_INTERFACERELEASE);
                    }

                    // Close the USB device 
                    _usbDevice.Close();
                }

                _usbDevice = null;
                UsbDevice.Exit();
            }
        }

        public static string ConvertYesNo(bool input)
        {
            return input ? "Yes" : "No";
        }
    }
}