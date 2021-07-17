using System;
using System.Collections.Generic;
using System.IO;
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
			_loadedProfiles = new List<ControllerSettings>();
			if (!Directory.Exists("profiles"))
				Directory.CreateDirectory("profiles");

			string[] files = Directory.GetFiles("profiles", "*.txt");
			if (files.Length == 0)
				return;

			for (int i = 0; i < files.Length; i++)
			{
				string[] settings = File.ReadAllLines(files[i]);
				ControllerSettings controllerSettings = new ControllerSettings();
				if (settings.Length == 0)
					return;

				controllerSettings.LoadFromFile(files[i]);
				string name = System.IO.Path.GetFileNameWithoutExtension(files[i]);
				_loadedProfiles.Add(controllerSettings);

				if (!lsbProfiles.Items.Contains(name))
					lsbProfiles.Items.Add(name);
			}
		}

		// Implement custom header bars
		private void rectHeader_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				this.DragMove();
			}
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
				SelectedProfile = _loadedProfiles[lsbProfiles.SelectedIndex];
				WindowResult = WindowResult.SaveClosed;
				this.Close();
			}
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			TextEntry textEntry = new TextEntry("Enter Profile Name");
			textEntry.ShowDialog();

			if (textEntry.Result != WindowResult.Closed && !string.IsNullOrEmpty(textEntry.EnteredText))
			{
				string fileName = textEntry.EnteredText;
				char[] invalids = System.IO.Path.GetInvalidFileNameChars(); // Get file name invalid characters 
				string newName = string.Join("_", fileName.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.'); // Sanitize file name

				_currentSettings.SaveFile($"profiles\\{newName}.txt");
				LoadSettings();

				new MessageWindow($"Saved {newName} to a profile.", displayLeft: false, displayRight: true, "OK", "OK")
					.ShowDialog();
			}
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (File.Exists($"profiles\\{lsbProfiles.SelectedItem}.txt"))
			{
				MessageWindow messageWindow = new MessageWindow($"Are you sure you want to delete {lsbProfiles.SelectedItem}?", displayLeft: true, displayRight: true, "Yes", "No");
				messageWindow.ShowDialog();

				if (messageWindow.Result == WindowResult.OK)
				{
					File.Delete($"profiles\\{lsbProfiles.SelectedItem}.txt");
					_loadedProfiles.RemoveAt(lsbProfiles.SelectedIndex);
					lsbProfiles.Items.Remove(lsbProfiles.SelectedItem);
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
				StringBuilder sb = new StringBuilder();

				sb.AppendLine("Enable Digital Triggers - " + ConvertYesNo(settings.EnableDigitalPress));
				sb.AppendLine("Enable Rumble - " + ConvertYesNo(settings.EnableRumble));
				sb.AppendLine("Swap A/B Buttons - " + ConvertYesNo(settings.SwapAB));
				sb.AppendLine("Swap X/Y Buttons - " + ConvertYesNo(settings.SwapXY));
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
