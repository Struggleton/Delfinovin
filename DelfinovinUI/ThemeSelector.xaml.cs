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
using System.Windows.Shapes;

namespace DelfinovinUI
{
    /// <summary>
    /// Interaction logic for ThemeSelector.xaml
    /// </summary>
    public partial class ThemeSelector : Window
    {
        public WindowResult Result;

        public ThemeSelector()
        {
            InitializeComponent();

			cmbControllerColor.ItemsSource = Enum.GetValues(typeof(ControllerColor));
		}

        // Implement custom header bars
		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			Result = WindowResult.Closed;
			this.Close();
		}

		// Implement custom header bars
		private void rectHeader_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				this.DragMove();
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
			{
				Source = new Uri("/DelfinovinUI;component/Themes/Skyline.xaml", UriKind.Relative)
			};

			ControllerColor controllerColor = (ControllerColor)Enum.Parse(typeof(ControllerColor), cmbControllerColor.SelectedItem.ToString());
			uint hexCode = (uint)controllerColor;
			byte[] bytes = BitConverter.GetBytes(hexCode);

			App.Current.Resources["ControllerColor"] = new SolidColorBrush(Color.FromArgb(255, bytes[2], bytes[1], bytes[0]));
			Result = WindowResult.SaveClosed;
			Close();
		}

		public enum ControllerColor : uint
		{
			Indigo = 0xFF3C3760,
			JetBlack = 0x514F50,
			SpiceOrange = 0xCB7017,
			Platinum = 0xA09FA5,
			EmeraldBlue = 0x0E8999,
			White = 0xBDB8BC,
			StarlightGold = 0x988350
		}
	}
}
