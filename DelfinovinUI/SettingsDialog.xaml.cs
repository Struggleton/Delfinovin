using System.Windows.Controls;

namespace DelfinovinUI
{
    /// <summary>
    /// Interaction logic for SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : UserControl
    {

		public SettingsDialog()
		{
			InitializeComponent();
		}

		public ControllerSettings GetSettings()
		{
			// Gather all settings set from the dialog
			// and return them
			return new ControllerSettings
			{
				EnableDigitalPress = enableDigitalTriggers.IsChecked.Value,
				EnableRumble = enableRumble.IsChecked.Value,
				LeftStickDeadzone = (float)(leftStickDeadzone.Value / 100.0), 
				LeftStickRange = (float)(leftStickRange.Value / 100.0),
				RightStickDeadzone = (float)(rightStickDeadzone.Value / 100.0),
				RightStickRange = (float)(rightStickRange.Value / 100.0),
				SwapAB = swapAB.IsChecked.Value,
				SwapXY = swapXY.IsChecked.Value,
				TriggerDeadzone = (float)(triggerDeadzone.Value / 100.0),
				TriggerThreshold = (float)(triggerThreshold.Value / 100.0)
			};
		}

		public void UpdateControl(ControllerSettings controllerSettings)
		{
			// Update the dialog based on the controller profile loaded
			enableDigitalTriggers.IsChecked = controllerSettings.EnableDigitalPress;
			enableRumble.IsChecked = controllerSettings.EnableRumble;
			leftStickDeadzone.Value = controllerSettings.LeftStickDeadzone * 100f;
			leftStickRange.Value = controllerSettings.LeftStickRange * 100f;
			rightStickDeadzone.Value = controllerSettings.RightStickDeadzone * 100f;
			rightStickRange.Value = controllerSettings.RightStickRange * 100f;
			swapAB.IsChecked = controllerSettings.SwapAB;
			swapXY.IsChecked = controllerSettings.SwapXY;
			triggerDeadzone.Value = controllerSettings.TriggerDeadzone * 100f;
			triggerThreshold.Value = controllerSettings.TriggerThreshold * 100f;
		}
	}
}
