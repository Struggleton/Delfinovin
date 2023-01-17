using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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

        private string _itemText;
        public string ItemText
        {
            get { return _itemText; }
            set 
            { 
                _itemText = value;
                textDisplay.Content = _itemText;
            }
        }
        
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
