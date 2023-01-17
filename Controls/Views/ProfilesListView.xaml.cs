using Delfinovin.Controllers;
using Delfinovin.Controls.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;

namespace Delfinovin.Controls.Views
{
    /// <summary>
    /// Provide a view for managing and loading saved profiles
    /// </summary>
    public partial class ProfilesListView : UserControl
    {
        public event EventHandler<ControllerProfile> SelectProfileButtonClicked;
        public event EventHandler<ControllerProfile> DeleteProfileButtonClicked;

        public ProfilesListView()
        {
            InitializeComponent();
            UpdateList();

            DeleteProfileButtonClicked += DeleteProfileClicked;
        }

        private void DeleteProfileClicked(object? sender, ControllerProfile profile)
        {
            if (profile.Favorited)
            {
                // Prevent the user from deleting the favorited profile.
                MessageDialog messageDialog = new MessageDialog(Strings.NotificationProfileFavorited, Forms.MessageBoxButtons.OK);
                messageDialog.ShowDialog();
            }

            else
            {
                // Prompt the user if they'd like to delete the profile
                string deleteMessage = string.Format(Strings.PromptDeleteProfile, profile.ProfileName);
                MessageDialog messageDialog = new MessageDialog(deleteMessage);
                messageDialog.ShowDialog();
                
                bool result = messageDialog.Result == Forms.DialogResult.Yes;
                if (!result)
                    return;

                // Delete the profile using its profile name and refresh the ListView.
                ProfileManager.DeleteProfileByProfileName(profile.ProfileName);
                UpdateList();

                // Tell the user we've deleted the profile.
                messageDialog = new MessageDialog(Strings.NotificationProfileDeleted, Forms.MessageBoxButtons.OK);
                messageDialog.ShowDialog();
            }
        }

        private void UpdateList()
        {
            // Get a list of profile names
            List<string> profileNameList = ProfileManager.GetProfileNameList();

            // Create a list of profileButtons to add to the ListView
            List<ProfileDetailButton> profileButtons = new List<ProfileDetailButton>();

            // Clear the current list view
            profileList.Items.Clear();

            foreach (string profileName in profileNameList) 
            { 
                // Load the profile and create a profile detail button.
                ControllerProfile profile = ProfileManager.GetProfileFromProfileName(profileName);
                ProfileDetailButton profileDetail = new ProfileDetailButton(profile);

                // Add the control to the list.
                profileButtons.Add(profileDetail);
            }

            // Sort the list based what is favorited
            profileButtons = profileButtons.OrderByDescending(x => x.Profile.Favorited)
                                           .ToList();
            
            foreach (ProfileDetailButton detailButton in profileButtons) 
            { 
                // Add each profile detail button to the ListView
                profileList.Items.Add(detailButton);
            }
        }

        private void ProfileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable/disable the apply/delete buttons based on if the
            // selected index is valid.
            selectProfile.IsEnabled = profileList.SelectedIndex != -1;
            deleteProfile.IsEnabled = profileList.SelectedIndex != -1;
        }

        private void SelectProfile_Click(object sender, RoutedEventArgs e)
        {
            // Get the ProfileDetailButton from the selected index
            ProfileDetailButton profileButton = (ProfileDetailButton)profileList.Items[profileList.SelectedIndex];

            // Raise the event with the profile
            SelectProfileButtonClicked?.Invoke(this, profileButton.Profile);
        }

        private void DeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            // Get the ProfileDetailButton from the selected index
            ProfileDetailButton profileButton = (ProfileDetailButton)profileList.Items[profileList.SelectedIndex];

            // Raise the event with the profile
            DeleteProfileButtonClicked?.Invoke(this, profileButton.Profile);
        }
    }
}
