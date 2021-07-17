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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DelfinovinnUI
{
    /// <summary>
    /// Interaction logic for GamecubeDialog.xaml
    /// </summary>
    public partial class GamecubeDialog : UserControl
    {
		public GamecubeDialog()
		{
			InitializeComponent();
		}

		public void UpdateDialog(GamecubeInputState inputState, ControllerSettings settings)
		{
			bool aButton = (settings.SwapAB ? inputState.BUTTON_B : inputState.BUTTON_A);
			bool bButton = (settings.SwapAB ? inputState.BUTTON_A : inputState.BUTTON_B);
			bool xButton = (settings.SwapXY ? inputState.BUTTON_Y : inputState.BUTTON_X);
			bool yButton = (settings.SwapXY ? inputState.BUTTON_X : inputState.BUTTON_Y);
			Button_A.Fill = (aButton ? Brushes.Red : Brushes.PaleTurquoise);
			Button_B.Fill = (bButton ? Brushes.Red : Brushes.Firebrick);
			Button_X.Fill = (xButton ? Brushes.Red : Brushes.Gainsboro);
			Button_Y.Fill = (yButton ? Brushes.Red : Brushes.Gainsboro);
			StartButton.Fill = (inputState.BUTTON_START ? Brushes.Red : Brushes.Gainsboro);
			Button_DLeft.Fill = (inputState.DPAD_LEFT ? Brushes.Red : Brushes.Gainsboro);
			Button_DRight.Fill = (inputState.DPAD_RIGHT ? Brushes.Red : Brushes.Gainsboro);
			Button_DUp.Fill = (inputState.DPAD_UP ? Brushes.Red : Brushes.Gainsboro);
			Button_DDown.Fill = (inputState.DPAD_DOWN ? Brushes.Red : Brushes.Gainsboro);

			int LeftStickX = inputState.LEFT_STICK_X * 100 / 255 + -50;
			int LeftStickY = inputState.LEFT_STICK_Y * 100 / 255 + -50;
			int CStickX = inputState.C_STICK_X * 60 / 255 + -30;
			int CStickY = inputState.C_STICK_Y * 60 / 255 + -30;

			LeftStick.RenderTransform = new TranslateTransform(LeftStickX, -LeftStickY);
			CStick.RenderTransform = new TranslateTransform(CStickX, -CStickY);
		}
	}
}
