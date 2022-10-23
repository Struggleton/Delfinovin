using Delfinovin.Controls;
using System;
using System.Collections.Generic;
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

namespace Delfinovin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetApplicationTitle();
            CreateDetailButtons();
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

                controllerList.Children.Add(button);
            }
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
