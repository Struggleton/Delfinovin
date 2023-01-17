using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Forms = System.Windows.Forms;
using ProfileManager = Delfinovin.Controllers.ProfileManager;
using UserSettings = Delfinovin.Properties.Settings;

namespace Delfinovin.Controls.Windows
{
    /// <summary>
    /// A window providing controls to change
    /// Application-wide settings.
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
            CreateComboBoxListItems();
        }

        private void UpdateControls()
        {
            // Update the controls with the application settings.
            checkUpdatesStartup.Checked = UserSettings.Default.CheckForUpdates;
            minimizeToSystemTray.Checked = UserSettings.Default.MinimizeToTray;
            minimizeAppOnStartup.Checked = UserSettings.Default.MinimizeOnStartup;
            runAppOnPCStart.Checked = UserSettings.Default.RunOnStartup;
        }

        private void CreateComboBoxListItems()
        {
            for (int i = 0; i < UserSettings.Default.DefaultProfiles.Count; i++)
            {
                profileListItems[i] = new ComboBoxListItem();
                profileListItems[i].ItemText = Strings.ListItemDefaultProfile + $"{i + 1}";
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
            // Check to see if the profile still exists.
            // If not, remove it from the DefaultProfiles entry.
            bool profileExists = _profiles.Contains(profileName);
            UserSettings.Default.DefaultProfiles[profileNum] = profileExists ? profileName : "";
            UserSettings.Default.Save();
        }

        private void UpdateStartupEntry(bool runOnStartup)
        {
            // Get the Startup folder's path 
            string startupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "Delfinovin.lnk");
            if (runOnStartup)
            {
                ControlExtensions.CreateApplicationShortcut(startupPath);
            }

            else
            {
                // See if the shortcut exists. If it does remove it
                if (File.Exists(startupPath))
                {
                    File.Delete(startupPath);
                }
            }
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            // Gather the settings from the controls.
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

            // Save the user settings.
            UserSettings.Default.Save();

            // Update the startup entry, if it hasn't already.
            UpdateStartupEntry(runAppOnPCStart.Checked);
            Close();
        }

        private void SelectTheme_Click(object sender, RoutedEventArgs e)
        {
            // Open the theme selector menu
            ThemeSelectorMenu themeMenu = new ThemeSelectorMenu();
            themeMenu.ShowDialog();
        }

        private void RestoreDefaults_Click(object sender, RoutedEventArgs e)
        {
            // Prompt the user if they'd like to reset the application settings.
            MessageDialog messageDialog = new MessageDialog(Strings.PromptResetSettings);
            messageDialog.ShowDialog();

            bool restore = messageDialog.Result == Forms.DialogResult.Yes;

            // Exit if they choose no
            if (!restore)
                return;

            // Reset these values to the defaults and save it to 
            // the application setting file.
            UserSettings.Default.CheckForUpdates = true;
            UserSettings.Default.MinimizeOnStartup = false;
            UserSettings.Default.MinimizeToTray = false;
            UserSettings.Default.RunOnStartup = false;
            UserSettings.Default.ControllerColor = Enum.GetName(typeof(ControllerColor), ControllerColor.Indigo);

            // Reset all of the default profiles.
            for (int i = 0; i < UserSettings.Default.DefaultProfiles.Count; i++)
                UserSettings.Default.DefaultProfiles[i] = String.Empty;

            UserSettings.Default.HideDonationOnStartup = false;

            // Save the user settings.
            UserSettings.Default.Save();

            // Update the startup entry and controls
            UpdateStartupEntry(false);
            UpdateControls();

            // Tell the user we've reset the settings.
            messageDialog = new MessageDialog(Strings.NotificationSettingsReset, Forms.MessageBoxButtons.OK);
            messageDialog.ShowDialog();
        }

        private void HotkeySettings_Click(object sender, RoutedEventArgs e)
        {
            // Open the HotkeyMappingWindow
            HotkeyMappingWindow mappingWindow = new HotkeyMappingWindow();
            mappingWindow.ShowDialog();

            // Reload the settings. 
            // If the user didn't save any settings
            // the hotkeys will not persist.
            UserSettings.Default.Reload();
        }

        private void CheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            // Pass false to enable response windows
            Updater.CheckForUpdates(false);
        }
    }
}
