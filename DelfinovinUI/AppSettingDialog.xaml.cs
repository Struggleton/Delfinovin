using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace DelfinovinUI
{
    /// <summary>
    /// Interaction logic for AppSettingDialog.xaml
    /// </summary>
    public partial class AppSettingDialog : Window
    {
        private List<ControllerSettings> _loadedProfiles;
        public WindowResult Result;

		public AppSettingDialog()
		{
			InitializeComponent();

			// Load each profile then update the controls 
			LoadProfiles();
			UpdateControls();
		}

		private void LoadProfiles()
		{
			// Initialize the list of profiles
			_loadedProfiles = new List<ControllerSettings>();

			// If the profile directory doesn't exist, create it
			if (!Directory.Exists("profiles"))
				Directory.CreateDirectory("profiles");

			// Load all text files from the profile directory.
			// if there are none stop execution
			string[] files = Directory.GetFiles(".\\profiles", "*.txt");
			if (files.Length == 0)
				return;
			
			for (int i = 0; i < files.Length; i++)
			{
				// Create a new profile to load settings into
				ControllerSettings controllerSettings = new ControllerSettings();

				if (controllerSettings.LoadFromFile(files[i]))
				{
					// Update the UI with the set default profiles
					string name = System.IO.Path.GetFileNameWithoutExtension(files[i]);
					_loadedProfiles.Add(controllerSettings);
					defaultProfile1.Items.Add(name);
					defaultProfile2.Items.Add(name);
					defaultProfile3.Items.Add(name);
					defaultProfile4.Items.Add(name);
				}
			}
		}

		private void UpdateControls()
		{
			// Update the controls based on the currently loaded ApplicationSettings
			minimizeSystemTray.IsChecked = ApplicationSettings.MinimizeToTray;
			minimizeOnStartup.IsChecked = ApplicationSettings.MinimizeOnStartup;
			checkForUpdates.IsChecked = ApplicationSettings.CheckForUpdates;
			defaultProfile1.SelectedItem = ApplicationSettings.DefaultProfile1;
			defaultProfile2.SelectedItem = ApplicationSettings.DefaultProfile2;
			defaultProfile3.SelectedItem = ApplicationSettings.DefaultProfile3;
			defaultProfile4.SelectedItem = ApplicationSettings.DefaultProfile4;
		}

		// Implement header bars
		private void rectHeader_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				this.DragMove();
			}
		}

		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			Result = WindowResult.Closed;
			this.Close();
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			// Gather the application values and save them to the file.
			ApplicationSettings.MinimizeToTray = minimizeSystemTray.IsChecked.Value;
			ApplicationSettings.MinimizeOnStartup = minimizeOnStartup.IsChecked.Value;
			ApplicationSettings.CheckForUpdates = checkForUpdates.IsChecked.Value;
			ApplicationSettings.DefaultProfile1 = defaultProfile1.Text;
			ApplicationSettings.DefaultProfile2 = defaultProfile2.Text;
			ApplicationSettings.DefaultProfile3 = defaultProfile3.Text;
			ApplicationSettings.DefaultProfile4 = defaultProfile4.Text;

			ApplicationSettings.SaveSettings();

			Result = WindowResult.SaveClosed;
			Close();
		}

        private void btnUpdates_Click(object sender, RoutedEventArgs e)
        {
			Updater.CheckCurrentRelease(false);
        }
    }
}
