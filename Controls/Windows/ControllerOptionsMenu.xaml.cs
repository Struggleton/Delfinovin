using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace Delfinovin.Controls.Windows
{
    /// <summary>
    /// Interaction logic for ControllerOptionsMenu.xaml
    /// </summary>
    public partial class ControllerOptionsMenu : Window
    {
        public event EventHandler<OptionSelection> OptionSelected;
        private bool _isClosing;

        public ControllerOptionsMenu()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void ItemSelected(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            OptionSelected.Invoke(this, (OptionSelection)item.Tag);
            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _isClosing = true;
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            if (!_isClosing)
                Close();
        }
    }
}
