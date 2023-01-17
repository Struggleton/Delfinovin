using Delfinovin.Controllers;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Delfinovin.Controls
{
    /// <summary>
    /// A ListItem to display button icons based on Gamecube
    /// Controller buttons.
    /// </summary>
    public partial class InputListItem : UserControl
    {
        public InputListItem(List<GamecubeControllerButtons> buttons)
        {
            InitializeComponent();
            UpdateButtonDisplay(buttons);
        }

        private void UpdateButtonDisplay(List<GamecubeControllerButtons> buttons)
        {
            foreach (GamecubeControllerButtons button in buttons)
            {
                buttonIcons.Children.Add(new Image()
                {
                    Source = ButtonImagePairs.GamecubeButtonPairs[button].Item2
                });
            }
        }
    }
}
