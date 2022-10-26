using Delfinovin.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
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
using Delfinovin.Controls.Views;

namespace Delfinovin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GamecubeDialog _controllerDialog = new GamecubeDialog();
        private int _selectedControllerPort = 0;
        
        public MainWindow()
        {
            InitializeComponent();
            SetApplicationTitle();
            CreateDetailButtons();
            SetDefaultView();
        }

        private void SetApplicationTitle()
        {
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            appTitle.Content = Strings.WindowTitle + " " + string.Format(Strings.VersionText, 
                appVersion.Major, 
                appVersion.Minor, 
                appVersion.Build);
        }

        private void CreateDetailButtons()
        {
            for (int i = 0; i < 4; i++)
            {
                ControllerDetailButton button = new ControllerDetailButton()
                {
                    CalibrationStatus = CalibrationStatus.Uncalibrated,
                    ConnectionStatus = ConnectionStatus.Connected,
                    ControllerPort = i + 1,
                };

                button.Clicked += ControllerButton_Clicked;
                controllerList.Children.Add(button);
            }
        }

        private void ControllerButton_Clicked(object? sender, RoutedEventArgs e)
        {
            _selectedControllerPort = ((ControllerDetailButton)sender).ControllerPort - 1;
            Debug.WriteLine(_selectedControllerPort);
        }

        private void NavigationButton_Clicked(object sender, RoutedEventArgs e)
        {
            NavigationSelection navigationTag = (NavigationSelection)((NavigationButton)sender).Tag;
            SetCurrentView(navigationTag);
        }

        private void SetCurrentView(NavigationSelection navigationSelection)
        {
            if (navigationSelection == NavigationSelection.Home)
            {
                viewDisplay.Content = _controllerDialog;
                viewDisplay.OnApplyTemplate();
            }

            else if (navigationSelection == NavigationSelection.DonationSupport)
            {
                DonateSupportView donateSupportView = new DonateSupportView();
                viewDisplay.Content = donateSupportView;
                viewDisplay.OnApplyTemplate();
            }
        }

        private void SetDefaultView()
        {
            bool donateHidden = Properties.Settings.Default.HideDonationOnStartup;
            NavigationSelection navigationSelection = donateHidden ? NavigationSelection.Home : 
                                                                     NavigationSelection.DonationSupport;

            SetCurrentView(navigationSelection);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
