using System.Windows;
using System.Windows.Controls;

namespace Delfinovin.Controls
{
    /// <summary>
    /// A reusable UserControl to allow for window controls (Minimize, Close, etc.)
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

        private void WindowClose_Click(object sender, RoutedEventArgs e)
        {
            if (ParentWindow != null)
                ParentWindow.Close();
        }

        private void WindowMinimize_Click(object sender, RoutedEventArgs e)
        {
            if (ParentWindow != null)
                ParentWindow.WindowState = WindowState.Minimized;
        }
    }
}
