using LibUsbDotNet;
using LibUsbDotNet.DeviceNotify;
using LibUsbDotNet.Main;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.ListView;

namespace DelfinovinUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int _VendorID = 0x057E;
        private const int _ProductID = 0x0337;

        private static UsbDevice _usbDevice;
        private static UsbDeviceFinder _usbDeviceFinder;

        private static UsbEndpointReader _controllerReader;
        private static UsbEndpointWriter _controllerWriter;

        private static GamecubeAdapter _gamecubeAdapter;
        private static AdapterStatus _AdapterStatus;
        private static byte[] _rumbleCommand;
        private static bool _IsCalibrating;

        public static IDeviceNotifier UsbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();
        private readonly SynchronizationContext _syncContext;

        public MainWindow()
        {
            InitializeComponent();

            _syncContext = SynchronizationContext.Current;
            UsbDeviceNotifier.OnDeviceNotify += (OnDeviceNotify);
            ctsDialog.btnApply.Click += BtnApply_Click;

            BeginControllerLoop();
        }

        private void BeginControllerLoop()
        {
            InitializeUSB();
            InitializeAdapter();

            if (_AdapterStatus == AdapterStatus.AdapterInitialized)
            {
                SetAdapterStatus();
                _controllerReader.DataReceived += (CtrlrDataReceived);
            }
        }

        private void InitializeUSB()
        {
            _usbDeviceFinder = new UsbDeviceFinder(_VendorID, _ProductID);
            _usbDevice = UsbDevice.OpenUsbDevice(_usbDeviceFinder);

            if (_usbDevice == null)
            {
                _AdapterStatus = AdapterStatus.AdapterDisconnected;
                return;
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
            _AdapterStatus = AdapterStatus.AdapterConnected;
            
        }

        private void DeinitializeUSB()
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

            // unsubscribe from event and close
            _controllerReader.DataReceived -= CtrlrDataReceived;

            _controllerReader.Dispose();
            _controllerWriter.Dispose();

            _AdapterStatus = AdapterStatus.AdapterDisconnected;
            SetAdapterStatus();
        }

        private void InitializeAdapter()
        {
            if (_AdapterStatus == AdapterStatus.AdapterConnected)
            {
                try
                {
                    _gamecubeAdapter = new GamecubeAdapter();
                    _rumbleCommand = new byte[] { 0x11, 0x00, 0x00, 0x00, 0x00 }; // set up rumble command

                    if (_AdapterStatus == AdapterStatus.AdapterConnected)
                        _controllerWriter.Write(new byte[] { 0x13 }, 5000, out int transferLength); // send start command
                }

                catch
                {
                    throw new Exception(Strings.ERROR_WRITEFAILED);
                }

                _AdapterStatus = AdapterStatus.AdapterInitialized;
                SetViGEmStatus(_gamecubeAdapter.ViGEmInstalled);
            }
        }

        private void SendRumble()
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

        private void SetViGEmStatus(bool installed)
        {
            if (installed)
            {
                this.lblViGEmStatus.Content = "🎮 ViGEmBus is installed";
                this.lblViGEmStatus.Foreground = Brushes.Green;
            }

            else
            {
                this.lblViGEmStatus.Content = "🎮 ViGEmBus is not installed";
                this.lblViGEmStatus.Foreground = Brushes.Red;
            }
        }

        private void SetAdapterStatus()
        {
            if (_AdapterStatus == AdapterStatus.AdapterInitialized)
            {
                this.lblAdapterStatus.Content = "🌎 Gamecube adapter connected";
                this.lblAdapterStatus.Foreground = Brushes.Green;
            }

            else
            {
                this.lblAdapterStatus.Content = "🌎 Gamecube adapter not connected";
                this.lblAdapterStatus.Foreground = Brushes.Red;
            }
        }

        private void CtrlrDataReceived(object sender, EndpointDataEventArgs e)
        {
            if (_AdapterStatus == AdapterStatus.AdapterInitialized)
            {
                _gamecubeAdapter.UpdateStates(e.Buffer);
                _gamecubeAdapter.UpdateControllers();

                if (_IsCalibrating)
                {
                    _gamecubeAdapter.UpdateCalibrations();
                }

                if (_gamecubeAdapter.RumbleChanged)
                    SendRumble();

                // Update UI from different thread
                _syncContext.Post(o =>
                {
                    _gamecubeAdapter.UpdateDialog(this.controllerDialog, lvwControllers.SelectedIndex);

                    if (_gamecubeAdapter.ControllerInserted)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            ListViewItem currentItem = ((ListViewItem)lvwControllers.Items[i]);
                            currentItem.IsEnabled = _gamecubeAdapter.Controllers[i].IsConnected;
                            currentItem.IsSelected = _gamecubeAdapter.Controllers[i].IsConnected;
                        }

                        _gamecubeAdapter.ControllerInserted = false;
                    }
                }, null);  
            }
        }

        private void OnDeviceNotify(object sender, DeviceNotifyEventArgs e)
        {
            if (e.Device.IdVendor == _VendorID && e.Device.IdProduct == 0x0337)
            {
                // Check if new device has been plugged in
                if (e.EventType == EventType.DeviceArrival)
                {
                    BeginControllerLoop();
                }

                // Check if adapter is unplugged
                else if (e.EventType == EventType.DeviceRemoveComplete)
                {
                    DeinitializeUSB();
                }
            }
        }

        private enum AdapterStatus
        {
            AdapterDisconnected,
            AdapterConnected,
            AdapterInitialized,
        }


        // Implement custom header bars
        private void rectHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Process.GetCurrentProcess().Kill();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void lviFAQHelp_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("testin!");

            
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            tnrSlides.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tnrSlides.SelectedIndex = 1;
        }

        private void lviSettings_Selected(object sender, RoutedEventArgs e)
        {
            controllerDialog.CStickOutline.LayoutTransform = new ScaleTransform(1.1, 1.1);
        }
    }
}
