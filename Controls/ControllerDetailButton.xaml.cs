using Delfinovin.Controls.Windows;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Delfinovin.Controls
{
    /// <summary>
    /// A user control to display the current status of the 
    /// associated game controller and manage settings.
    /// </summary>
    public partial class ControllerDetailButton : UserControl
    {
        private const string CONTROLLER_PLUGGED_LINK = "/Delfinovin;component/Resources/Icons/controller-plugged.png";
        private const string CONTROLLER_UNPLUGGED_LINK = "/Delfinovin;component/Resources/Icons/controller-unplugged.png";
        private const string ARROW_DOWN = "/Delfinovin;component/Resources/Icons/arrow-down.png";
        private const string ARROW_UP = "/Delfinovin;component/Resources/Icons/arrow-up.png";

        public bool IsSettingMenuOpen { get; set; } = false;

        private ConnectionStatus _connectionStatus;
        public ConnectionStatus ConnectionStatus
        {
            get { return _connectionStatus; }
            set
            {
                _connectionStatus = value;
                UpdateConnectionDisplay();
            }
        }

        private CalibrationStatus _calibrationStatus;
        public CalibrationStatus CalibrationStatus
        {
            get { return _calibrationStatus; }
            set
            {
                _calibrationStatus = value;
                UpdateCalibrationDisplay();
            }
        }

        private int _controllerPort;
        public int ControllerPort
        {
            get { return _controllerPort; }
            set 
            { 
                _controllerPort = value;
                UpdatePortDisplay();
            }
        }

        public event EventHandler<OptionSelection> OptionSelected;
        public event EventHandler<RoutedEventArgs> Clicked;

        public ControllerDetailButton()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void OpenDetails()
        {
            // Create a new optionsMenu and spawn it above the current button.
            ControllerOptionsMenu optionsMenu = new ControllerOptionsMenu();

            // Subscribe to the window events
            optionsMenu.OptionSelected += ControllerOptionsMenu_OptionSelected;
            optionsMenu.Closed += ControllerOptionsMenu_Closed;
            optionsMenu.Show();

            // Get the point above this button and spawn the menu at that location.
            Point location = ControlExtensions.GetScreenSpawnPosition(this, optionsMenu);
            optionsMenu.Left = location.X;
            optionsMenu.Top = location.Y;

            IsSettingMenuOpen = true;
            UpdateSettingsTab();
        }

        private void UpdateSettingsTab()
        {
            // Update the expander icon based on if the menu is open or not.
            if (IsSettingMenuOpen)
            {
                settingsButton.Source = new BitmapImage(new Uri(ARROW_DOWN, UriKind.Relative));
            }

            else
            {
                settingsButton.Source = new BitmapImage(new Uri(ARROW_UP, UriKind.Relative));
            }
        }

        private void UpdateConnectionDisplay()
        {
            switch (ConnectionStatus)
            {
                case ConnectionStatus.Connected:
                    connectionStatusDisplay.Source = new BitmapImage(new Uri(CONTROLLER_PLUGGED_LINK, UriKind.Relative));
                    IsEnabled = true;
                    break;
                case ConnectionStatus.Disconnected:
                    connectionStatusDisplay.Source = new BitmapImage(new Uri(CONTROLLER_UNPLUGGED_LINK, UriKind.Relative));
                    CalibrationStatus = CalibrationStatus.Uncalibrated;
                    IsEnabled = false;
                    break;
            }
        }

        private void UpdatePortDisplay()
        {
            portDisplay.Text = string.Format(Strings.DetailsControllerPort, ControllerPort);
        }

        private void UpdateCalibrationDisplay()
        {
            calibrationStatusDisplay.Text = CalibrationStatus.ToString();
        }

        private void ControllerOptionsMenu_Closed(object? sender, EventArgs e)
        {
            // The menu was closed, update the settings button.
            IsSettingMenuOpen = false;
            UpdateSettingsTab();
        }

        private void ControllerOptionsMenu_OptionSelected(object? sender, OptionSelection e)
        {
            // Pass the selected option to the event handler.
            OptionSelected?.Invoke(this, e);
        }

        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the details menu and invoke the click event.
            OpenDetails();
            Clicked?.Invoke(this, e);
        }
    }
}
