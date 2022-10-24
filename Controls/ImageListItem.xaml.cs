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
    /// A UserControl intended to be used to display an image
    /// and text next to each other.
    /// </summary>
    public partial class ImageListItem : UserControl
    {
        private string _imageSource;
        public string ImageSource 
        { 
            get { return _imageSource; }
            set 
            { 
                _imageSource = value;
                UpdateImage();
            } 
        }

        public string ItemText { get; set; }
        
        public ImageListItem()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void UpdateImage()
        {
            imageDisplay.Source = new BitmapImage(new Uri(ImageSource, UriKind.Relative));
        }
    }
}
