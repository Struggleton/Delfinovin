using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Delfinovin.Controls
{
    /// <summary>
    /// A control displaying the application icon and 
    /// text to be placed as a header.
    /// </summary>
    public partial class ApplicationTitleLabel : UserControl
    {
        public string HeaderText { get; set; }

        private Window ParentWindow { get; set; }

        public ApplicationTitleLabel()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void WindowControlsLoaded(object sender, RoutedEventArgs e)
        {
            // Get the parent window so we can manipulate it
            ParentWindow = Window.GetWindow(this);
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // If the parent window isn't null, drag it wherever our mouse moves.
            if (ParentWindow != null)
                ParentWindow.DragMove();
        }
    }
}
