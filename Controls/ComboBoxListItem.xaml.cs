using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Delfinovin.Controls
{
    /// <summary>
    /// Interaction logic for ComboBoxListItem.xaml
    /// </summary>
    /// 
    public partial class ComboBoxListItem : UserControl
    {
        public List<string> Items { get; set; } = new List<string>();
        public string ItemText { get; set; }
        public int SelectedIndex { get; set; } = -1;
        public string SelectedItem { get; set; }
        

        public ComboBoxListItem()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
