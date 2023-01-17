using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using UserSettings = Delfinovin.Properties.Settings;

namespace Delfinovin.Controls.Views
{
    /// <summary>
    /// A view displaying buttons for supporting
    /// and donating to the project. :)
    /// </summary>
    public partial class DonateSupportView : UserControl
    {
        public DonateSupportView()
        {
            InitializeComponent();
            this.DataContext = this;

            SetDonationCheckVisibility();
        }

        private void SetDonationCheckVisibility()
        {
            // Set the visibility based on if the user's set the donation
            // check to true.
            bool donateHidden = UserSettings.Default.HideDonationOnStartup;
            hideDialog.Visibility = donateHidden ? Visibility.Hidden : Visibility.Visible;
        }

        private void NavigationButton_Clicked(object sender, RoutedEventArgs e)
        {
            // Get the navigation button from the sender
            NavigationButton navButton = (NavigationButton)sender;

            // Get the URL from the button tag
            string buttonLink = navButton.Tag.ToString();

            // Open the URL in the default browser.
            Process.Start(new ProcessStartInfo(buttonLink) 
            { 
                UseShellExecute = true 
            });
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            UserSettings.Default.HideDonationOnStartup = (bool)hideDialog.IsChecked;
            UserSettings.Default.Save();
        }
    }
}
