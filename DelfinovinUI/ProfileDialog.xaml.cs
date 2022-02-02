using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DelfinovinUI
{
    /// <summary>
    /// Interaction logic for ProfileDialog.xaml
    /// </summary>
    public partial class ProfileDialog : Window
    {
        private ControllerSettings _currentSettings;
        private List<ControllerSettings> _loadedProfiles;

        public ControllerSettings SelectedProfile { get; set; }
        public WindowResult WindowResult { get; set; }

        public ProfileDialog()
        {
            InitializeComponent();
        }

		public ProfileDialog(ControllerSettings currentProfile)
		{
			InitializeComponent();
			_currentSettings = currentProfile;

			LoadSettings();
		}

		private void LoadSettings()
		{
			// Initialize the list
			_loadedProfiles = new List<ControllerSettings>();

			// If the profiles folder doesn't exist, create it
			if (!Directory.Exists("profiles"))
				Directory.CreateDirectory("profiles");

			// Load all text files from the profiles folder
			string[] files = Directory.GetFiles(".\\profiles", "*.txt");

			// If we don't have any files, stop loading
			if (files.Length == 0)
				return;

			for (int i = 0; i < files.Length; i++)
			{
				// Load the text to an array
				string[] settings = File.ReadAllLines(files[i]);

				// If the settings file is empty, skip it
				if (settings.Length == 0)
					continue; 

				// Create a new Controller profile to load settings into
				// and load it from the file.
				ControllerSettings controllerSettings = new ControllerSettings();
				controllerSettings.LoadFromFile(files[i]);

				// Get the name of the file so we can add it to the list
				string name = System.IO.Path.GetFileNameWithoutExtension(files[i]);
				_loadedProfiles.Add(controllerSettings);


				// If it already exists, don't add it again
				if (!lsbProfiles.Items.Contains(name))
					lsbProfiles.Items.Add(name);
			}
		}

		// Implement custom header bars
		private void rectHeader_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				this.DragMove();
		}

		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
			WindowResult = WindowResult.Closed;
		}


		// Profile setting buttons
		private void btnLoad_Click(object sender, RoutedEventArgs e)
		{
			if (lsbProfiles.SelectedIndex != -1)
			{
				// Return the profile and close the window.
				SelectedProfile = _loadedProfiles[lsbProfiles.SelectedIndex];
				WindowResult = WindowResult.SaveClosed;
				this.Close();
			}
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			TextEntry textEntry = new TextEntry("Enter Profile Name");
			textEntry.ShowDialog();

			// Continue if the text entry returned a value + we saved it
			if (textEntry.Result != WindowResult.Closed && !string.IsNullOrEmpty(textEntry.EnteredText))
			{
				string fileName = textEntry.EnteredText;

				// Get file name invalid characters 
				char[] invalids = System.IO.Path.GetInvalidFileNameChars();

				// Sanitize file name
				string newName = string.Join("_", fileName.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.'); 

				// Save it to a file and reload the settings
				_currentSettings.SaveFile($"profiles\\{newName}.txt");
				LoadSettings();

				// Notify the user
				new MessageWindow($"Saved {newName} to a profile.", displayLeft: false, displayRight: true, "OK", "OK")
					.ShowDialog();
			}
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			// If the file still exists, prompt them to delete.
			if (File.Exists($"profiles\\{lsbProfiles.SelectedItem}.txt"))
			{
				MessageWindow messageWindow = new MessageWindow($"Are you sure you want to delete {lsbProfiles.SelectedItem}?", displayLeft: true, displayRight: true, "Yes", "No");
				messageWindow.ShowDialog();

				if (messageWindow.Result == WindowResult.OK)
				{
					File.Delete($"profiles\\{lsbProfiles.SelectedItem}.txt");

					// Remove the item from the arrays 
					_loadedProfiles.RemoveAt(lsbProfiles.SelectedIndex);
					lsbProfiles.Items.Remove(lsbProfiles.SelectedItem);

					// Clear the settings display area
					txbSettingDisplay.Text = "";

					new MessageWindow("Profile deleted.", displayLeft: false, displayRight: true, "OK", "OK").ShowDialog();
				}
			}
		}

		private void lsbProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (lsbProfiles.SelectedIndex != -1)
			{
				ControllerSettings settings = _loadedProfiles[lsbProfiles.SelectedIndex];

				// Show settings by using a string builder to create a paragraph
				StringBuilder sb = new StringBuilder();

				sb.AppendLine("Enable Digital Triggers - " + ConvertYesNo(settings.EnableDigitalPress));
				sb.AppendLine("Enable Rumble - " + ConvertYesNo(settings.EnableRumble));
				sb.AppendLine("Swap A/B Buttons - " + ConvertYesNo(settings.SwapAB));
				sb.AppendLine("Swap X/Y Buttons - " + ConvertYesNo(settings.SwapXY));
				sb.AppendLine("Guide BTN Combo - " + ConvertYesNo(settings.GuideBTNCombo));
				sb.AppendLine($"Trigger Deadzones - {settings.TriggerDeadzone * 100f}%");
				sb.AppendLine($"Trigger Threshold - {settings.TriggerThreshold * 100f}%");
				sb.AppendLine($"Left Stick Deadzones - {settings.LeftStickDeadzone * 100f}%");
				sb.AppendLine($"Left Stick Ranges - {settings.LeftStickRange * 100f}%");
				sb.AppendLine($"Right Stick Deadzones - {settings.RightStickDeadzone * 100f}%");
				sb.AppendLine($"Right Stick Ranges - {settings.RightStickRange * 100f}%");
				txbSettingDisplay.Text = sb.ToString();
			}
		}

		private string ConvertYesNo(bool input)
		{
			return input ? "✔" : "✖";
		}
	}
}
