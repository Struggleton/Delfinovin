using Delfinovin.Controllers;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;

namespace Delfinovin.Controls.Windows
{
    /// <summary>
    /// A window that allows the user to set
    /// settings on a ControllerProfile
    /// </summary>
    public partial class ControllerSettingWindow : Window
    {
        private const int RUMBLE_DURATION = 1500;

        private ControllerProfile _currentProfile;
        private int _currentPort;
        private GamecubeAdapter _adapter;

        public ControllerSettingWindow(ref GamecubeAdapter adapter, int port)
        {
            InitializeComponent();

            _currentPort = port;
            _currentProfile = ProfileManager.CurrentProfiles[_currentPort];
            _adapter = adapter;
            _adapter.InputFrameProcessed += InputFrameProcessed;

            UpdateControls();
        }

        private void InputFrameProcessed(object? sender, ControllerStatus[] input)
        {
            this.Dispatcher.Invoke(async () =>
            {
                controllerDialog.UpdateDialog(input[_currentPort]);
            });
        }

        private void UpdateControls()
        {
            enableRumble.Checked = _currentProfile.EnableRumble;
            swapControlSticks.Checked = _currentProfile.SwapControlSticks;
            triggerDeadzone.Value = (int)(_currentProfile.TriggerDeadzone * 100);
            triggerThreshold.Value = (int)(_currentProfile.TriggerThreshold * 100);
            leftStickDeadzone.Value = (int)(_currentProfile.LeftStickDeadzone * 100);
            leftStickRange.Value = (int)(_currentProfile.LeftStickRange * 100);
            rightStickDeadzone.Value = (int)(_currentProfile.RightStickDeadzone * 100);
            rightStickRange.Value = (int)(_currentProfile.RightStickRange * 100);

            SetStickTags(swapControlSticks.Checked);
            UpdateDeadzoneDisplay();
        }

        private void UpdateCurrentProfile()
        {
            _currentProfile.EnableRumble = enableRumble.Checked;
            _currentProfile.SwapControlSticks = swapControlSticks.Checked;

            _currentProfile.LeftStickRange = leftStickRange.Value / 100f;
            _currentProfile.LeftStickDeadzone = leftStickDeadzone.Value / 100f;

            _currentProfile.RightStickRange = rightStickRange.Value / 100f;
            _currentProfile.RightStickDeadzone = rightStickDeadzone.Value / 100f;

            _currentProfile.TriggerDeadzone = triggerDeadzone.Value / 100f;
            _currentProfile.TriggerThreshold = triggerThreshold.Value / 100f;
        }

        private void ApplySettings_Click(object sender, RoutedEventArgs e)
        {
            UpdateCurrentProfile();
            DialogResult = true;
            Close();
        }

        private void RestoreDefaults_Click(object sender, RoutedEventArgs e)
        {
            // Prompt the user if they'd like to reset the application settings.
            MessageDialog resetDialog = new MessageDialog(Strings.PromptResetSettings);
            resetDialog.ShowDialog();

            bool restore = resetDialog.Result == Forms.DialogResult.Yes;

            // Exit if they choose no
            if (!restore)
                return;

            // Create a new profile with default settings and
            // update the current profiles list with them.
            _currentProfile = new ControllerProfile();
            ProfileManager.CurrentProfiles[_currentPort] = _currentProfile;

            UpdateControls();

            // Tell the user we've reset the settings.
            resetDialog = new MessageDialog(Strings.NotificationSettingsReset, Forms.MessageBoxButtons.OK);
            resetDialog.ShowDialog();
        }

        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            // Get a name to save the profile to. If nothing was entered stop
            TextEntryWindow textEntry = new TextEntryWindow(Strings.PromptEnterProfileName, Strings.HeaderProfileNameEntry, Strings.Save);
            bool textEntered = (bool)textEntry.ShowDialog();

            if (!textEntered)
                return;

            // Gather all of the values from the controls and
            // update the current profile with it.
            UpdateCurrentProfile();
            ProfileManager.SaveProfile(_currentProfile, textEntry.EnteredText);

            string saveMessage = string.Format(Strings.NotificationProfileSaved, textEntry.EnteredText);
            MessageDialog messageDialog = new MessageDialog(saveMessage, Forms.MessageBoxButtons.OK);
            messageDialog.ShowDialog();
        }

        private void ButtonMapping_Click(object sender, RoutedEventArgs e)
        {
            // Open the buttonMapping window. If the window returns true, update the
            // current profile's button mapping with the one from the window.
            ButtonMappingWindow mappingWindow = new ButtonMappingWindow(_currentProfile.ButtonMapping);
            bool result = (bool)mappingWindow.ShowDialog();

            if (!result) 
                return;

            _currentProfile.ButtonMapping = mappingWindow.ButtonMapping;
        }

        private void StickDeadzoneValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Get the slider item from the sender
            SliderListItem sliderItem = (SliderListItem)sender;

            // Cast the tag value into a stick
            Sticks stick = (Sticks)sliderItem.Tag;

            // Update the deadzone display with the new stick + deadzone value.
            controllerDialog.SetDeadzoneSize((float)(e.NewValue), stick);
        }

        private void SwapSticksToggleValueChanged(object sender, bool swap)
        {
            SetStickTags(swap);
            UpdateDeadzoneDisplay();
        }

        private void UpdateDeadzoneDisplay()
        {
            // Update the deadzone display based on the 
            // slider value and current stick tag applied
            controllerDialog.SetDeadzoneSize(leftStickDeadzone.Value, (Sticks)leftStickDeadzone.Tag);
            controllerDialog.SetDeadzoneSize(rightStickDeadzone.Value, (Sticks)rightStickDeadzone.Tag);
        }

        private void SetStickTags(bool swap)
        {
            leftStickDeadzone.Tag = swap ? Sticks.RStick : Sticks.LStick;
            rightStickDeadzone.Tag = swap ? Sticks.LStick : Sticks.RStick;
        }

        private void RumbleToggleValueChanged(object sender, bool value)
        {
            if (value)
            {
                Task.Run(async () =>
                {
                    _adapter.Controllers[_currentPort].IsVibrating = true;
                    await Task.Delay(RUMBLE_DURATION);

                    _adapter.Controllers[_currentPort].IsVibrating = false;
                });
            }

        }
    }
}
