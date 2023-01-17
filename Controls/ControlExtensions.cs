using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WshLibrary = IWshRuntimeLibrary;

namespace Delfinovin
{
    /// <summary>
    /// A set of functions in order to provide extended functionality
    /// to the application / simplify control methods
    /// </summary>
    public static class ControlExtensions
    {
        /// <summary>
        /// Gets the X/Y coordinates above a control.
        /// </summary>
        /// <param name="baseControl">The control to spawn relative to.</param>
        /// <param name="spawningControl">The control being spawned.</param>
        /// <returns>Point - the location above the baseControl.</returns>
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

        /// <summary>
        /// Converts a hex code/uint to a Color object
        /// </summary>
        /// <param name="hexCode">The color code to convert</param>
        /// <returns>Color - Returns a color with the RGB elements set</returns>
        public static Color GetColorFromHex(uint hexCode)
        {
            // Get the four bytes from the hex code (A, R, G, B)
            byte[] bytes = BitConverter.GetBytes(hexCode);

            // Convert bytes from byte array to a Color
            return Color.FromArgb(255, bytes[2], bytes[1], bytes[0]);
        }

        /// <summary>
        /// Clips the provided text to a specific text bound.
        /// </summary>
        /// <param name="charCount">The limit of how long the text can be</param>
        /// <param name="text">The text to be clipped.</param>
        /// <returns>String - either the clipped text, or if text length is less than the charCount, the text</returns>
        public static string ClipTextToBounds(int charCount, string text)
        {
            string clippedText = "";

            // Our string is less than the clip length, return the original text
            if (charCount > text.Length)
            {
                return text;
            }

            // Clip the ends of the text with the character count
            else
            {
                clippedText = text.Substring(0, charCount);
            }

            // Add at the end to show there is more text
            clippedText += "...";

            return clippedText;
        }

        /// <summary>
        /// Create an application shortcut at the provided path.
        /// </summary>
        /// <param name="path">The path at which to create the application at.</param>
        public static void CreateApplicationShortcut(string path)
        {
            // Check to see if the shortcut exists
            if (!File.Exists(path))
            {
                // Create a WshShell to create the shortcut with
                WshLibrary.WshShell wsh = new WshLibrary.WshShell();
                WshLibrary.IWshShortcut shortcut = wsh.CreateShortcut(path) as WshLibrary.IWshShortcut;

                // Get the application's current working path
                shortcut.TargetPath = Assembly.GetExecutingAssembly().Location;

                // Not sure what this does
                shortcut.WindowStyle = 1;

                // Set the description for the shortcut
                shortcut.Description = Strings.DetailsApplicationDescription;

                // Get the application's working directory
                shortcut.WorkingDirectory = Directory.GetCurrentDirectory();

                // Save the shortcut 
                shortcut.Save();
            }
        }

        public static string[] GetResourcesUnder(string folder)
        {
            folder = folder.ToLower() + "/";

            // Get our Assembly
            Assembly assembly = Assembly.GetCallingAssembly();

            // Get the name of the assembly and append the .g.resources tag to it
            string resourcesName = assembly.GetName().Name + ".g.resources";

            // Get the stream of all the resources in this assembly
            Stream stream = assembly.GetManifestResourceStream(resourcesName);

            // Create a resource reader using the stream from before
            using (ResourceReader resourceReader = new ResourceReader(stream))
            {
                // Get all resource entries
                // Where the name starts with the folder we've been provided
                // Create a list of all the entries, removing the folder path from it.
                var resources = resourceReader.OfType<DictionaryEntry>()
                                  .Where(p => ((string)p.Key).StartsWith(folder))
                                  .Select(p => ((string)p.Key).Substring(folder.Length));

                // Return our list of resource names.
                return resources.ToArray();
            }
        }
    }
}
