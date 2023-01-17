using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UserSettings = Delfinovin.Properties.Settings;

namespace Delfinovin.Controls.Windows
{
    /// <summary>
    /// Interaction logic for HotkeyMappingWindow.xaml
    /// </summary>
    public partial class HotkeyMappingWindow : Window
    {
        private List<GamecubeControllerButtons> _currentButtons;
        private HotkeySelection _currentSelectedHotkey;

        public HotkeyMappingWindow()
        {
            InitializeComponent();
        }

        private void UpdateSelectedHotkey(HotkeySelection hotkeySelected)
        {
            GamecubeControllerButtons newButtons = ConvertListToButtons(_currentButtons);
            switch (hotkeySelected)
            {
                case HotkeySelection.CalibrationHotkey:
                    UserSettings.Default.CalibrationHotkey = (long)newButtons;
                    break;
                case HotkeySelection.StartRecording:
                    UserSettings.Default.StartRecordingHotkey = (long)newButtons;
                    break;
                case HotkeySelection.StopRecording:
                    UserSettings.Default.StopRecordingHotkey = (long)newButtons;
                    break;
                case HotkeySelection.BeginPlayback:
                    UserSettings.Default.BeginPlaybackHotkey = (long)newButtons;
                    break;
                case HotkeySelection.StopPlayback:
                    UserSettings.Default.StopPlaybackHotkey = (long)newButtons;
                    break;
            }
        }

        private GamecubeControllerButtons ConvertListToButtons(List<GamecubeControllerButtons> buttonList)
        {
            // Set base button to bitwise OR values to
            GamecubeControllerButtons buttons = GamecubeControllerButtons.None;

            // For each value in the list, bitwise OR to the button list
            foreach (GamecubeControllerButtons button in buttonList)
                buttons |= button;

            // Return the new Gamecube button bitwise field
            return buttons;
        }

        private GamecubeControllerButtons GetCurrentHotkey(HotkeySelection hotkeySelected)
        {
            GamecubeControllerButtons setButtons = GamecubeControllerButtons.None;
            switch (hotkeySelected)
            {
                case HotkeySelection.StartRecording:
                    setButtons = (GamecubeControllerButtons)UserSettings.Default.StartRecordingHotkey;
                    break;
                case HotkeySelection.StopRecording:
                    setButtons = (GamecubeControllerButtons)UserSettings.Default.StopRecordingHotkey;
                    break;
                case HotkeySelection.CalibrationHotkey:
                    setButtons = (GamecubeControllerButtons)UserSettings.Default.CalibrationHotkey;
                    break;
                case HotkeySelection.BeginPlayback:
                    setButtons = (GamecubeControllerButtons)UserSettings.Default.BeginPlaybackHotkey;
                    break;
                case HotkeySelection.StopPlayback:
                    setButtons = (GamecubeControllerButtons)UserSettings.Default.StopPlaybackHotkey;
                    break;
            }

            return setButtons;
        }

        private ComboBoxListItem CreateMappingComboBox()
        {
            // Create a combobox item.
            ComboBoxListItem comboItem = new ComboBoxListItem();

            // Add all of the possible buttons to map.
            comboItem.Items = Enum.GetNames(typeof(GamecubeControllerButtons)).ToList();

            // Subscribe to the selection changed event for later
            comboItem.SelectionChanged += MappingComboBox_SelectionChanged;


            comboItem.ItemText = Strings.ListItemMapping;

            // Assign a controller tag to the control
            comboItem.Tag = GamecubeControllerButtons.None;

            // Return this combobox item.
            return comboItem;
        }


        private void AddMapping_Click(object sender, RoutedEventArgs e)
        {
            // Only continue if we've selected an output button
            if (hotkeyListView.SelectedIndex == -1)
                return;

            // Create a combo box item and add it to the list view.
            ComboBoxListItem comboItem = CreateMappingComboBox();
            buttonListView.Items.Add(comboItem);

            // Add a dummy entry to the list for modification
            _currentButtons.Add(GamecubeControllerButtons.None);
        }

        private void RemoveMapping_Click(object sender, RoutedEventArgs e)
        {
            // Only continue if there is a input button selected
            if (buttonListView.SelectedIndex == -1)
                return;

            // Remove the button the button list
            _currentButtons.RemoveAt(buttonListView.SelectedIndex);

            // Remove the control from the input button controls
            buttonListView.Items.RemoveAt(buttonListView.SelectedIndex);

            UpdateSelectedHotkey(_currentSelectedHotkey);
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            // Save the settings to application settings
            UserSettings.Default.Save();

            // Close the window
            Close();
        }

        private void HotkeyListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            ListViewItem hotkeyListItem = (ListViewItem)sender;
            _currentSelectedHotkey = (HotkeySelection)hotkeyListItem.Tag;

            GamecubeControllerButtons setButtons = GetCurrentHotkey(_currentSelectedHotkey);

            selectedHotkeyDisplay.Content = hotkeyListItem.Content;

            _currentButtons = new List<GamecubeControllerButtons>();

            // We've selected an entry, enable the button to add more button entries.
            addMapping.IsEnabled = true;

            buttonListView.Items.Clear();

            List <GamecubeControllerButtons> buttons = Enum.GetValues<GamecubeControllerButtons>().
                                                            Where(x => setButtons.HasFlag(x) &&
                                                                 x != GamecubeControllerButtons.None).
                                                            ToList();

            foreach (GamecubeControllerButtons button in buttons)
            {
                ComboBoxListItem comboItem = CreateMappingComboBox();

                // Set the combobox's selected index after finding the index of the 
                // current button.
                comboItem.SelectedIndex = comboItem.Items.IndexOf(button.ToString());

                // Add the combo box to the list
                buttonListView.Items.Add(comboItem);
                _currentButtons.Add(button);
            }
        }

        
        private void MappingComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            // Get the combobox item from the sender
            ComboBoxListItem comboItem = (ComboBoxListItem)sender;

            // Find the index of this selected item
            int buttonIndex = buttonListView.Items.IndexOf(comboItem);
            if (buttonIndex == -1)
                return;

            // Try to parse our combo box's selection
            if (!Enum.TryParse(comboItem.SelectedItem, true, out GamecubeControllerButtons result))
                return;

            // Store the current button in our button list
            _currentButtons[buttonIndex] = result;

            // Convert our button list to a single bitflag enum
            // and update the hotkey setting.
            UpdateSelectedHotkey(_currentSelectedHotkey);
        }

        private void ButtonListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable the remove hotkey button if the selected index is valid.
            removeMapping.IsEnabled = buttonListView.SelectedIndex != -1;
        }
    }
}
