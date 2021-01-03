using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Delfinovin
{
    public static class ApplicationSettings
    {
        private static string SETTING_FILENAME = "settings.txt";

        [Description("Deadzone refers to the amount that the trigger has to be pressed in order to register any input.")]
        public static float TriggerDeadzone { get; set; } = 0.15f;

        [Description("Threshold refers to the amount that the analog trigger has to be pressed before it registers as a full press.")]
        public static float TriggerThreshold { get; set; } = 0.65f;

        [Description("Enable/disable printing raw data that is being sent from the adapter.")]
        public static bool EnableRawPrint { get; set; } = true;

        [Description("If enabled, any analog button press will act as a full digital press. Make sure your analog deadzone is set properly!")]
        public static bool EnableAnalogPress { get; set; } = false;

        [Description("Enable/disable specific ports. Ports 1-4 supported")]
        public static List<int> PortsEnabled { get; set; } = new List<int>() { 1 };

        public static void LoadSettings()
        {
            if (File.Exists(SETTING_FILENAME) )
            {
                string[] settings = File.ReadAllLines(SETTING_FILENAME);

                // file exists! if not use defaults
                if (settings.Length > 0)
                {
                    foreach (var setting in settings)
                    {
                        string[] settingPair = setting.Replace(": ", ":").Split(':');

                        Type type = typeof(ApplicationSettings);
                        foreach (var prop in type.GetProperties())
                        {
                            if (settingPair[0] == prop.Name)
                            {
                                if (prop.Name == "PortsEnabled")
                                {
                                    PortsEnabled.Clear();
                                    string[] ports = settingPair[1].TrimEnd(' ').Split(' ');

                                    foreach (string port in ports)
                                    {
                                        if (int.TryParse(port, out int portNum))
                                        {
                                            PortsEnabled.Add(portNum - 1);
                                        }
                                        else
                                        {
                                            throw new Exception(Strings.ERROR_PORTS);
                                        }
                                    }

                                }

                                else
                                {
                                    prop.SetValue(null, Convert.ChangeType(settingPair[1], prop.PropertyType));
                                }
                            }
                        }
                    }
                }
            }
            

            else
            {
                SaveSettings();
            }

        }

        public static void SaveSettings()
        {
            StringBuilder sb = new StringBuilder();

            Type type = typeof(ApplicationSettings);
            foreach (var prop in type.GetProperties())
            {
                var value = prop.GetValue(null, null);
                var name = prop.Name;

                if (prop.Name == "PortsEnabled")
                {
                    sb.Append($"{name}: ");
                    foreach (int port in PortsEnabled)
                        sb.Append(port + " ");
                }

                else
                    sb.AppendLine($"{name}: {value}");

            }

            File.WriteAllText(SETTING_FILENAME, sb.ToString());
        }
    }
}
