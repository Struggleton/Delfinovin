using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Delfinovin
{
    /// <summary>
    /// Provide an easily extensible settings system with basic file read/writing.
    /// </summary>
    public static class ApplicationSettings
    {
        private static string SETTING_FILENAME = "settings.txt";

        public static float TriggerDeadzone { get; set; } = 0.15f; // Set the amount that the trigger has to be pressed in order to register any input.
        public static float TriggerThreshold { get; set; } = 0.65f; // Set the amount that the analog trigger has to be pressed before it registers as a full press.
        public static float StickDeadzone { get; set; } = 0.00f; // Sets the range of the stick that is registered as a neutral position.
        public static bool EnableRawPrint { get; set; } = true; // Enable/disable printing raw data that is being sent from the adapter.
        public static bool CalibrateCenter { get; set; } = true; // Enable/disable stick center calibration on startup.
        public static bool EnableDigitalPress { get; set; } = false; // If any press is past the TriggerDeadzone, it registers as a digital button.
        public static bool EnableRumble { get; set; } = false; // Enable Haptic Feedback on powered controllers.
        public static List<int> PortsEnabled { get; set; } = new List<int>() { 1 }; // Enable/disable ports (Ports 1-4 supported.)

        public static void LoadSettings()
        {
            if (File.Exists(SETTING_FILENAME))
            {
                string[] settings = File.ReadAllLines(SETTING_FILENAME);

                // check if there are actually settings to read
                if (settings.Length > 0)
                {
                    foreach (var setting in settings)
                    {
                        // Make sure setting separator leaves no whitespace
                        string[] settingPair = setting.Replace(": ", ":").Split(':');

                        Type type = typeof(ApplicationSettings);
                        foreach (var prop in type.GetProperties())
                        {
                            // Find if there is a field here with the same setting name
                            if (settingPair[0] == prop.Name)
                            {
                                // Explict list support, Convert.ChangeType can't do this natively
                                if (prop.Name == "PortsEnabled")
                                {
                                    PortsEnabled = ParsePortsToList(settingPair[1]);
                                }

                                else
                                {
                                    // Convert string to whatever type the field is
                                    prop.SetValue(null, Convert.ChangeType(settingPair[1], prop.PropertyType, CultureInfo.InvariantCulture));
                                }
                            }
                        }
                    }
                }
            }
            
            // If the file doesn't exist, create a new one using defaults
            else
            {
                SaveDefaultSettings();
            }
        }

        private static void SaveDefaultSettings()
        {
            StringBuilder sb = new StringBuilder();

            Type type = typeof(ApplicationSettings);
            foreach (var prop in type.GetProperties()) // Get all static fields
            {
                var value = prop.GetValue(null, null);
                var name = prop.Name;

                if (prop.Name == "PortsEnabled")
                {
                    sb.Append($"{name}: {ParseListToString(PortsEnabled)}");
                }

                else
                {
                    sb.AppendLine($"{name}: {value}");
                }
            }

            File.WriteAllText(SETTING_FILENAME, sb.ToString());
        }

        private static List<int> ParsePortsToList(string stringList)
        {
            string[] ports = stringList.TrimEnd(' ').Split(' ');
            List<int> portsList = new List<int>();

            foreach (string port in ports)
            {
                if (int.TryParse(port, out int portNum))
                {
                    // Do this so ports are easily usable in arrays
                    portsList.Add(portNum - 1);
                }

                // Throw an error if it fails to parse string
                else
                {
                    throw new Exception(Strings.ERROR_PORTS);
                }
            }

            return portsList;
        }

        private static string ParseListToString(List<int> intList)
        {
            return string.Join(" ", intList);
        }
    }
}
