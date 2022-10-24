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

namespace Delfinovin.Controls.Views
{
    /// <summary>
    /// Interaction logic for DonateSupportView.xaml
    /// </summary>
    public partial class DonateSupportView : UserControl
    {
        public DonateSupportView()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void NavigationButton_Clicked(object sender, RoutedEventArgs e)
        {
            string buttonLink = ((NavigationButton)sender).Tag.ToString();
            Process.Start(buttonLink);
        }
    }
}
