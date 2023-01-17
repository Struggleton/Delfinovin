using Delfinovin.Controllers;
using Delfinovin.Controls;
using Delfinovin.Controls.Views;
using Delfinovin.Controls.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Forms = System.Windows.Forms;
using UserSettings = Delfinovin.Properties.Settings;

namespace Delfinovin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string APP_ICON_PATH = "/Delfinovin;component/Resources/Icons/app.ico";
        private const string FAQ_LINK = "https://github.com/Struggleton/delfinovin-docs/wiki";
        private int _selectedControllerPort = 0;
        
        private GamecubeDialog _controllerDialog = new GamecubeDialog();
        private GamecubeAdapter _gamecubeAdapter = new GamecubeAdapter();
        private InputPlaybackView _inputPlaybackView;
        private ProfilesListView _profileListView;
        private Forms.NotifyIcon _notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            InitializeWindow();
            CheckForUpdates();

            _gamecubeAdapter.ControllerConnectionStatusChanged += ControllerConnectionStatusChanged;
            _gamecubeAdapter.InputFrameProcessed += InputFrameProcessed;
            _gamecubeAdapter.CalibrationStatusChanged += CalibrationStatusChanged;
            _gamecubeAdapter.BeginPlaybackEventRequested += BeginPlaybackEventRequested;
            _gamecubeAdapter.StopPlaybackEventRequested += StopPlaybackEventRequested;
            _gamecubeAdapter.StopRecordingInputEventRequested += StopRecordingInputEventRequested;
            _gamecubeAdapter.StartRecordingInputEventRequested += StartRecordingInputEventRequested;
            _gamecubeAdapter.Start();


            _inputPlaybackView = new InputPlaybackView(ref _gamecubeAdapter);
            _profileListView = new ProfilesListView();
        }

        private void CheckForUpdates()
        {
            if (UserSettings.Default.CheckForUpdates)
                // Pass true to disable the response window
                Updater.CheckForUpdates(true);
        }

        private void InitializeWindow()
        {
            InitializeNotifyIcon();
            SetApplicationTitle();
            CreateDetailButtons();
            SetDefaultView();

            if (UserSettings.Default.MinimizeOnStartup)
                this.WindowState = WindowState.Minimized;
        }

        private void InitializeNotifyIcon()
        {
            // Create a new NotifyIcon
            _notifyIcon = new Forms.NotifyIcon();

            // Set its hover text to the main window title
            _notifyIcon.Text = Strings.HeaderMainWindowTitle;

            // Get the application icon stream
            Stream iconStream = Application.GetResourceStream(new Uri(APP_ICON_PATH, UriKind.Relative)).Stream;

            // Set the notify icon to the application icon
            _notifyIcon.Icon = new System.Drawing.Icon(iconStream);

            // Create a new menu strip
            Forms.ContextMenuStrip notifyIconStrip = new Forms.ContextMenuStrip();

            // Add Open/Close options and subscribe to the events
            notifyIconStrip.Items.Add(Strings.Open, null, OnNotifyIconOpenClick);
            notifyIconStrip.Items.Add(Strings.Close, null, OnNotifyIconCloseClick);

            // Apply the menu strip to the notify icon
            _notifyIcon.ContextMenuStrip = notifyIconStrip;
        }

        private void SetApplicationTitle()
        {
            // Get the version of this assembly
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;

            // Update the header text with the current version
            mainWindowTitle.HeaderText = Strings.HeaderMainWindowTitle + " " + string.Format(Strings.HeaderVersionText, 
                appVersion.Major, 
                appVersion.Minor, 
                appVersion.Build);
        }

        private void CreateDetailButtons()
        {
            // Create four detailButtons and initialize
            // them with default values
            for (int i = 0; i < 4; i++)
            {
                ControllerDetailButton detailButton = new ControllerDetailButton()
                {
                    CalibrationStatus = CalibrationStatus.Uncalibrated,
                    ConnectionStatus = ConnectionStatus.Disconnected,

                    // Do this so it appears we are starting
                    // from Port 1 instead of 0
                    ControllerPort = i + 1,
                };

                // Subscribe to events 
                detailButton.Clicked += ControllerButton_Clicked;
                detailButton.OptionSelected += ControllerButton_OptionSelected;

                // Add the detail button the controllerDetailsList
                controllerList.Children.Add(detailButton);
            }
        }

        private void SetDefaultView()
        {
            // Check if the setting to disable donation view on start
            // is set. If it is, send the user directly to the home view.
            bool donateHidden = UserSettings.Default.HideDonationOnStartup;
            NavigationSelection navigationSelection = donateHidden ? NavigationSelection.Home :
                                                                     NavigationSelection.DonationSupport;

            SetCurrentView(navigationSelection);
        }

        private void SetCurrentView(NavigationSelection navigationSelection)
        {
            // Set the current view and update the content of the
            // transitioner with the content of whatever we need to display

            // Adjust the margins of the Viewbox as needed
            if (navigationSelection == NavigationSelection.Home)
            {
                viewContainer.Margin = new Thickness(0, 40, 10, 20);
                viewDisplay.Content = _controllerDialog;
                viewDisplay.OnApplyTemplate();
            }

            else if (navigationSelection == NavigationSelection.DonationSupport)
            {
                DonateSupportView donateSupportView = new DonateSupportView();
                viewContainer.Margin = new Thickness(0, 40, 10, 20);
                viewDisplay.Content = donateSupportView;
                viewDisplay.OnApplyTemplate();
            }

            else if (navigationSelection == NavigationSelection.Profiles)
            {
                _profileListView = new ProfilesListView();

                // When bringing the ProfileListView into view, subscribe
                // to the click event so we can get any profiles being set
                _profileListView.SelectProfileButtonClicked += ProfileListView_SelectProfileButtonClicked;

                viewContainer.Margin = new Thickness(0, 0, 0, 0);
                viewDisplay.Content = _profileListView;
                viewDisplay.OnApplyTemplate();
            }

            else if (navigationSelection == NavigationSelection.FAQ)
            {
                Process.Start(new ProcessStartInfo(FAQ_LINK)
                {
                    UseShellExecute = true
                });
            }

            else if (navigationSelection == NavigationSelection.PlaybackRecording)
            {
                viewContainer.Margin = new Thickness(0, 0, 0, 0);
                viewDisplay.Content = _inputPlaybackView;
                viewDisplay.OnApplyTemplate();
            }

            else if (navigationSelection == NavigationSelection.Settings)
            {
                ApplicationSettingsMenu applicationSettingsMenu = new ApplicationSettingsMenu();
                applicationSettingsMenu.ShowDialog();
            }
        }


        protected override void OnStateChanged(EventArgs e)
        {
            // If the window is minimized and we set the window to minimize to the tray
            // Hide the icon and make the notifyIcon visible.
            if (WindowState == WindowState.Minimized && UserSettings.Default.MinimizeToTray)
            {
                this.Hide();
                _notifyIcon.Visible = true;
            }

            base.OnStateChanged(e);
        }

        private void OnNotifyIconOpenClick(object? sender, EventArgs e)
        {
            // Show our window and set the window state to normal.
            this.Show();
            base.WindowState = WindowState.Normal;

            // Hide the notifyIcon
            _notifyIcon.Visible = false;
        }

        private void OnNotifyIconCloseClick(object? sender, EventArgs e)
        {
            // Close the application.
            this.Close();
        }

        private void InputFrameProcessed(object? sender, ControllerStatus[] inputs)
        {
            // Update the controller dialog with the inputs
            // from the currently selected controller.
            this.Dispatcher.Invoke(() =>
            {
                _controllerDialog.UpdateDialog(inputs[_selectedControllerPort]);
            });
        }

        private void CalibrationStatusChanged(object? sender, CalibrationChangedEventArgs e)
        {
            // Get the calibration status from the EventArgs
            // and update the detail button with the new status.
            this.Dispatcher.Invoke(() =>
            {
                // Get the detailButton from the controllerDetailsList
                ControllerDetailButton detailButton = (ControllerDetailButton)controllerList.Children[e.ControllerPort];

                // Update the calibration status
                detailButton.CalibrationStatus = e.CalibrationStatus;
            });
        }

        private void ControllerConnectionStatusChanged(object? sender, ControllerConnectionStatusEventArgs e)
        {
            // Update the controller detail button with the EventArgs
            this.Dispatcher.Invoke(() =>
            {
                // Get the controllerDetailButton from the stackPanel controls 
                ControllerDetailButton detailButton = (ControllerDetailButton)controllerList.Children[e.ControllerPort];
                detailButton.ConnectionStatus = e.ConnectionStatus;
                
                // If the controller is newly connected,
                // select it as our current controller.
                if (e.ConnectionStatus == ConnectionStatus.Connected)
                {
                    _selectedControllerPort = e.ControllerPort;
                    if (ProfileManager.LoadDefaultProfile(_selectedControllerPort))
                        _gamecubeAdapter.UpdateCalibration(_selectedControllerPort, CalibrationStatus.Calibrated);
                }
            });
        }

        private void ControllerButton_OptionSelected(object? sender, OptionSelection e)
        {
            // Get the detailButton from the sender
            ControllerDetailButton detailButton = (ControllerDetailButton)sender;

            // Subtract one from the port as this value is used 
            // as the display number
            int port = detailButton.ControllerPort - 1;

            if (e == OptionSelection.EditControllerSettings)
            {
                // Open the controller setting window.
                ControllerSettingWindow settingWindow = new ControllerSettingWindow(ref _gamecubeAdapter, port);
                bool appliedSettings = (bool)settingWindow.ShowDialog();

                // The user applied settings, assume it's calibrated
                if (appliedSettings)
                    _gamecubeAdapter.UpdateCalibration(port, CalibrationStatus.Calibrated);

                // Focus this window after the dialog has been closed
                this.Focus();
            }

            else if (e == OptionSelection.CalibrateController)
            {
                // Display that the calibration is beginning.
                MessageDialog calibrateDialog = new MessageDialog(Strings.PromptCalibrationInstructions, Forms.MessageBoxButtons.OK);
                calibrateDialog.ShowDialog();

                // After the window is closed, focus this main window 
                this.Focus();

                // Set the adapter to begin pulling calibration values
                _gamecubeAdapter.UpdateCalibration(port, CalibrationStatus.Calibrating);

                // Asynchronously wait for 5 seconds, then set it as calibrated.
                Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    _gamecubeAdapter.UpdateCalibration(port, CalibrationStatus.Calibrated);

                }).ContinueWith(task => 
                {
                    // Update the ProfileManager with the new ranges
                    float[] ranges = _gamecubeAdapter.Controllers[port].Calibration.GetRange();
                    ProfileManager.CurrentProfiles[port].LeftStickRange = ranges[0];
                    ProfileManager.CurrentProfiles[port].RightStickRange = ranges[1];

                    // After we're done, tell the user that we're done.
                    // Use the CurrentSynchronizationContext to tell the task
                    // we are using the main UI thread to post this dialog
                    calibrateDialog = new MessageDialog(Strings.NotificationCalibrationComplete, Forms.MessageBoxButtons.OK);
                    calibrateDialog.ShowDialog();

                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            
            else if (e == OptionSelection.PopoutInputViewer)
            {
                // Open the input display window, passing the Gamecube adapter
                // and selected port number.
                InputDisplayWindow inputDisplay = new InputDisplayWindow(ref _gamecubeAdapter, _selectedControllerPort);
                inputDisplay.Show();
            }
        }

        private void ProfileListView_SelectProfileButtonClicked(object? sender, ControllerProfile profile)
        {
            // Update the current profile in the ProfileManager
            ProfileManager.CurrentProfiles[_selectedControllerPort] = profile;

            // Update the calibration status on the controller port
            _gamecubeAdapter.UpdateCalibration(_selectedControllerPort, CalibrationStatus.Calibrated);

            int selectedPort = _selectedControllerPort + 1;

            // Tell the user we've applied the selected profile on
            // X port.
            string profileApplied = string.Format(Strings.NotificationProfileApplied, profile.ProfileName, selectedPort);

            MessageDialog messageDialog = new MessageDialog(profileApplied, Forms.MessageBoxButtons.OK);
            messageDialog.ShowDialog();
        }

        private void ControllerButton_Clicked(object? sender, RoutedEventArgs e)
        {
            // Get the detail button from the sender
            ControllerDetailButton detailButton = (ControllerDetailButton)sender;

            // Update the currently selected controller port 
            _selectedControllerPort = detailButton.ControllerPort - 1;

            // Update the current port to play a recording on.
            _inputPlaybackView.ControllerPort = _selectedControllerPort;
        }

        private void NavigationButton_Clicked(object sender, RoutedEventArgs e)
        {
            // Get the navigation button from the sender
            NavigationButton sideButton = (NavigationButton)sender;

            // Get the navigationTag to set our view with 
            NavigationSelection navigationTag = (NavigationSelection)sideButton.Tag;

            // Update the current view
            SetCurrentView(navigationTag);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Our application is closing, shutdown the running
            // Gamecube adapter and NotifyIcon processes
            _gamecubeAdapter.Stop();
            _notifyIcon?.Dispose();  
        }

        private void StartRecordingInputEventRequested(object? sender, int port)
        {
            _inputPlaybackView.ControllerPort = port;
            _inputPlaybackView.BeginRecording();
        }

        private void StopRecordingInputEventRequested(object? sender, int port)
        {
            _inputPlaybackView.StopRecording();
        }

        private void BeginPlaybackEventRequested(object? sender, int port)
        {
            _inputPlaybackView.ControllerPort = port;
            _inputPlaybackView.StartPlayback();
        }

        private void StopPlaybackEventRequested(object? sender, int port)
        {
            _inputPlaybackView.ControllerPort = port;
            _inputPlaybackView.StopPlayback();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
