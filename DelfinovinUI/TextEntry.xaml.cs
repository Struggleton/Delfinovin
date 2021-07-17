using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DelfinovinUI
{
    /// <summary>
    /// Interaction logic for TextEntry.xaml
    /// </summary>
    public partial class TextEntry : Window
    {
		public string EnteredText { get; set; }
		public WindowResult Result { get; set; }

		public TextEntry(string displayText)
		{
			InitializeComponent();
			txbTextEntry.Text = displayText;
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

		private void txbNameEntry_GotFocus(object sender, RoutedEventArgs e)
		{
			// Use dispatcher to select all text when in focus
			TextBox textBox = (TextBox)sender;
			textBox.Dispatcher.BeginInvoke((Action)delegate
			{
				textBox.SelectAll();
			});
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			EnteredText = txbTextEntry.Text;
			Result = WindowResult.SaveClosed;

			this.Close();
		}
	}

	public enum WindowResult
	{
		SaveClosed,
		Closed,
		OK,
		Cancel
	}
}
