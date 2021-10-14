using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace DelfinovinUI
{
    public partial class GamecubeDialogNew : UserControl
    {
		public GamecubeDialogNew()
		{
			InitializeComponent();
		}

		// Update the dialog based on current settings + controller state
		public void UpdateDialog(GamecubeInputState inputState, ControllerSettings settings)
		{
			bool aButton = (settings.SwapAB ? inputState.BUTTON_B : inputState.BUTTON_A);
			bool bButton = (settings.SwapAB ? inputState.BUTTON_A : inputState.BUTTON_B);
			bool xButton = (settings.SwapXY ? inputState.BUTTON_Y : inputState.BUTTON_X);
			bool yButton = (settings.SwapXY ? inputState.BUTTON_X : inputState.BUTTON_Y);

			ButtonA.Fill = (aButton ? Brushes.Red : Brushes.DarkTurquoise);
			ButtonB.Fill = (bButton ? Brushes.Red : Brushes.Firebrick);
			ButtonX.Fill = (xButton ? Brushes.Red : Brushes.Gainsboro);
			ButtonY.Fill = (yButton ? Brushes.Red : Brushes.Gainsboro);
			ButtonZ.Fill = (inputState.BUTTON_Z ? Brushes.Red : Brushes.MediumBlue);

			ButtonStart.Fill = (inputState.BUTTON_START ? Brushes.Red : Brushes.Gainsboro);
			ButtonDPAD_Left.Fill = (inputState.DPAD_LEFT ? Brushes.Red : Brushes.DimGray);
			ButtonDPAD_Right.Fill = (inputState.DPAD_RIGHT ? Brushes.Red : Brushes.DimGray);
			ButtonDPAD_Up.Fill = (inputState.DPAD_UP ? Brushes.Red : Brushes.DimGray);
			ButtonDPAD_Down.Fill = (inputState.DPAD_DOWN ? Brushes.Red : Brushes.DimGray);

			// Normalize values into triggerdeadzone/threshold range
			byte clampedLeftTrigger = Extensions.ClampTriggers(inputState.ANALOG_LEFT, settings.TriggerDeadzone, settings.TriggerThreshold);
			byte clampedRightTrigger = Extensions.ClampTriggers(inputState.ANALOG_RIGHT, settings.TriggerDeadzone, settings.TriggerThreshold);

			SolidColorBrush leftTriggerColor = new SolidColorBrush(new Color() { R = clampedLeftTrigger, A = 255});
			SolidColorBrush rightTriggerColor = new SolidColorBrush(new Color() { R = clampedRightTrigger, A = 255 });

			LeftTrigger.Fill = (clampedLeftTrigger > 0 ? leftTriggerColor : Brushes.Gainsboro);
			RightTrigger.Fill = (clampedRightTrigger > 0 ? rightTriggerColor : Brushes.Gainsboro);

            int LeftStickX = inputState.LEFT_STICK_X * 200 / 255 + -50;
            int LeftStickY = inputState.LEFT_STICK_Y * 200 / 255 + -300;
            int CStickX = inputState.C_STICK_X * 80 / 255 + 500;
            int CStickY = inputState.C_STICK_Y * 80 / 255 + -295;


			TransformGroup leftTransformGroup = new TransformGroup();
			TransformGroup rightTransformGroup = new TransformGroup();

			leftTransformGroup.Children.Add(new TranslateTransform(LeftStickX, -LeftStickY));
			leftTransformGroup.Children.Add(new ScaleTransform(0.4, 0.4));

			rightTransformGroup.Children.Add(new TranslateTransform(CStickX, -CStickY));
			rightTransformGroup.Children.Add(new ScaleTransform(0.5, 0.5));


			LeftStick.RenderTransform = leftTransformGroup;
			RightStick.RenderTransform = rightTransformGroup;

		}
	}
}
