using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace Delfinovin.Controls
{
    public partial class GamecubeDialog : UserControl
    {
		public GamecubeDialog()
		{
			InitializeComponent();
		}

		/*
		// Update the dialog based on current settings + controller state
		public void UpdateDialog(ControllerStatus input, GamecubeCalibration calibration)
		{
			//SetStickPosition(input, calibration);
		}

		public void SetDeadzoneSize(float deadzoneSize, ControllerButtons stick)
        {
			bool isVisible = deadzoneSize > 0;
			if (stick == ControllerButtons.LStick)
            {
				LeftStickDeadzone.Visibility = isVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
				((EllipseGeometry)LeftStickDeadzone.Data).RadiusX = Extensions.Remap(deadzoneSize, 0, 100, 0, 65);
				((EllipseGeometry)LeftStickDeadzone.Data).RadiusY = Extensions.Remap(deadzoneSize, 0, 100, 0, 65);
			}

			else
            {
				RightStickDeadzone.Visibility = isVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
				((EllipseGeometry)RightStickDeadzone.Data).RadiusX = Extensions.Remap(deadzoneSize, 0, 100, 0, 60);
				((EllipseGeometry)RightStickDeadzone.Data).RadiusY = Extensions.Remap(deadzoneSize, 0, 100, 0, 60);
			}
		}

		private void SetStickPosition(ControllerStatus input, GamecubeCalibration calibration)
        {
			TransformGroup leftStickRender = LeftStick.RenderTransform as TransformGroup;

			// Recenter the stick inputs based on the calibration
			int LeftStickX = (input.LEFT_STICK_X + 127 - calibration.StickOrigins[0]);
			int LeftStickY = (input.LEFT_STICK_Y + 127 - calibration.StickOrigins[1]);

			// Scale the stick input to the UI pixels
			float newTranslationX = Extensions.Remap(LeftStickX, 0, 255, 0, 50) - 25; // 25s are to offset the stick pos
			float newTranslationY = Extensions.Remap(LeftStickY, 0, 255, 0, 50) - 25;

			// Update the current translation
			leftStickRender.Children[2] = new TranslateTransform(newTranslationX, -newTranslationY);

			// Same process for the right stick
			TransformGroup rightStickRender = RightStick.RenderTransform as TransformGroup;

			int RightStickX = (input.C_STICK_X + 127 - calibration.StickOrigins[2]);
			int RightStickY = (input.C_STICK_Y + 127 - calibration.StickOrigins[3]);

			newTranslationX = Extensions.Remap(RightStickX, 0, 255, 0, 70) - 35;
			newTranslationY = Extensions.Remap(RightStickY, 0, 255, 0, 70) - 35;

			rightStickRender.Children[2] = new TranslateTransform(newTranslationX, -newTranslationY);
		}
		*/
	}
}
