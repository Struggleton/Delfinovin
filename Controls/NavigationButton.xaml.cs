using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Delfinovin.Controls
{
    /// <summary>
    /// A control used to navigate through an applications' views
    /// </summary>
    public partial class NavigationButton : UserControl
    {
        public string ButtonText { get; set; }

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

        public event EventHandler<RoutedEventArgs>Clicked;

        public NavigationButton()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void UpdateButtonIcon()
        {
            buttonIcon.Source = new BitmapImage(new Uri(ButtonIcon, UriKind.Relative));
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            Clicked?.Invoke(this, e);
        }
    }
}
