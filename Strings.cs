namespace Delfinovin
{
    public class Strings
    {
        public const string LinkGithub = "https://www.github.com/Struggleton/Delfinovin";
        public const string LinkPatreon = "https://www.patreon.com/Struggleton";
        public const string LinkKoFi = "https://www.ko-fi.com/Struggleton";
        public const string LinkPaypal = "https://www.paypal.com/paypalme/Struggleton";

        public const string NavigationHome = "Home";
        public const string NavigationProfiles = "Profiles";
        public const string NavigationPlayback = "Playback Recording";
        public const string NavigationSupport = "Donate/Support";
        public const string NavigationFAQ = "FAQ";
        public const string NavigationSettings = "Settings";


        public const string PromptCalibrationInstructions = "After pressing the OK button, rotate both sticks for " +
                                                            "5 seconds, ensuring the sticks reach all of the edges.";
        public const string PromptUpdateAvailable = "There is an update available - Would you like to download and " +
                                                    "restart Delfinovin?";
        public const string PromptResetSettings = "Would you like to reset the application settings to default?";
        public const string PromptEnterProfileName = "Enter a profile name.";
        public const string PromptDeleteProfile = "Are you sure you'd like to delete this profile ({0})?";
        public const string PromptDeleteRecording = "Are you sure you'd like to delete this recording ({0})?";
        public const string PromptOverwriteRecording = "There is currently a recording stored. Do you want to start a new one?";
        public const string PromptClearRecording = "There is currently a recording stored. Do you want to clear it?";
        public const string PromptEnterRecordingName = "Choose a name for the recording.";
        public const string PromptInstallViGEm = "ViGEmBus is not installed. Would you like to install it now?";

        public const string NotificationCalibrationComplete = "Calibration complete.";
        public const string NotificationProfileApplied = "Applied {0} to port {1}!";
        public const string NotificationNoUpdate = "No updates are available.";
        public const string NotificationSettingsReset = "Application settings reset.";
        public const string NotificationProfileSaved = "Saved {0} to a profile!";
        public const string NotificationProfileFavorited = "This selected profile is favorited. " +
                                                           "Unfavorite it first before deleting it.";
        public const string NotificationProfileDeleted = "Profile deleted.";
        public const string NotificationRecordingDeleted = "Recording deleted.";
        public const string NotificationRecordingCleared = "Stored recording cleared.";
        public const string NotificationRecordingSaved = "Saved {0} to a recording!";

        public const string ErrorDownloadFailed = "An error occurred! Check the internet connection.";
        public const string ErrorMagicNotFound = "Error! Gamecube magic header not found!";


        public const string DetailsProfileInfo = "Left Stick Range/Deadzone: {0}%/{2}%, Right Stick Range/Deadzone: {1}%/{3}%";
        public const string DetailsDefaultName = "ProfileName";
        public const string DetailsDefaultInfo = "Profile Info";
        public const string DetailsControllerPort = "Controller {0}";
        public const string DetailsGithubSupport = "This software is open-source. If there are any issues, " +
            "comments or features you'd like to see, " +
            "please check the Github repository! Contributions are welcome!";
        public const string DetailsDonationSupport = "All of this application's development is done in my free time. " +
            "If you'd like to see continued support or to help me out, " +
            "please consider donating!";
        public const string DetailsApplicationDescription = "An XInput solution for Gamecube Controllers";

        public const string HeaderMainWindowTitle = "Delfinovin - Gamecube Controller Application";
        public const string HeaderControllerOptions = "Delfinovin - Controller Options";
        public const string HeaderInputDisplay = "Delfinovin - Input Display";
        public const string HeaderInputRecording = "Delfinovin - Input Recording";
        public const string HeaderApplicationSettings = "Delfinovin - Application Settings";
        public const string HeaderControllerSettings = "Delfinovin - Controller Settings";
        public const string HeaderProfileNameEntry = "Delfinovin - Profile Name Entry";
        public const string HeaderProgressWindow = "Delfinovin - Download Progress";
        public const string HeaderHotkeyMapping = "Delfinovin - Hotkey Mapping";
        public const string HeaderButtonMapping = "Delfinovin - Button Mapping";
        public const string HeaderThemeSelector = "Delfinovin - Theme Selector";
        public const string HeaderTextEntry = "Delfinovin - Text Entry";
        public const string HeaderMessagePrompt = "Delfinovin - Message Prompt";
        public const string HeaderVersionText = "[by @Struggleton, v{0}.{1}.{2}]";
        public const string HeaderGithub = "Support the project on Github!";
        public const string HeaderDonation = "Support the project by donating!";


        public const string ToastHeaderNoRecordingSelected = "Delfinovin - No Recording Selected";
        public const string ToastHeaderNoRecordingSession = "Delfinovin - No Recording Session";
        public const string ToastHeaderPlaybackInProgress = "Delfinovin - Playback in Progress";
        public const string ToastHeaderRecordingInProgress = "Delfinovin - Recording in Progress";
        public const string ToastHeaderPlaybackBegining = "Delfinovin - Beginning Playback";
        public const string ToastHeaderRecordingStarting = "Delfinovin - Starting Recording";
        public const string ToastHeaderRecordingStopping = "Delfinovin - Stopping Recording";
        public const string ToastHeaderCalibrationBeginning = "Delfinovin - Beginning Calibration";
        public const string ToastHeaderCalibrationComplete = "Delfinovin - Calibration Complete";

        public const string ToastRecordingNotSelected = "There is no recording selected!";
        public const string ToastStopCurrentPlayback = "A playback is currently in progress. Stop it first before playing a second one.";
        public const string ToastPlaybackBeginning = "Beginning recording playback.";
        public const string ToastNoRecordingStop = "There is no recording session to stop!";
        public const string ToastPlaybackInProgress = "A playback session is in progress. Stop the current playback before recording.";
        public const string ToastStopCurrentRecording = "There is a recording currently in progress. " +
                                                       "Stop the current one before starting a new one.";
        public const string ToastRecordingStarting = "Beginning recording! Stop recording by pressing the recording button again.";
        public const string ToastRecordingStopping = "Stopping recording. Go to the Playback tab to save this recording to a file.";
        public const string ToastRecordingInProgress = "A recording is currently in progress. A recording cannot be currently be played.";
        public const string ToastCalibrationBeginning = "Beginning calibration. Rotate both sticks for 5 seconds, " +
                                                        "ensuring the sticks reach all of the edges.";

        public const string MenuItemEditController = "Edit Controller Settings";
        public const string MenuItemCalibrateSticks = "Calibrate Control Sticks";
        public const string MenuItemOpenViewer = "Open Input Viewer";
        public const string MenuItemSaveRecording = "Save Recording";
        public const string MenuItemClearRecording = "Clear Recording";
        public const string MenuItemStoredRecording = "Stored Recording ({0})";
        public const string MenuItemController1 = "Controller 1";
        public const string MenuItemController2 = "Controller 2";
        public const string MenuItemController3 = "Controller 3";
        public const string MenuItemController4 = "Controller 4";
        public const string MenuItemController = "Controller {0}";

        public const string ToolTipGreenscreen = "Enable/Disable Green Screen";
        public const string ToolTipAddMapping = "Add Mapping";
        public const string ToolTipRemoveMapping = "Remove Mapping";
        public const string ToolTipUnmapped = "This button is unmapped.";
        public const string ToolTipHotkey = "Hotkey Settings";

        public const string ListItemDefaultProfile = $"Default Profile - Controller #";
        public const string ListItemStartCalibration = "Start Calibration";
        public const string ListItemStopRecording = "Stop Recording";
        public const string ListItemStartRecording = "Start Recording";
        public const string ListItemBeginPlayback = "Begin Playback";
        public const string ListItemStopPlayback = "Stop Playback";
        public const string ListItemNoRecordingStored = "No Recording Stored";
        public const string ListItemRecordingInProgress = "Recording in progress...";
        public const string ListItemMapping = "Mapping: ";

        public const string SettingEnableRumble = "Enable Rumble";
        public const string SettingSwapSticks = "Swap Control Sticks";
        public const string SettingTriggerDeadzone = "Trigger Deadzone";
        public const string SettingTriggerThreshold = "Trigger Threshold";
        public const string SettingLeftStickRange = "Left Stick Range";
        public const string SettingLeftStickDeadzone = "Left Stick Deadzone";
        public const string SettingRightStickRange = "Right Stick Range";
        public const string SettingRightStickDeadzone = "Right Stick Deadzone";

        public const string SettingCheckForUpdates = "Check for Updates on Startup";
        public const string SettingMinimizeToTray = "Minimize Program to System Tray";
        public const string SettingMinimizeOnStartup = "Minimize Application on Startup";
        public const string SettingRunOnStartup = "Run Application on PC Startup";
        public const string SettingDontShowAgain = "Don't show this dialog on startup again.";

        public const string RestoreDefaults = "Restore Defaults";
        public const string ApplicationTheme = "Application Theme";
        public const string SelectProfile = "Select Profile";
        public const string DeleteProfile = "Delete Profile";
        public const string ControllerColor = "Controller Color";
        public const string SaveProfile = "Save Profile";
        public const string SaveTheme = "Save Theme";
        public const string SaveSettings = "Save Settings";
        public const string ApplySettings = "Apply Settings";
        public const string ButtonMapping = "Button Mapping";
        public const string SelectTheme = "Select Theme";
        public const string CheckForUpdates = "Check for Updates";
        public const string Open = "Open";
        public const string Close = "Close";
        
        public const string Delete = "Delete";
        public const string OK = "OK";
        public const string Yes = "Yes";
        public const string No = "No";
        public const string Save = "Save";
        public const string Github = "Github";
        public const string Paypal = "Paypal";
        public const string KoFi = "Ko-Fi";
        public const string Patreon = "Patreon";
    }
}
