using Delfinovin.Controllers;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;

namespace Delfinovin.Controls.Windows
{
    /// <summary>
    /// Interaction logic for InputDisplayWindow.xaml
    /// </summary>
    public partial class InputDisplayWindow : Window
    {
        private ControllerStatus _previousInput;
        private ControllerStatus _currentInput;
        private bool _greenScreenEnabled;
        private int _controllerPort;

        public InputDisplayWindow(ref GamecubeAdapter adapter, int controllerPort)
        {
            InitializeComponent();

            // Store our current controller port.
            _controllerPort = controllerPort;

            // Subscribe to the CollectionChanged event on our input list so we can manipulate it
            ((INotifyCollectionChanged)inputList.Items).CollectionChanged += CollectionChanged;

            // Subscribe to the input processed event
            adapter.InputFrameProcessed += InputFrameProcessed;
        }

        private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (inputList.Items.Count > 0)
            {
                // Set the selected index to the latest number
                // and scroll with the input list.
                inputList.SelectedIndex = inputList.Items.Count - 1;
                inputList.ScrollIntoView(inputList.SelectedItem);

                // If the count of input items exceeds 50,
                // clear it so we don't store too many
                if (inputList.Items.Count > 50)
                    inputList.Items.Clear();
            }
        }

        private void InputFrameProcessed(object? sender, ControllerStatus[] inputs)
        {
            this.Dispatcher.Invoke(async () =>
            {
                // Get the current input from the event args
                _currentInput = inputs[_controllerPort];

                // Update our controller dialog
                controllerDialog.UpdateDialog(_currentInput);

                // Check if the previous update is the same. If so do nothing
                if (!_previousInput.IsButtonsEqual(_currentInput))
                {
                    // Check if any buttons are being pressed. If not, display nothing
                    if (_currentInput.Buttons == GamecubeControllerButtons.None)
                        return;

                    // Extract the buttons to a list and pass it to a new InputListItem
                    List<GamecubeControllerButtons> buttonsPressed = _currentInput.GetButtonsPressed();

                    // Remove the digital inputs if they exist so we don't have
                    // both the analog and digital press displayed
                    if (buttonsPressed.Contains(GamecubeControllerButtons.L))
                        buttonsPressed.Remove(GamecubeControllerButtons.L);

                    if (buttonsPressed.Contains(GamecubeControllerButtons.R))
                        buttonsPressed.Remove(GamecubeControllerButtons.R);

                    if (buttonsPressed.Count > 0)
                    {
                        InputListItem inputListItem = new InputListItem(buttonsPressed);
                        inputList.Items.Add(inputListItem);
                    }
                }
            });

            // Update the previous input with the current one.
            _previousInput = _currentInput;
        }


        private void DisplayGreenScreen_Click(object sender, RoutedEventArgs e)
        {
            // Reverse the boolean for enabling the green screen
            _greenScreenEnabled = !_greenScreenEnabled;

            // Update our Green screen overlay and change the green
            // screen button background.
            greenScreenOverlay.Visibility = _greenScreenEnabled ? Visibility.Visible : Visibility.Collapsed;
            displayGreenScreen.Background = _greenScreenEnabled ? Brushes.DarkSlateBlue : Brushes.Green;
        }
    }
}
