using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Delfinovin.Controls
{
    /// <summary>
    /// Provide a ListItem that displays a combobox
    /// with a corresponding label.
    /// </summary>

    public partial class ComboBoxListItem : UserControl
    {
        public List<string> Items { get; set; } = new List<string>();
        public string ItemText { get; set; }
        public int SelectedIndex { get; set; } = -1;
        public string SelectedItem { get; set; }

        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        public ComboBoxListItem()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
    }
}
