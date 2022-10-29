using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Delfinovin
{
    public static class ControlExtensions
    {
        /// <summary>
        /// Gets the X/Y coordinates above a control.
        /// </summary>
        /// <param name="baseControl">The control to spawn relative to.</param>
        /// <param name="spawningControl">The control being spawned.</param>
        /// <returns>Point, the location above the baseControl.</returns>
        public static Point GetScreenSpawnPosition(Control baseControl, Control spawningControl)
        {
            // Get the current location on screen the main control is at.
            Point locationFromScreen = baseControl.PointToScreen(new Point(baseControl.ActualWidth, 0));

            // Get position above the control using the spawningControl's height/width.
            Point spawningLocation = new Point(0, 0)
            {
                X = locationFromScreen.X - spawningControl.Width,
                Y = locationFromScreen.Y - spawningControl.Height
            };

            return spawningLocation;
        }

        public static System.Windows.Media.Color GetColorFromHex(uint hexCode)
        {
            byte[] bytes = BitConverter.GetBytes(hexCode);
            return System.Windows.Media.Color.FromArgb(255, bytes[2], bytes[1], bytes[0]);
        }
    }
}
