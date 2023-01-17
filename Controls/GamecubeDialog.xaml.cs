using Delfinovin.Controllers;
using System.Windows.Controls;
using System.Windows.Media;

namespace Delfinovin.Controls
{
    /// <summary>
    /// A user control displaying a Gamecube Controller and provides functions to 
    /// update said display with controller inputs.
    /// </summary>
    public partial class GamecubeDialog : UserControl
    {
		public GamecubeDialog()
		{
			InitializeComponent();
		}

		/// <summary>
        /// Update this control with input from a controller.
        /// </summary>
        /// <param name="input">The controller input to send to the Gamecube dialog.</param>
        public void UpdateDialog(ControllerStatus input)
		{
            // Update the dialog based on current settings + controller state
            SetStickPosition(input);
            SetButtons(input);
        }

		private void SetButtons(ControllerStatus input)
		{
            ButtonA.Fill = (input.IsButtonPressed(GamecubeControllerButtons.A) ? Brushes.Red : Brushes.DarkTurquoise);
            ButtonB.Fill = (input.IsButtonPressed(GamecubeControllerButtons.B) ? Brushes.Red : Brushes.Firebrick);
            ButtonX.Fill = (input.IsButtonPressed(GamecubeControllerButtons.X) ? Brushes.Red : Brushes.Gainsboro);
            ButtonY.Fill = (input.IsButtonPressed(GamecubeControllerButtons.Y) ? Brushes.Red : Brushes.Gainsboro);
            ButtonZ.Fill = (input.IsButtonPressed(GamecubeControllerButtons.Z) ? Brushes.Red : Brushes.MediumBlue);
            ButtonStart.Fill = (input.IsButtonPressed(GamecubeControllerButtons.Start) ? Brushes.Red : Brushes.Gainsboro);

            ButtonDPAD_Left.Fill = (input.IsButtonPressed(GamecubeControllerButtons.DpadLeft) ? Brushes.Red : Brushes.DimGray);
            ButtonDPAD_Right.Fill = (input.IsButtonPressed(GamecubeControllerButtons.DpadRight) ? Brushes.Red : Brushes.DimGray);
            ButtonDPAD_Up.Fill = (input.IsButtonPressed(GamecubeControllerButtons.DpadUp) ? Brushes.Red : Brushes.DimGray);
            ButtonDPAD_Down.Fill = (input.IsButtonPressed(GamecubeControllerButtons.DpadDown) ? Brushes.Red : Brushes.DimGray);

            
            byte leftTrigger = (byte)(input.Triggers.X > (255f * 0.15f) ? (byte)input.Triggers.X : 0);
            byte rightTrigger = (byte)(input.Triggers.Y > (255f * 0.15f) ? (byte)input.Triggers.Y : 0);

            // Convert the current trigger press strength into a byte value
            // and use it for the red element
            SolidColorBrush leftTriggerColor = new SolidColorBrush(new Color() { R = leftTrigger, A = 255 });
            SolidColorBrush rightTriggerColor = new SolidColorBrush(new Color() { R = rightTrigger, A = 255 });

            // Conditional if above 0, means it is getting pressed - use the brush
            // if 0, not getting pressed, leave it as the default color
            LeftTrigger.Fill = (input.Triggers.X > 0 ? leftTriggerColor : Brushes.Gainsboro);
            RightTrigger.Fill = (input.Triggers.Y > 0 ? rightTriggerColor : Brushes.Gainsboro);
        }

		private void SetStickPosition(ControllerStatus input)
        {
			TransformGroup leftStickRender = LeftStick.RenderTransform as TransformGroup;

			// Scale the stick input to the UI pixels
			float newTranslationX = Extensions.Remap(input.LStick.X, 0, 255, 0, 50) - 25; // 25s are to offset the stick pos
			float newTranslationY = Extensions.Remap(input.LStick.Y, 0, 255, 0, 50) - 25;

			// Update the current translation
			leftStickRender.Children[2] = new TranslateTransform(newTranslationX, -newTranslationY);

			// Same process for the right stick
			TransformGroup rightStickRender = RightStick.RenderTransform as TransformGroup;

			newTranslationX = Extensions.Remap(input.RStick.X, 0, 255, 0, 70) - 35;
			newTranslationY = Extensions.Remap(input.RStick.Y, 0, 255, 0, 70) - 35;

			rightStickRender.Children[2] = new TranslateTransform(newTranslationX, -newTranslationY);
		}

        public void SetDeadzoneSize(float deadzoneSize, Sticks stick)
        {
            bool isVisible = deadzoneSize > 0;
            if (stick == Sticks.LStick)
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
    }
}
