using LibUsbDotNet;
using LibUsbDotNet.DeviceNotify;
using LibUsbDotNet.Main;
using MaterialDesignThemes.Wpf.Transitions;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
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
		private enum AdapterStatus
		{
			AdapterDisconnected,
			AdapterConnected,
			AdapterInitialized
		}

		private const int _VendorID = 0x57E;
		private const int _ProductID = 0x337;

		private readonly SynchronizationContext _syncContext;

		private UsbDevice _usbDevice;
		private UsbDeviceFinder _usbDeviceFinder;
		private IDeviceNotifier _usbDeviceNotifier;

		private UsbEndpointReader _controllerReader;
		private UsbEndpointWriter _controllerWriter;

		private GamecubeAdapter _gamecubeAdapter;
		private ControllerSettings[] _settings;
		private AdapterStatus _adapterStatus;

		private byte[] _rumbleCommand;
		private int _selectedPort = 0;
		private bool _isCalibrating;
		private bool _vigemInstalled;

		System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
		public MainWindow()
		{
			InitializeComponent();

			_usbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();
			_syncContext = SynchronizationContext.Current;
			_usbDeviceNotifier.OnDeviceNotify += OnDeviceNotify;
			ctsDialog.btnApply.Click += BtnApply_Click;
			ctsDialog.btnProfiles.Click += BtnProfiles_Click;

			BeginControllerLoop();
		}

        private void BeginControllerLoop()
		{
			GetDriverStatus();
			if (_vigemInstalled)
			{
				InitializeUSB();
				InitializeAdapter();

				_settings = new ControllerSettings[4];
				for (int i = 0; i < 4; i++)
					_settings[i] = new ControllerSettings();

				ApplicationSettings.LoadSettings();
				UpdateDefaultProfiles();


				if (_adapterStatus == AdapterStatus.AdapterInitialized)
				{
					_controllerReader.DataReceived += CtrlrDataReceived;
				}
			}

			else
            {
				MessageWindow messageWindow = new MessageWindow("ViGEmBus is not installed. " +
					"ViGEm is required to use Delfinovin." +
					"\n\nWould you like to open the ViGEm downloads page?", true, true, "Yes!", "No.");
				messageWindow.ShowDialog();

				if (messageWindow.Result != WindowResult.OK)
					return;

				Process.Start("https://github.com/ViGEm/ViGEmBus/releases");
			}
		}

		private void InitializeUSB()
		{
			_usbDeviceFinder = new UsbDeviceFinder(_VendorID, _ProductID);
			_usbDevice = UsbDevice.OpenUsbDevice(_usbDeviceFinder);
			if (_usbDevice == null)
			{
				_adapterStatus = AdapterStatus.AdapterDisconnected;
				return;
			}

			IUsbDevice wholeUsbDevice = _usbDeviceFinder as IUsbDevice;
			if (wholeUsbDevice != null)
			{
				wholeUsbDevice.SetConfiguration(1);
				wholeUsbDevice.ClaimInterface(0);
			}

			_controllerReader = _usbDevice.OpenEndpointReader(ReadEndpointID.Ep01, 37);
			_controllerReader.ReadThreadPriority = ThreadPriority.Highest;
			_controllerReader.DataReceivedEnabled = true;

			_controllerWriter = _usbDevice.OpenEndpointWriter(WriteEndpointID.Ep02);
			_adapterStatus = AdapterStatus.AdapterConnected;
		}

		private void DeinitializeUSB()
		{
			if (_usbDevice != null && _usbDevice.IsOpen)
			{
				(_usbDevice as IUsbDevice)?.ReleaseInterface(0);
				_usbDevice.Close();
			}

			_usbDevice = null;
			UsbDevice.Exit();

			if (_controllerReader != null)
            {
				_controllerReader.DataReceived -= CtrlrDataReceived;
				_controllerReader.Dispose();
				_controllerWriter.Dispose();
			}

			_adapterStatus = AdapterStatus.AdapterDisconnected;
			SetAdapterStatus();
		}

		private void InitializeAdapter()
		{
			if (_adapterStatus != AdapterStatus.AdapterConnected)
				return;

			try
			{
				byte[] buffer = new byte[256];
				UsbSetupPacket packet = new UsbSetupPacket(0x21, 11, 0x0001, 0, 0); // apparently this fixes support with Nyko/third party adapters
				_usbDevice.ControlTransfer(ref packet, buffer, buffer.Length, out int lengthTranfered);

				_gamecubeAdapter = new GamecubeAdapter();
				_rumbleCommand = new byte[5] { 0x11, 0x00, 0x00, 0x00, 0x00 };
				if (_adapterStatus == AdapterStatus.AdapterConnected)
					_controllerWriter.Write(new byte[1] { 0x13 }, 5000, out int transferLength);
			}

			catch
			{
				throw new Exception("Error! Write failed!");
			}

			_adapterStatus = AdapterStatus.AdapterInitialized;
			SetAdapterStatus();
		}

		private void GetDriverStatus()
		{
			try
			{
				ViGEmClient client = new ViGEmClient();
				_vigemInstalled = true;
				client.Dispose();
				
			}

			catch (VigemBusNotFoundException)
			{
				_vigemInstalled = false;
			}

			SetViGEmStatus(_vigemInstalled);
		}

		private void SetViGEmStatus(bool installed)
		{
			lblViGEmStatus.Content = (installed ? "🎮 ViGEmBus is installed" : "🎮 ViGEmBus is not installed");
			lblViGEmStatus.Foreground = (installed ? Brushes.Green : Brushes.Red);
		}

		private void SetAdapterStatus()
		{
			if (_adapterStatus == AdapterStatus.AdapterInitialized)
			{
				lblAdapterStatus.Content = "🌎 Gamecube adapter connected";
				lblAdapterStatus.Foreground = Brushes.Green;
			}
			else
			{
				lblAdapterStatus.Content = "🌎 Gamecube adapter not connected";
				lblAdapterStatus.Foreground = Brushes.Red;
			}
		}

		private void SendRumbleStatus()
		{
			for (int i = 0; i < 4; i++)
			{
				byte currentRumbleState = Extensions.BoolToByte(_gamecubeAdapter.Controllers[i].RumbleChanged && _settings[i].EnableRumble);
				_rumbleCommand[i + 1] = currentRumbleState;
			}

			if (_controllerWriter.Write(_rumbleCommand, 5000, out int transferLength) != 0)
			{
				throw new Exception("Error! Write failed!");
			}
		}

		public void UpdateDefaultProfiles()
		{
			bool[] settingLoaded = new bool[4];
			settingLoaded[0] = _settings[0].LoadFromFile($"profiles\\{ApplicationSettings.DefaultProfile1}.txt");
			settingLoaded[1] = _settings[1].LoadFromFile($"profiles\\{ApplicationSettings.DefaultProfile2}.txt");
			settingLoaded[2] = _settings[2].LoadFromFile($"profiles\\{ApplicationSettings.DefaultProfile3}.txt");
			settingLoaded[3] = _settings[3].LoadFromFile($"profiles\\{ApplicationSettings.DefaultProfile4}.txt");

			for (int i = 0; i < 4; i++)
            {
				if (settingLoaded[i])
					_gamecubeAdapter.UpdateSettings(_settings[i], i); // Do this so we don't load falsely calibrated profiles
			}
		}

		private void ExitProgram()
		{
			ni.Dispose();
			Environment.Exit(0);
		}

		private void CtrlrDataReceived(object sender, EndpointDataEventArgs e)
		{
			if (_adapterStatus != AdapterStatus.AdapterInitialized)
				return;

			_gamecubeAdapter.UpdateStates(e.Buffer);
			_gamecubeAdapter.UpdateControllers();

			if (_gamecubeAdapter.RumbleChanged)
				SendRumbleStatus();

			_syncContext.Post(delegate // Update UI from different thread
			{
				_gamecubeAdapter.UpdateDialog(controllerDialog, _selectedPort);
				if (_gamecubeAdapter.ControllerInserted)
				{
					for (int i = 0; i < 4; i++)
					{
						ListViewItem listViewItem = (ListViewItem)lvwControllers.Items[i];
						listViewItem.IsEnabled = _gamecubeAdapter.Controllers[i].IsConnected;
						listViewItem.IsSelected = _gamecubeAdapter.Controllers[i].IsConnected;

						Button editButton = (Button)lvwEdits.Items[i];
						editButton.IsEnabled = _gamecubeAdapter.Controllers[i].IsConnected;
					}

					UpdateDefaultProfiles();
					_gamecubeAdapter.ControllerInserted = false;
				}
				
			}, null);
		}

		private void OnDeviceNotify(object sender, DeviceNotifyEventArgs e)
		{
			if (e.Device.IdVendor == _VendorID && e.Device.IdProduct == _ProductID)
			{
				if (e.EventType == EventType.DeviceArrival)
					BeginControllerLoop();

				else if (e.EventType == EventType.DeviceRemoveComplete)
					DeinitializeUSB();
			}
		}

		private void cmiEditSettings_Click(object sender, RoutedEventArgs e)
		{
			tnrSlides.SelectedIndex = 1;
			ctsDialog.UpdateControl(_settings[lvwControllers.SelectedIndex]);
		}

		private void cmiCalibrate_Click(object sender, RoutedEventArgs e)
		{
			_isCalibrating = !_isCalibrating;
			if (_isCalibrating) // Reset calibration so new values take precedence 
            {
				_gamecubeAdapter.Controllers[_selectedPort].Calibration.ResetCalibration();
				_gamecubeAdapter.Controllers[_selectedPort].CalibrationStatus = CalibrationStatus.Calibrating;
			}

			lblOtherInfo.Content = (_isCalibrating ? $"Calibrating Controller {_selectedPort + 1}... Make sure to finalize the calibration." : "");
			ContextMenu menu = Resources["ctmControllerSettings"] as ContextMenu;
			MenuItem menuItem = menu.Items[1] as MenuItem;
			menuItem.Header = ((!_isCalibrating) ? "Calibrate Controller" : "Finish Calibrating");

			if (!_isCalibrating)
			{
				_settings[_selectedPort].LeftStickRange = _gamecubeAdapter.Controllers[_selectedPort].Calibration.GetRange()[0];
				_settings[_selectedPort].RightStickRange = _gamecubeAdapter.Controllers[_selectedPort].Calibration.GetRange()[1];

				ctsDialog.leftStickRange.Value = _settings[_selectedPort].LeftStickRange * 100f; // Scale values to slider values
				ctsDialog.rightStickRange.Value = _settings[_selectedPort].RightStickRange * 100f;

				_gamecubeAdapter.UpdateSettings(_settings[_selectedPort], _selectedPort); // update controller settings
				_gamecubeAdapter.Controllers[_selectedPort].CalibrationStatus = CalibrationStatus.Calibrated;
				lblOtherInfo.Content = "Finshed calibrating controller.";
			}
		}

		private void BtnProfiles_Click(object sender, RoutedEventArgs e)
		{
			ProfileDialog profileDialog = new ProfileDialog(ctsDialog.GetSettings());
			profileDialog.ShowDialog();

			if (profileDialog.WindowResult == WindowResult.SaveClosed)
			{
				ControllerSettings controllerProfile = profileDialog.SelectedProfile;
				ctsDialog.UpdateControl(controllerProfile);
			}
		}
		
		private void BtnApply_Click(object sender, RoutedEventArgs e)
		{
			_settings[_selectedPort] = ctsDialog.GetSettings(); 
			_gamecubeAdapter.UpdateSettings(_settings[_selectedPort], _selectedPort);
			lblOtherInfo.Content = $"Applied settings on Controller {_selectedPort + 1}!";

			tnrSlides.SelectedIndex = 0; // Transitions to blank page
		}

		// Implement custom header bars
		private void rectHeader_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				this.DragMove();
		}

		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			ExitProgram();
		}

		private void CloseMenu_Click(object sender, EventArgs e)
		{
			ExitProgram();
		}

		private void btnMinimize_Click(object sender, RoutedEventArgs e)
		{
			if (ApplicationSettings.MinimizeToTray)
			{
				ni.Icon = Properties.Resources.trayMinimizeIcon;

				ni.ContextMenu = new System.Windows.Forms.ContextMenu();
				var openMenu = new System.Windows.Forms.MenuItem();
				openMenu.Click += OpenMenu_Click;
				openMenu.Text = "Open";

				var closeMenu = new System.Windows.Forms.MenuItem();
				closeMenu.Click += CloseMenu_Click;
				closeMenu.Text = "Exit";

				ni.ContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
				{
					openMenu,
					closeMenu
				});

				ni.Visible = true;
			}
			WindowState = WindowState.Minimized;
		}

        private void OpenMenu_Click(object sender, EventArgs e)
        {
			this.Show();
			base.WindowState = WindowState.Normal;
		}

        protected override void OnStateChanged(EventArgs e)
		{
			if (WindowState == WindowState.Minimized && ApplicationSettings.MinimizeToTray)
				this.Hide();
		}

		private void lviSettings_MouseUp(object sender, MouseButtonEventArgs e)
		{
			AppSettingDialog appSettingDialog = new AppSettingDialog();
			appSettingDialog.ShowDialog();

			if (appSettingDialog.Result == WindowResult.SaveClosed)
			{
				UpdateDefaultProfiles();
				ctsDialog.UpdateControl(_settings[_selectedPort]);
			}
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
			ContextMenu cm = this.FindResource("ctmControllerSettings") as ContextMenu;
			cm.PlacementTarget = sender as Button;
			cm.IsOpen = true;
		}

        private void lvwControllers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			_selectedPort = lvwControllers.SelectedIndex;
			if (_settings != null && _selectedPort != -1)
				ctsDialog.UpdateControl(_settings[_selectedPort]);
        }
    }
}
