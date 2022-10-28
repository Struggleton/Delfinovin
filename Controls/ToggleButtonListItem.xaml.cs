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
    /// Interaction logic for ToggleButtonListItem.xaml
    /// </summary>
    public partial class ToggleButtonListItem : UserControl
    {
        public string ItemText { get; set; }
        public bool Checked { get; set; }

        public ToggleButtonListItem()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
