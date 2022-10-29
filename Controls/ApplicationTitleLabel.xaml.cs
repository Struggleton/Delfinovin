using System;
using System.Collections.Generic;
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
    /// Interaction logic for ApplicationTitleLabel.xaml
    /// </summary>
    public partial class ApplicationTitleLabel : UserControl
    {
        private Window ParentWindow { get; set; }

        public string HeaderText { get; set; }

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
            if (ParentWindow != null)
                ParentWindow.DragMove();
        }
    }
}
