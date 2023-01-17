using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Forms = System.Windows.Forms;

namespace Delfinovin.Controls.Windows
{
    /// <summary>
    /// A window to provide the application with functionality to 
    /// provide popup windows and receive input.
    /// </summary>
    public partial class MessageDialog : Window
    {
        public Forms.DialogResult Result;
        public MessageDialog(string text = "", Forms.MessageBoxButtons buttons = Forms.MessageBoxButtons.YesNo)
        {
            InitializeComponent();
            SetButtonVisibility(buttons);

            dialogText.Text = text;
        }

        private void SetButtonVisibility(Forms.MessageBoxButtons buttons)
        {
            // Set the visibility based on the button enum set
            if (buttons == Forms.MessageBoxButtons.YesNo) 
            {
                okayButton.Visibility = Visibility.Collapsed;
                yesButton.Visibility = Visibility.Visible;
                noButton.Visibility = Visibility.Visible;
            }

            else if (buttons == Forms.MessageBoxButtons.OK)
            {
                okayButton.Visibility = Visibility.Visible;
                yesButton.Visibility = Visibility.Collapsed;
                noButton.Visibility = Visibility.Collapsed;
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Get the button from the sender
            Button button = (Button)sender;

            // Cast the tags of the buttons to a dialog result
            Result = Enum.Parse<Forms.DialogResult>(button.Tag.ToString());

            // Close the window
            Close();
        }

        private void MessageDialog_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
