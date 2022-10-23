using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Delfinovin
{
    public static class Extensions
    {
        public static double[] GetScreenSpawnPosition(Control baseControl, Control spawningControl)
        {
            double[] spawnLocation = new double[2];
            var basePoint = baseControl.PointToScreen(new System.Windows.Point(0, 0));
            spawnLocation[0] = basePoint.X;
            spawnLocation[1] = basePoint.Y - spawningControl.Height;

            return spawnLocation;
        }
    }
}
