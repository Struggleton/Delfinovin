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

namespace DelfinovinnUI
{
    /// <summary>
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
		public WindowResult Result { get; set; }

		public MessageWindow(string text = "", bool displayLeft = false, bool displayRight = false, string leftText = "OK", string rightText = "Cancel")
		{
			InitializeComponent();
			btnLeft.Visibility = ((!displayLeft) ? Visibility.Hidden : Visibility.Visible);
			btnRight.Visibility = ((!displayRight) ? Visibility.Hidden : Visibility.Visible);
			btnLeft.Content = leftText;
			btnRight.Content = rightText;
			lblText.Text = text;
		}

		private void rectHeader_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				this.DragMove();
		}

		private void btnLeft_Click(object sender, RoutedEventArgs e)
		{
			Result = WindowResult.OK;
			this.Close();
		}

		private void btnRight_Click(object sender, RoutedEventArgs e)
		{
			Result = WindowResult.Cancel;
			this.Close();
		}

		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			Result = WindowResult.Closed;
			this.Close();
		}
	}
}
