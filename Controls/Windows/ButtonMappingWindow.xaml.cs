using Delfinovin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Delfinovin.Controls.Windows
{
    /// <summary>
    /// Interaction logic for ButtonMappingWindow.xaml
    /// </summary>
    public partial class ButtonMappingWindow : Window
    {
        private Dictionary<XboxControllerButtons, GamecubeControllerButtons> _buttonMapping;
        public Dictionary<XboxControllerButtons, GamecubeControllerButtons> ButtonMapping 
        { 
            get { return _buttonMapping; } 
            private set { _buttonMapping = value; }
        }

        private List<GamecubeControllerButtons> _currentButtonList;
        private XboxControllerButtons _currentKeyValue;

        public ButtonMappingWindow(Dictionary<XboxControllerButtons, GamecubeControllerButtons> buttonMapping)
        {
            InitializeComponent();

            // Do this so we copy by value not by reference
            _buttonMapping = new Dictionary<XboxControllerButtons, GamecubeControllerButtons>(buttonMapping);

            // Iterate through all the buttons and add any missing values.
            // We can remove the ones that have no value on save
            XboxControllerButtons[] xboxButtons = Enum.GetValues<XboxControllerButtons>();
            foreach (XboxControllerButtons button in xboxButtons) 
            {
                if (!_buttonMapping.ContainsKey(button))
                    _buttonMapping.Add(button, GamecubeControllerButtons.None);
            }

            CreateImageListItems();
        }

        private void CreateImageListItems()
        {
            foreach (var entry in ButtonImagePairs.XboxButtonPairs)
            {
                ImageListItem item = new ImageListItem();
                item.ImageSource = entry.Value.Item2.UriSource.OriginalString;
                item.ItemText = entry.Value.Item1;
                item.Tag = entry.Key;

                // Add the control to the list 
                outputButtonListView.Items.Add(item);
            }

            // Refresh the output mapping display
            UpdateOutputListDisplay();
        }

        private ComboBoxListItem CreateMappingComboBox()
        {
            // Create a combobox item.
            ComboBoxListItem comboItem = new ComboBoxListItem();

            // Add all of the possible buttons to map.
            comboItem.Items = Enum.GetNames(typeof(GamecubeControllerButtons)).ToList();
            
            comboItem.ItemText = Strings.ListItemMapping;

            // Subscribe to the selection changed event for later
            comboItem.SelectionChanged += MappingComboBox_SelectionChanged;

            // Return this combobox item.
            return comboItem;
        }

        private void MappingComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            // Get the combobox item from the sender
            ComboBoxListItem comboItem = sender as ComboBoxListItem;

            // Find the index of this selected 
            int buttonIndex = mappingButtonListView.Items.IndexOf(comboItem);
            if (buttonIndex == -1)
                return;

            // Try to parse our combo box's selection
            if (!Enum.TryParse(comboItem.SelectedItem, true, out GamecubeControllerButtons result))
                return;
            
            // Update the list with the current result
            _currentButtonList[buttonIndex] = result;

            // Update the button mapping dictionary with the
            // button value after converting the list to 
            // a button bitfield.
            _buttonMapping[_currentKeyValue] = ConvertListToButtons(_currentButtonList);
        }


        private void AddMapping_Click(object sender, RoutedEventArgs e)
        {
            // Only continue if we've selected an output button
            if (outputButtonListView.SelectedIndex == -1)
                return;

            // Create a combo box item and add it to the list view.
            ComboBoxListItem comboItem = CreateMappingComboBox();
            mappingButtonListView.Items.Add(comboItem);

            // Add a dummy entry to the list for modification
            _currentButtonList.Add(GamecubeControllerButtons.None);

            // Refresh the output list.
            UpdateOutputListDisplay();
        }

        private void UpdateOutputListDisplay()
        {
            // Iterate through all of the output items
            for (int i = 0; i < outputButtonListView.Items.Count; i++)
            {
                ImageListItem imageItem = (ImageListItem)outputButtonListView.Items[i];

                // Check if the corresponding Dictionary value is equal to none (No buttons are mapped)
                bool hasValue = _buttonMapping[(XboxControllerButtons)imageItem.Tag] != GamecubeControllerButtons.None;

                // If they are not equal to none, set the brush and and tool tip.
                imageItem.Background = hasValue ? Brushes.Transparent : Brushes.IndianRed;
                imageItem.ToolTip = hasValue ? "" : Strings.ToolTipUnmapped;
            }
        }

        private void RemoveMapping_Click(object sender, RoutedEventArgs e)
        {
            // Only continue if there is a input mapping
            // from the mapping ListView selected
            if (mappingButtonListView.SelectedIndex == -1)
                return;

            // Remove the button value from the button list
            _currentButtonList.RemoveAt(mappingButtonListView.SelectedIndex);

            // Remove the combobox from the mapping list view
            mappingButtonListView.Items.RemoveAt(mappingButtonListView.SelectedIndex);

            // Update the buttons with new values
            _buttonMapping[_currentKeyValue] = ConvertListToButtons(_currentButtonList);

            // Refresh the output display
            UpdateOutputListDisplay();
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

        private void MappingButtonListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable the remove button if the selected index is valid.
            removeMapping.IsEnabled = mappingButtonListView.SelectedIndex != -1;
        }

        private void OutputButtonListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get the ListViewItem from the sender
            ListViewItem listViewItem = (ListViewItem)sender;

            // Get the ImageListItem from the content of the listviewitem
            ImageListItem imageListItem = (ImageListItem)listViewItem.Content;

            // Clear the previous mapping ListView
            mappingButtonListView.Items.Clear();

            // Update the selected mapping display
            selectedMappingDisplay.ItemText = imageListItem.ItemText;
            selectedMappingDisplay.ImageSource = imageListItem.ImageSource;

            // We've selected an entry, enable the button to add more button entries.
            addMapping.IsEnabled = true;

            // Update the current key value using the ListItem's tag value
            _currentKeyValue = (XboxControllerButtons)imageListItem.Tag;

            // Set up the current list of controller buttons.
            _currentButtonList = new List<GamecubeControllerButtons>();

            // Get the list of values that 
            List<GamecubeControllerButtons> buttons = Enum.GetValues<GamecubeControllerButtons>()
                                                          .Where(x => _buttonMapping[_currentKeyValue].HasFlag(x) &&
                                                                 x != GamecubeControllerButtons.None)
                                                          .ToList();

            
            foreach (GamecubeControllerButtons button in buttons)
            {
                // Create a combobox control for each button in our mapping entry.
                ComboBoxListItem comboItem = CreateMappingComboBox();

                // Set the combobox's selected index after
                // finding the index of the current button.
                comboItem.SelectedIndex = comboItem.Items.IndexOf(button.ToString());

                // Add the combo box to the mappingList
                // and add the controller button to the
                // of current button values.
                mappingButtonListView.Items.Add(comboItem);
                _currentButtonList.Add(button);
            }

            UpdateOutputListDisplay();
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            // Loop through and remove every key/value
            // with no value set.
            foreach (var entry in _buttonMapping)
            {
                if (entry.Value == GamecubeControllerButtons.None)
                    _buttonMapping.Remove(entry.Key);
            }

            // Set the property ButtonMapping to the 
            // private field and set the dialogResult to true
            ButtonMapping = _buttonMapping;
            DialogResult = true;
        }
    }
}
