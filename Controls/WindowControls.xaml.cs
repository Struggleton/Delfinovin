using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace Delfinovin.Controls
{
    /// <summary>
    /// Interaction logic for WindowControls.xaml
    /// </summary>
    public partial class WindowControls : UserControl
    {
        private Window ParentWindow { get; set; }

        public Visibility MinimizeButtonVisibility { get; set; }
        public Visibility CloseButtonVisibility { get; set; }

        public WindowControls()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void WindowControlsLoaded(object sender, RoutedEventArgs e)
        {
            // Get the parent window so we can manipulate it
            ParentWindow = Window.GetWindow(this);
        }

        private void windowClose_Click(object sender, RoutedEventArgs e)
        {
            if (ParentWindow != null)
                ParentWindow.Close();
        }

        private void windowMinimize_Click(object sender, RoutedEventArgs e)
        {
            if (ParentWindow != null)
                ParentWindow.WindowState = WindowState.Minimized;
        }
    }
}
