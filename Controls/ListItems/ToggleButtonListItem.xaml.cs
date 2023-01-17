using System;
using System.Windows;
using System.Windows.Controls;

namespace Delfinovin.Controls
{
    /// <summary>
    /// A ListItem that provides the ToggleButton control
    /// with a corresponding label 
    /// </summary>
    public partial class ToggleButtonListItem : UserControl
    {
        public string ItemText { get; set; }
        public bool Checked { get; set; }

        public event EventHandler<bool> ToggleValueChanged;

        public ToggleButtonListItem()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void ValueChanged(object sender, RoutedEventArgs e)
        {
            ToggleValueChanged?.Invoke(this, (bool)settingButton.IsChecked);
        }
    }
}
