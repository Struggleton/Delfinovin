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
    /// Interaction logic for NavigationButton.xaml
    /// </summary>
    public partial class NavigationButton : UserControl
    {
        private string _buttonIcon;
        public string ButtonIcon
        {
            get { return _buttonIcon; } 
            set 
            { 
                _buttonIcon = value;
                UpdateButtonIcon();
            }
        } 

        private string _buttonText;
        public string ButtonText 
        { 
            get { return _buttonText; } 
            set { _buttonText = value; } 
        }

        public event EventHandler<RoutedEventArgs>Clicked;
        public NavigationButton()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            Clicked?.Invoke(this, e);
        }

        private void UpdateButtonIcon()
        {
            buttonIcon.Source = new BitmapImage(new Uri(ButtonIcon, UriKind.Relative));
        }
    }
}
