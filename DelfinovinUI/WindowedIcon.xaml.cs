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
using System.Windows.Shapes;

namespace DelfinovinUI
{
    /// <summary>
    /// Interaction logic for WindowedIcon.xaml
    /// </summary>
    public partial class WindowedIcon : Window
    {
        public WindowResult Result { get; set; }
        public WindowedIcon()
        {
            InitializeComponent();
            gamecubeDialog.ButtonA.MouseDown += ButtonA_MouseDown;
        }

        private void ButtonA_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new MessageWindow("This is a test for pressing the A button!").ShowDialog();
        }

        // Implement custom header bars
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Result = WindowResult.Closed;
            this.Close();
        }

        private void rectHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
            
        }

        private enum ControllerButtons
        {
            LButton, RButton, ZButton, StartButton,
            AButton, BButton, XButton, YButton,
            DPadLeft, DPadRight, DPadDown, DPadUp
        }
    }
}
