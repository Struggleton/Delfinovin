using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using UserSettings = Delfinovin.Properties.Settings;

namespace Delfinovin.Controls.Windows
{
    /// <summary>
    /// A window providing the user with a list of themes/colors
    /// to customize the application with.
    /// </summary>
    public partial class ThemeSelectorMenu : Window
    {
        private const string SELECTED_THEME_FILE = "/Delfinovin;component/Resources/Themes/{0}.xaml";
        private const string THEME_PATH = "Resources/Themes";

        public ThemeSelectorMenu()
        {
            InitializeComponent();
            this.DataContext = this;

            UpdateColorSelector();
            UpdateThemeSelector();  
        }

        private void UpdateThemeSelector()
        {
            // Get all of the themes in the Themes folder in the project
            foreach (string file in ControlExtensions.GetResourcesUnder(THEME_PATH))
            {
                // Get the basename with out the extension
                string baseName = Path.GetFileNameWithoutExtension(file);

                // Add them to the combobox and uppercase the first letter
                applicationTheme.Items.Add(Extensions.UppercaseFirst(baseName));
            }

            // Set the selected item to the set application theme 
            applicationTheme.SelectedItem = UserSettings.Default.ApplicationTheme;
        }

        private void UpdateColorSelector()
        {
            // Get the list of possible colors to choose from
            List<string> colorOptions = Enum.GetNames(typeof(ControllerColor)).ToList();

            // Reverse the list, with the oldest at the top.
            colorOptions.Reverse();

            // Add the items to the Combo box list.
            controllerColor.Items = colorOptions;

            // Pull from the usersettings the color to use.
            string selectedColor = UserSettings.Default.ControllerColor;
            
            // Set the selected index to the index of our selected color.
            controllerColor.SelectedIndex = colorOptions.IndexOf(selectedColor);
        }

        private void SaveTheme_Click(object sender, RoutedEventArgs e)
        {
            SetControllerColor(controllerColor.SelectedItem);
            SetTheme(applicationTheme.SelectedItem);

            // Save the user settings.
            UserSettings.Default.Save();
            Close();
        }

        private void SetControllerColor(string selectedColor)
        {
            if (String.IsNullOrEmpty(selectedColor))
                return;
            
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
            }
        }

        private void SetTheme(string selectedTheme)
        {
            if (String.IsNullOrEmpty(selectedTheme))
                return;

            // Update the application-wide theme with the selected one.
            Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
            {
                Source = new Uri(string.Format(SELECTED_THEME_FILE, selectedTheme), UriKind.Relative)
            };

            // Save the application theme to our UserSettings
            UserSettings.Default.ApplicationTheme = selectedTheme;
        }
    }
}
