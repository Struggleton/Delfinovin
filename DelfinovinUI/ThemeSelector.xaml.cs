using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DelfinovinUI
{
    /// <summary>
    /// Interaction logic for ThemeSelector.xaml
    /// </summary>
    public partial class ThemeSelector : Window
    {
        public WindowResult Result;

        public ThemeSelector()
        {
            InitializeComponent();
			UpdateUIElements();
		}

		private void UpdateUIElements()
        {
			// Bind controller colors to ItemSource
			cmbControllerColor.ItemsSource = Enum.GetValues(typeof(ControllerColor));

			// Try to parse the selected ApplicationSetting to an enum. If it works, set the combobox item
			if (Enum.TryParse(ApplicationSettings.ControllerColor, out ControllerColor controllerColor))
				cmbControllerColor.SelectedItem = controllerColor;

			// Get all of the themes in the Themes folder in the project
			foreach (string file in Extensions.GetResourcesUnder("Themes"))
			{
				// Get the basename with out the extension
				string baseName = System.IO.Path.GetFileNameWithoutExtension(file);

				// Add them to the combobox and uppercase the first letter
				cmbApplicationTheme.Items.Add(Extensions.UppercaseFirst(baseName));
			}

			// Set the selected item to the set application theme 
			cmbApplicationTheme.SelectedItem = ApplicationSettings.ApplicationTheme;
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			// Get the currently selected theme from the comboBox
			var selectedTheme = cmbApplicationTheme.SelectedItem;

			// Check to see if the selected item is null
			if (selectedTheme != null)
            {
				// Update the application-wide theme with the selected one.
				Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
				{
					Source = new Uri($"/DelfinovinUI;component/Themes/{selectedTheme.ToString()}.xaml", UriKind.Relative)
				};

				// Update the ApplicationSettings class
				ApplicationSettings.ApplicationTheme = selectedTheme.ToString();
			}

			// Get the currently controller color from the comboBox
			var selectedColor = cmbControllerColor.SelectedItem;

			// Check to see if the selected item is null
			if (selectedColor != null)
            {
				// Get the controller color 
				if (Enum.TryParse(selectedColor.ToString(), out ControllerColor controllerColor))
				{
					// Cast  the controllerColor into an uint
					uint hexCode = (uint)controllerColor;

					// Convert the uint into a color
					Color convertedColor = Extensions.GetColorFromHex(hexCode);

					// Set the resource "ControllerColor" to with the new color 
					App.Current.Resources["ControllerColor"] = new SolidColorBrush(convertedColor);

					// Update the ApplicationSettings class
					ApplicationSettings.ControllerColor = selectedColor.ToString();
				}
			}

			// Save the ApplicationSettings, set
			// the window result as Saved + Closed
			// and close the window.
			ApplicationSettings.SaveSettings();
			Result = WindowResult.SaveClosed;
			Close();
		}

		// Implement custom header bars
		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			Result = WindowResult.Closed;
			this.Close();
		}

		// Implement custom header bars
		private void rectHeader_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				this.DragMove();
		}

		public enum ControllerColor : uint
		{
			Indigo = 0xFF3C3760, //
			JetBlack = 0x28282B, //
			SpiceOrange = 0xCB7017, //
			Platinum = 0xD3D6D8, // 
			EmeraldBlue = 0x0E8999,
			White = 0xFFFFFF,
			StarlightGold = 0x988350,
			SymphonicGreen = 0x9EB7B4,
			LuigiGreen = 0x078E41,
			MarioRed = 0xCF2B2A,
			WarioYellow = 0xC58A24

		}
	}
}
