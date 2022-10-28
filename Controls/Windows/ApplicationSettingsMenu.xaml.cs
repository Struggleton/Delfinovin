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
using UserSettings = Delfinovin.Properties.Settings;
using ProfileManager = Delfinovin.Controllers.ProfileManager;

namespace Delfinovin.Controls.Windows
{
    /// <summary>
    /// Interaction logic for ApplicationSettingsMenu.xaml
    /// </summary>
    public partial class ApplicationSettingsMenu : Window
    {
        private List<string> _profiles => ProfileManager.GetProfileNameList();
        private ComboBoxListItem[] profileListItems = new ComboBoxListItem[4];

        public ApplicationSettingsMenu()
        {
            InitializeComponent();
            this.DataContext = this;

            UpdateControls();
        }

        private void UpdateControls()
        {
            checkUpdatesStartup.Checked = UserSettings.Default.CheckForUpdates;
            minimizeToSystemTray.Checked = UserSettings.Default.MinimizeToTray;
            minimizeAppOnStartup.Checked = UserSettings.Default.MinimizeOnStartup;
            runAppOnPCStart.Checked = UserSettings.Default.RunOnStartup;

            CreateComboBoxListItems();
        }

        private void CreateComboBoxListItems()
        {
            for (int i = 0; i < UserSettings.Default.DefaultProfiles.Count; i++)
            {
                profileListItems[i] = new ComboBoxListItem();
                profileListItems[i].ItemText = $"Default Profile - Controller #{i + 1}";
                profileListItems[i].Items = _profiles;

                // Retrive the currently set default profile
                string defaultProfileName = UserSettings.Default.DefaultProfiles[i];

                // Check to see if it still exists. If it does,
                // set the currently selected entry to that one.
                UpdateDefaultProfileEntries(i, defaultProfileName);
                profileListItems[i].SelectedIndex = profileListItems[i].Items.IndexOf(defaultProfileName);

                // Add the items to the stackpanel.
                settingsContainer.Children.Add(profileListItems[i]);
            }
        }

        private void UpdateDefaultProfileEntries(int profileNum, string profileName)
        {
            bool profileExists = _profiles.Contains(profileName);
            UserSettings.Default.DefaultProfiles[profileNum] = profileExists ? profileName : "";
            UserSettings.Default.Save();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void saveSettings_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.Default.CheckForUpdates = checkUpdatesStartup.Checked;
            UserSettings.Default.MinimizeOnStartup = minimizeAppOnStartup.Checked;
            UserSettings.Default.MinimizeToTray = minimizeToSystemTray.Checked;
            UserSettings.Default.RunOnStartup = runAppOnPCStart.Checked;

            for (int i = 0; i < UserSettings.Default.DefaultProfiles.Count; i++)
            {
                // If the selected item exists in the profile list, update
                // the default profile, and the currently loaded one.
                if (_profiles.Contains(profileListItems[i].SelectedItem))
                {
                    UserSettings.Default.DefaultProfiles[i] = profileListItems[i].SelectedItem;
                    ProfileManager.CurrentProfiles[i] = ProfileManager.GetProfileFromProfileName(profileListItems[i].SelectedItem);
                }
            }

            UserSettings.Default.Save();
            Close();
        }
    }
}
