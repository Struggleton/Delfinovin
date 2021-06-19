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

namespace DelfinovinUI
{
    public partial class ControllerControl : UserControl
    {
        public ControllerControl()
        {
            InitializeComponent();
        }

        public void UpdateDialog(GamecubeInputState inputState)
        {
            this.Button_A.Fill = inputState.BUTTON_A ? Brushes.Red : Brushes.PaleTurquoise;
            this.Button_B.Fill = inputState.BUTTON_B ? Brushes.Red : Brushes.Firebrick;
            this.Button_X.Fill = inputState.BUTTON_X ? Brushes.Red : Brushes.Gainsboro;
            this.Button_Y.Fill = inputState.BUTTON_Y ? Brushes.Red : Brushes.Gainsboro;
            this.StartButton.Fill = inputState.BUTTON_START ? Brushes.Red : Brushes.Gainsboro;
            this.Button_DLeft.Fill = inputState.DPAD_LEFT ? Brushes.Red : Brushes.Gainsboro;
            this.Button_DRight.Fill = inputState.DPAD_RIGHT ? Brushes.Red : Brushes.Gainsboro;
            this.Button_DUp.Fill = inputState.DPAD_UP ? Brushes.Red : Brushes.Gainsboro;
            this.Button_DDown.Fill = inputState.DPAD_DOWN ? Brushes.Red : Brushes.Gainsboro;

            int LeftStickX = ((inputState.LEFT_STICK_X - 0) * 100 / 255) + (-50);
            int LeftStickY = ((inputState.LEFT_STICK_Y - 0) * 100 / 255) + (-50);

            int CStickX = ((inputState.C_STICK_X - 0) * 60 / 255) + (-30);
            int CStickY = ((inputState.C_STICK_Y - 0) * 60 / 255) + (-30);

            this.LeftStick.RenderTransform = new TranslateTransform(LeftStickX, -LeftStickY);
            this.CStick.RenderTransform = new TranslateTransform(CStickX, -CStickY);
        }
    }
}
