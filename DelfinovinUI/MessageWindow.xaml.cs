using System.Windows;
using System.Windows.Input;

namespace DelfinovinUI
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

			// Set control visibility based on parameters set
			btnLeft.Visibility = ((!displayLeft) ? Visibility.Hidden : Visibility.Visible);
			btnRight.Visibility = ((!displayRight) ? Visibility.Hidden : Visibility.Visible);
			btnLeft.Content = leftText;
			btnRight.Content = rightText;
			lblText.Text = text;
		}

		// Implement custom header bars
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
