using Delfinovin.Controllers;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UserSettings = Delfinovin.Properties.Settings;

namespace Delfinovin.Controls
{
    /// <summary>
    /// A control displaying information about an associated ControllerProfile (Name, Deadzones, etc.)
    /// </summary>
    public partial class ProfileDetailButton : UserControl
    {
        private const string NOT_FAVORITE_ICON_LINK = "/Delfinovin;component/Resources/Icons/not-favorite.png";
        private const string FAVORITE_DEFAULT_ICON_LINK = "/Delfinovin;component/Resources/Icons/favorite-default.png";
        private const string FAVORITE_ICON_LINK = "/Delfinovin;component/Resources/Icons/favorite.png";

        private ControllerProfile _controllerProfile;
        public ControllerProfile Profile
        {
            get { return _controllerProfile; }
            set
            {
                _controllerProfile = value;
                UpdateDetails(_controllerProfile);
            }
        }

        public ProfileDetailButton()
        {
            InitializeComponent();
        }

        public ProfileDetailButton(ControllerProfile profile)
        {
            InitializeComponent();
            Profile = profile;
        }

        public void UpdateDetails(ControllerProfile profile)
        {
            // Create a string with our profile information
            string profileInfo = string.Format(Strings.DetailsProfileInfo,
                profile.LeftStickRange * 100,
                profile.RightStickRange * 100,
                profile.LeftStickDeadzone * 100,
                profile.RightStickDeadzone * 100,
                profile.TriggerDeadzone * 100,
                profile.TriggerThreshold * 100);

            // Set the profile name and details and clip the text to ensure it fits
            profileName.Content = ControlExtensions.ClipTextToBounds(45, profile.ProfileName);
            profileDetails.Content = ControlExtensions.ClipTextToBounds(150, profileInfo);

            if (profile.Favorited)
            {
                // If our profile is in our DefaultProfiles list, set a special icon
                if (UserSettings.Default.DefaultProfiles.Contains(profile.ProfileName))
                {
                    favoriteStatus.Source = new BitmapImage(new Uri(FAVORITE_DEFAULT_ICON_LINK, UriKind.Relative));
                }

                // ...or set the default favorite icon.
                else
                {
                    favoriteStatus.Source = new BitmapImage(new Uri(FAVORITE_ICON_LINK, UriKind.Relative));
                }
            }
                
            else
            {
                favoriteStatus.Source = new BitmapImage(new Uri(NOT_FAVORITE_ICON_LINK, UriKind.Relative));
            }
        }

        private void FavoriteStatus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Reverse the favorited status of the profile
            Profile.Favorited = !Profile.Favorited;

            // Assign profile to itself to update the information
            Profile = Profile;

            // Save the profile to file with the new favorite status.
            ProfileManager.SaveProfile(Profile, Profile.ProfileName);
        }
    }
}
