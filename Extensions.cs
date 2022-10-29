using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WshLibrary = IWshRuntimeLibrary;
using System.IO;

namespace Delfinovin
{
    public static class Extensions
    {
        public static float Remap(this float from, float fromMin, float fromMax, float toMin, float toMax)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return to;
        }

        public static void CreateApplicationShortcut(string path)
        {
            // Check to see if the shortcut exists
            if (!File.Exists(path))
            {
                // Create a WshShell to create the shortcut with
                WshLibrary.WshShell wsh = new WshLibrary.WshShell();
                WshLibrary.IWshShortcut shortcut = wsh.CreateShortcut(path) as WshLibrary.IWshShortcut;

                // Get the application's current working path
                shortcut.TargetPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                // Not sure what this does
                shortcut.WindowStyle = 1;

                // Set the description for the shortcut
                shortcut.Description = "An XInput solution for Gamecube Controllers";

                // Get the application's working directory
                shortcut.WorkingDirectory = Directory.GetCurrentDirectory();

                // Save the shortcut to the startup folder
                shortcut.Save();
            }
        }
    }
}
