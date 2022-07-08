using LibUsbDotNet;
using LibUsbDotNet.DeviceNotify;
using LibUsbDotNet.Main;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


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

		private bool _isCalibrating;
		private bool _vigemInstalled;

		public int _selectedPort = 0;

		private WindowedIcon[] icons;

		System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
		public MainWindow()
		{
			InitializeComponent();
			_usbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();

			// This is used later for updating the UI from a different thread
			_syncContext = SynchronizationContext.Current;

			// Set up events for detecting USB insertions 
			_usbDeviceNotifier.OnDeviceNotify += OnDeviceNotify; 

			ctsDialog.btnApply.Click += BtnApply_Click;
			ctsDialog.btnProfiles.Click += BtnProfiles_Click;

			GetApplicationSettings();
			BeginControllerLoop();
		}

		private void GetApplicationSettings()
        {
			// Load the application settings from the settings file
			ApplicationSettings.LoadSettings();

			// Check for updates, if set.
			if (ApplicationSettings.CheckForUpdates)
				Updater.CheckCurrentRelease(true); // Pass true to disable the response window

			// Minimize the window on application startup, if set.
			if (ApplicationSettings.MinimizeOnStartup)
            {
				if (ApplicationSettings.MinimizeToTray)
					CreateNotifyIcon();
				WindowState = WindowState.Minimized;
			}

			// Update the application theme and color,
			// if set.
			UpdateApplicationTheme();
		}

		private void UpdateApplicationTheme()
        {
			// Check to see if the application theme is set.
			if (ApplicationSettings.ApplicationTheme != "")
			{
				string[] themes = Extensions.GetResourcesUnder("Themes");

				// Check if the selected application theme is in the valid theme list
				if (Array.IndexOf(themes, (ApplicationSettings.ApplicationTheme + ".baml").ToLower()) >= 0)
                {
					// Update the application-wide theme with the selected one.
					Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
					{
						Source = new Uri($"/DelfinovinUI;component/Themes/{ApplicationSettings.ApplicationTheme}.xaml", UriKind.Relative)
					};
				}
			}

			if (Enum.TryParse(ApplicationSettings.ControllerColor, out ThemeSelector.ControllerColor controllerColor))
			{
				// Case the controllerColor into an uint
				uint hexCode = (uint)controllerColor;
				Color convertedColor = Extensions.GetColorFromHex(hexCode);

				// Set the resource "ControllerColor" to with the new color 
				App.Current.Resources["ControllerColor"] = new SolidColorBrush(convertedColor);
			}
		}

        private void BeginControllerLoop()
		{
			GetDriverStatus();
			if (_vigemInstalled)
			{
				InitializeUSB();
				InitializeAdapter();

				// Initialize controller profile array for later and load the 
				// user selected ones, if available.
				_settings = new ControllerSettings[4];
				icons = new WindowedIcon[4];
				for (int i = 0; i < 4; i++)
                {
					_settings[i] = new ControllerSettings();
					icons[i] = new WindowedIcon();

				}
					

				UpdateDefaultProfiles();

				// Only begin the loop if the adapter is initalized.
				if (_adapterStatus == AdapterStatus.AdapterInitialized)
				{
					_controllerReader.DataReceived += CtrlrDataReceived;
				}
			}

			else
            {
				// If it's not installed, propmt the user to download and install it.
				string message = "ViGEmBus is not installed. ViGEm is required to use Delfinovin." +
					"\n\nWould you like to open the ViGEm downloads page?";

				MessageWindow messageWindow = new MessageWindow(message, true, true, "Yes!", "No.");
				messageWindow.ShowDialog();

				if (messageWindow.Result != WindowResult.OK)
					return;

				// Open the ViGEmBus releases page in the default browser.
				Process.Start("https://github.com/ViGEm/ViGEmBus/releases");
			}
		}

		private void GetDriverStatus()
		{
			try
			{
				// Create a client to check if ViGEmBus is installed.
				ViGEmClient client = new ViGEmClient();
				_vigemInstalled = true;

				// Dispose of the client after we're done.
				client.Dispose(); 
			}

			// Catch the exception. Allow users to keep using the program but disable all controllers
			catch (VigemBusNotFoundException) 
			{
				_vigemInstalled = false;
			}

			// Set UI status
			SetViGEmStatus(_vigemInstalled);
		}


		private void InitializeUSB()
		{
			// Search for the adapter. If it's not found set the status to "Disconnected."
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

			// Set up event reads here and use the highest priority.
			_controllerReader = _usbDevice.OpenEndpointReader(ReadEndpointID.Ep01, 37);
			_controllerReader.ReadThreadPriority = ThreadPriority.Highest;
			_controllerReader.DataReceivedEnabled = true;

			_controllerWriter = _usbDevice.OpenEndpointWriter(WriteEndpointID.Ep02);
			_adapterStatus = AdapterStatus.AdapterConnected;
		}

		private void InitializeAdapter()
		{
			// Only continue if the adapter is inserted.
			if (_adapterStatus != AdapterStatus.AdapterConnected)
				return;

			try
			{
				// this fixes support with Nyko/third party adapters don't ask me how
				byte[] buffer = new byte[256];
				UsbSetupPacket packet = new UsbSetupPacket(0x21, 11, 0x0001, 0, 0); 
				_usbDevice.ControlTransfer(ref packet, buffer, buffer.Length, out int lengthTranfered);

				// Create a GamecubeAdapter device
				_gamecubeAdapter = new GamecubeAdapter();

				// Set up rumble commands for later
				_rumbleCommand = new byte[5] { 0x11, 0x00, 0x00, 0x00, 0x00 };
				if (_adapterStatus == AdapterStatus.AdapterConnected)
					// Prompt the USB device to start sending data
					_controllerWriter.Write(new byte[1] { 0x13 }, 5000, out int transferLength);
			}

			catch
			{
				throw new Exception("Error! Write failed!");
			}

			// All of the USB stuff is set up, set status to "Initialized." 
			_adapterStatus = AdapterStatus.AdapterInitialized;

			// Update the UI status
			SetAdapterStatus(); 
		}


		private void DeinitializeUSB()
		{
			// If the device is still in use, release the interface.
			if (_usbDevice != null && _usbDevice.IsOpen)
			{
				(_usbDevice as IUsbDevice)?.ReleaseInterface(0);
				_usbDevice.Close();
			}

			// Reset usbDevice
			_usbDevice = null;
			UsbDevice.Exit();

			if (_controllerReader != null)
            {
				// Unsubscribe from events + dispose of the readers.
				_controllerReader.DataReceived -= CtrlrDataReceived;
				_controllerReader.Dispose();
				_controllerWriter.Dispose();
			}

			// Disable all controllers if unplugged.
			for (int i = 0; i < 4; i++)
            {
				ListViewItem item = (ListViewItem)lvwControllers.Items[i];
				item.IsEnabled = false;
				item.IsSelected = false;

				Button editButton = (Button)lvwEdits.Items[i];
				editButton.IsEnabled = false;
			}

			// Set adapter status and update UI
			_adapterStatus = AdapterStatus.AdapterDisconnected;
			SetAdapterStatus();
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
				// Check rumble is enabled in the controller settings + the controller sent rumble
				// and update the rumble command. Increment by one to skip past the command byte
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
			// Check if the settings were actually loaded.
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

		private void CreateNotifyIcon()
		{
			notifyIcon.Icon = Properties.Resources.trayMinimizeIcon;
			notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();

			// Create menu items and subscribe to click event
			var openMenu = new System.Windows.Forms.MenuItem();
			openMenu.Click += OpenMenu_Click;
			openMenu.Text = "Open";

			var closeMenu = new System.Windows.Forms.MenuItem();
			closeMenu.Click += CloseMenu_Click;
			closeMenu.Text = "Exit";

			// Add them to the contextMenu 
			notifyIcon.ContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
			{
					openMenu,
					closeMenu
			});

			// Set the system tray icon visible
			notifyIcon.Visible = true;
		}

		private void ExitProgram()
		{
			// This is so we can close the notifyIcon properly
			// after application close.

			// I have no idea why you have to dispose of the Icon
			// and the notifyIcon object.
			notifyIcon.Visible = false;

			if (notifyIcon.Icon != null)
				notifyIcon.Icon.Dispose(); 

			notifyIcon.Dispose();

			// I couldn't find a better way to close the WPF application
			// without leaving the process behind.
			Environment.Exit(0);
		}

		private void CtrlrDataReceived(object sender, EndpointDataEventArgs e)
		{
			// Send the adapter data so that we can update the input states
			_gamecubeAdapter.UpdateStates(e.Buffer);

			// Send the reports to ViGEmBus and update the XInput devices
			_gamecubeAdapter.UpdateControllers();

			// If any of the rumble states changed for any controller
			// Update the rumble command array
			if (_gamecubeAdapter.RumbleChanged)
				SendRumbleStatus();

			// Update UI from different thread
			_syncContext.Post(delegate 
			{
				// Pass the control to GamecubeAdapter
				_gamecubeAdapter.UpdateDialog(controllerDialog, _selectedPort);
				_gamecubeAdapter.UpdateDialog(icons[_selectedPort].gamecubeDialog, _selectedPort);

				// If a new controller is inserted, enable the controllers + buttons 
				if (_gamecubeAdapter.ControllerInserted || _gamecubeAdapter.ControllerDisconnected)
				{
					for (int i = 0; i < 4; i++)
					{
						// Cast the items to a ListViewItem/Button so we can change the properties
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
			// Check if the plugged device is a Gamecube Adapter.
			if (e.Device.IdVendor == _VendorID && e.Device.IdProduct == _ProductID)
			{
				// On arrival, begin the controller loop
				if (e.EventType == EventType.DeviceArrival)
					BeginControllerLoop();

				// On unplug, deinit all controllers
				else if (e.EventType == EventType.DeviceRemoveComplete)
					DeinitializeUSB();
			}
		}

		private void cmiEditSettings_Click(object sender, RoutedEventArgs e)
		{
			// Swap to the controller settings slide 
			tnrSlides.SelectedIndex = 1;

			// Update the control based on the settings currently loaded.
			ctsDialog.UpdateControl(_settings[lvwControllers.SelectedIndex]);
		}

		private void cmiCalibrate_Click(object sender, RoutedEventArgs e)
		{
			_isCalibrating = !_isCalibrating;

			// Reset the calibration so new values take precedence 
			if (_isCalibrating) 
            {
				_gamecubeAdapter.Controllers[_selectedPort].Calibration.ResetCalibration();
				_gamecubeAdapter.Controllers[_selectedPort].CalibrationStatus = CalibrationStatus.Calibrating;
			}

			// Update the UI 
			lblOtherInfo.Content = (_isCalibrating ? $"Calibrating Controller {_selectedPort + 1}..." : "");

			// Update the context menu based on the current status.
			ContextMenu menu = Resources["ctmControllerSettings"] as ContextMenu;
			MenuItem menuItem = menu.Items[1] as MenuItem;
			menuItem.Header = ((!_isCalibrating) ? "Calibrate Controller" : "Finish Calibrating");

			if (!_isCalibrating)
			{
				// Update the controller profile based on the calibration
				_settings[_selectedPort].LeftStickRange = _gamecubeAdapter.Controllers[_selectedPort].Calibration.GetRange()[0];
				_settings[_selectedPort].RightStickRange = _gamecubeAdapter.Controllers[_selectedPort].Calibration.GetRange()[1];

				// Scale values to slider values
				ctsDialog.leftStickRange.Value = _settings[_selectedPort].LeftStickRange * 100f; 
				ctsDialog.rightStickRange.Value = _settings[_selectedPort].RightStickRange * 100f;

				// Update controller settings and set the status to "Calibrated."
				_gamecubeAdapter.UpdateSettings(_settings[_selectedPort], _selectedPort); 
				_gamecubeAdapter.Controllers[_selectedPort].CalibrationStatus = CalibrationStatus.Calibrated;

				// Update the UI
				lblOtherInfo.Content = "Finshed calibrating controller.";
			}
		}

		private void BtnProfiles_Click(object sender, RoutedEventArgs e)
		{
			// Open the profileDialog and allow the user to create/choose a profile
			ProfileDialog profileDialog = new ProfileDialog(ctsDialog.GetSettings());
			profileDialog.ShowDialog();

			// If the user saved / loaded a profile, get the returned profile and update the control
			if (profileDialog.WindowResult == WindowResult.SaveClosed)
			{
				ControllerSettings controllerProfile = profileDialog.SelectedProfile;

				// Update the controller settings dialog with the new settings.
				ctsDialog.UpdateControl(controllerProfile);
			}
		}
		
		private void BtnApply_Click(object sender, RoutedEventArgs e)
		{
			// Gather the settings from the controller dialog and apply it
			_settings[_selectedPort] = ctsDialog.GetSettings(); 
			_gamecubeAdapter.UpdateSettings(_settings[_selectedPort], _selectedPort);

			// Update the UI
			lblOtherInfo.Content = $"Applied settings on Controller {_selectedPort + 1}!";

			// Transitions to blank page
			tnrSlides.SelectedIndex = 0; 
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
				CreateNotifyIcon();

			// Minimize the window
			WindowState = WindowState.Minimized;
		}

        private void OpenMenu_Click(object sender, EventArgs e)
        {
			// Show the window and set the status back to normal
			this.Show();
			base.WindowState = WindowState.Normal;

			// Hide the notifyIcon
			notifyIcon.Visible = false;
		}

        protected override void OnStateChanged(EventArgs e)
		{
			// Only apply this if Minimize to tray is enabled
			if (WindowState == WindowState.Minimized && ApplicationSettings.MinimizeToTray)
				this.Hide();
		}

		private void lviSettings_MouseUp(object sender, MouseButtonEventArgs e)
		{
			AppSettingDialog appSettingDialog = new AppSettingDialog();
			appSettingDialog.ShowDialog();

			if (appSettingDialog.Result == WindowResult.SaveClosed)
			{
				// If the user saved the settings, update the default profiles
				// and update the controller settings UI
				UpdateDefaultProfiles();
				ctsDialog.UpdateControl(_settings[_selectedPort]);
			}
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
			// Find the context menu and place it where the button was clicked
			ContextMenu cm = this.FindResource("ctmControllerSettings") as ContextMenu;
			cm.PlacementTarget = sender as Button;
			cm.IsOpen = true;
		}

        private void lvwControllers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			// Update the currently selected controller port
			if (_settings != null && lvwControllers.SelectedIndex != -1)
            {
				_selectedPort = lvwControllers.SelectedIndex;
				ctsDialog.UpdateControl(_settings[_selectedPort]);
			}
        }

        private void lviFAQHelp_MouseUp(object sender, MouseButtonEventArgs e)
        {
			Process.Start("https://github.com/Struggleton/Delfinovin/blob/wpf-uidev/Delfinovin Help & FAQ.pdf");
        }

        private void cmiPopout_Click(object sender, RoutedEventArgs e)
        {
			icons[_selectedPort] = new WindowedIcon();
			icons[_selectedPort].Show();
		}
    }
}
