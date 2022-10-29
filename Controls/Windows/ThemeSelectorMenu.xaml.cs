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
using System.Windows.Shapes;
using Delfinovin.Controls;
using UserSettings = Delfinovin.Properties.Settings;

namespace Delfinovin.Controls.Windows
{
    /// <summary>
    /// Interaction logic for ThemeSelectorMenu.xaml
    /// </summary>
    public partial class ThemeSelectorMenu : Window
    {
        public ThemeSelectorMenu()
        {
            InitializeComponent();
            this.DataContext = this;

            UpdateColorSelector();
        }

        private void UpdateColorSelector()
        {
            List<string> colorOptions = Enum.GetNames(typeof(ControllerColor)).ToList();
            colorOptions.Reverse();

            string selectedColor = UserSettings.Default.ControllerColor;
            controllerColor.Items = colorOptions;
            controllerColor.SelectedIndex = colorOptions.IndexOf(selectedColor);
        }

        private void saveTheme_Click(object sender, RoutedEventArgs e)
        {
            SetControllerColor(controllerColor.SelectedItem);

            Close();
        }

        private void SetControllerColor(string selectedColor)
        {
            if (selectedColor != null)
            {
                // Get the controller color 
                if (Enum.TryParse(selectedColor, out ControllerColor controllerColor))
                {
                    // Cast the controllerColor into an uint
                    uint hexCode = (uint)controllerColor;

                    // Convert the uint into a color
                    Color convertedColor = ControlExtensions.GetColorFromHex(hexCode);

                    // Set the resource "ControllerColor" to with the new color 
                    Application.Current.Resources["ControllerColor"] = new SolidColorBrush(convertedColor);

                    // Update user settings.
                    UserSettings.Default.ControllerColor = selectedColor;
                    UserSettings.Default.Save();
                }
            }
        }
    }


}
