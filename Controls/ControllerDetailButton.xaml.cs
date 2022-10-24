using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Delfinovin.Controls.Windows;

namespace Delfinovin.Controls
{
    /// <summary>
    /// Interaction logic for ControllerDetailButton.xaml
    /// </summary>
    public partial class ControllerDetailButton : UserControl
    {
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

        public bool IsSettingMenuOpen { get; set; } = false;

        public event EventHandler<OptionSelection> OptionSelected;

        public ControllerDetailButton()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void UpdateConnectionDisplay()
        {
            switch (ConnectionStatus)
            {
                case ConnectionStatus.Connected:
                    connectionStatusDisplay.Source = new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/controller-plugged.png", UriKind.Relative));
                    IsEnabled = true;
                    break;
                case ConnectionStatus.Disconnected:
                    connectionStatusDisplay.Source = new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/controller-unplugged.png", UriKind.Relative));
                    CalibrationStatus = CalibrationStatus.Uncalibrated;
                    IsEnabled = false;
                    break;
            }
        }

        private void UpdateCalibrationDisplay()
        {
            calibrationStatusDisplay.Text = CalibrationStatus.ToString();
        }

        private void UpdatePortDisplay() 
        { 
            portDisplay.Text = string.Format(Strings.ControllerText, ControllerPort); 
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
            if (IsSettingMenuOpen)
            {
                settingsButton.Source = new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/arrow-down.png", UriKind.Relative));
            }

            else
            {
                settingsButton.Source = new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/arrow-up.png", UriKind.Relative));
            }
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
            OptionSelected.Invoke(this, e);
        }

        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            OpenDetails();
        }
    }
}
