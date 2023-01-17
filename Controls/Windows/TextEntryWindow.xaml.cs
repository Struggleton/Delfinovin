using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Delfinovin.Controls.Windows
{
    /// <summary>
    /// A window to provide the application with an easy way of
    /// receiving text entry from the user.
    /// </summary>
    public partial class TextEntryWindow : Window
    {
        public string EnteredText { get; set; }

        public TextEntryWindow(string displayText = "", string headerText = "", string buttonText = "")
        {
            InitializeComponent();

            textEntry.Text = displayText;
            textWindowTitle.HeaderText = headerText;
            saveButton.Content = buttonText;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the entered text from the textEntry textbox
            EnteredText = textEntry.Text;

            // We're leaving the dialog through the save button, set the result to true.
            DialogResult = true;

            // Close this window.
            Close();
        }

        private void TextEntry_GotFocus(object sender, RoutedEventArgs e)
        {
            // Get the textbox from the sender
            TextBox textBox = (TextBox)sender;

            // Whenever the textbox is focused (i.e clicking on it)
            // Select all of the text in the textbox
            textBox.Dispatcher.BeginInvoke((Action)delegate
            {
                textBox.SelectAll();
            });
        }

        private void TextEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Get the text box from the sender
            TextBox textBox = (TextBox)sender;

            // Get the cursorPosition before we start modifying text
            var cursorPosition = textBox.SelectionStart;

            // Use Regex to restrict the characters that can be typed.
            // To Windows-safe characters.
            textBox.Text = Regex.Replace(textBox.Text, "[^0-9a-zA-Z. ]", "");

            // Set the cursorPosition to the position before we modified anything.
            textBox.SelectionStart = cursorPosition;
        }
    }
}
