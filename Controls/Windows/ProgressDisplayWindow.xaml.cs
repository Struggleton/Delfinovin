using System.Windows;

namespace Delfinovin.Controls.Windows
{
    /// <summary>
    /// A simple window to provide progress updates
    /// (i.e Downloading files/updates)
    /// </summary>
    public partial class ProgressDisplayWindow : Window
    {
        public int Progress { get; set; }

        public ProgressDisplayWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
