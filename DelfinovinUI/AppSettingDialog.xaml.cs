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
    /// Interaction logic for AppSettingDialog.xaml
    /// </summary>
    public partial class AppSettingDialog : Window
    {
        private List<ControllerSettings> _loadedProfiles;

        public WindowResult Result;

		public AppSettingDialog()
		{
			InitializeComponent();
			LoadProfiles();
			UpdateControls();
		}

		private void LoadProfiles()
		{
			_loadedProfiles = new List<ControllerSettings>();
			if (!Directory.Exists("profiles"))
				Directory.CreateDirectory("profiles");

			string[] files = Directory.GetFiles(".\\profiles", "*.txt");
			if (files.Length == 0)
				return;
			
			for (int i = 0; i < files.Length; i++)
			{
				ControllerSettings controllerSettings = new ControllerSettings();
				if (controllerSettings.LoadFromFile(files[i]))
				{
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
			minimizeSystemTray.IsChecked = ApplicationSettings.MinimizeToTray;
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
			ApplicationSettings.MinimizeToTray = minimizeSystemTray.IsChecked.Value;
			ApplicationSettings.DefaultProfile1 = defaultProfile1.Text;
			ApplicationSettings.DefaultProfile2 = defaultProfile2.Text;
			ApplicationSettings.DefaultProfile3 = defaultProfile3.Text;
			ApplicationSettings.DefaultProfile4 = defaultProfile4.Text;
			ApplicationSettings.SaveSettings();

			Result = WindowResult.SaveClosed;
			Close();
		}
	}
}
