using System;
using System.Windows;
using System.Windows.Controls;

namespace Delfinovin.Controls
{
    /// <summary>
    /// A ListItem that provides the slider control
    /// with a corresponding label 
    /// </summary>
    public partial class SliderListItem : UserControl
    {
        public string ItemText { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public int Value { get; set; }

        public event EventHandler<RoutedPropertyChangedEventArgs<double>> ValueChanged;

        public SliderListItem()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ValueChanged?.Invoke(this, e);
        }
    }
}
