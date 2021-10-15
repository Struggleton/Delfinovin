using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace DelfinovinUI
{
	public static class ApplicationSettings
	{
		private const string SETTING_FILENAME = "settings.txt";
		private static CultureInfo usLangProvider = new CultureInfo("en-US");

		public static bool MinimizeToTray { get; set; } = true;
		public static bool MinimizeOnStartup { get; set; } = false;
		public static bool CheckForUpdates { get; set; } = true;
		public static string DefaultProfile1 { get; set; } = "";
		public static string DefaultProfile2 { get; set; } = "";
		public static string DefaultProfile3 { get; set; } = "";
		public static string DefaultProfile4 { get; set; } = "";
		public static string ApplicationTheme { get; set; } = "";
		public static string ControllerColor { get; set; } = "";

		public static void LoadSettings()
		{
			// If the application setting file doesn't exist, 
			if (!File.Exists(SETTING_FILENAME))
				SaveSettings();

			// Read all the settings to an array.
			string[] settings = File.ReadAllLines(SETTING_FILENAME);

			// If the file isn't longer than 0,
			// Stop execution and don't overwrite default values.
			if (settings.Length == 0)
				return;

			foreach (string setting in settings)
			{
				// Make sure there is no white space between separator 
				string[] settingPair = setting.Replace(": ", ":").Split(':'); 

				// This is so we can have Reflection assign values automatically
				// Without having to hardcode the reading/writing of values.
				Type type = typeof(ApplicationSettings);
				PropertyInfo[] properties = type.GetProperties();

				foreach (PropertyInfo prop in properties)
				{
					// If the name corresponds to the property, 
					// Assign its value from the file to it.
					if (settingPair[0] == prop.Name)
					{
						// Convert string to proper type.
						// Set the CultureInfo to United States so we
						// have a consistent decimal separator.
						prop.SetValue(null, Convert.ChangeType(settingPair[1], prop.PropertyType, usLangProvider));
					}
				}
			}
		}

		public static void SaveSettings()
		{
			// We're going to use Reflection to gather all the properties
			// and their values and save them to a file.
			StringBuilder sb = new StringBuilder();
			Type type = typeof(ApplicationSettings);
			PropertyInfo[] properties = type.GetProperties();

			foreach (PropertyInfo prop in properties)
			{
				// Get the name of the property and its value
				object value = prop.GetValue(null, BindingFlags.GetProperty, null, null, usLangProvider);

				// Apply the United States language provider 
				// to the string conversion so we have a consistent separator. 
				string valueStr = Convert.ToString(value, usLangProvider);

				string name = prop.Name;

				// Format the line
				sb.AppendLine($"{name}: {valueStr}");
			}

			// Save all the settings to a file. Overwrite it if it exists.
			File.WriteAllText(SETTING_FILENAME, sb.ToString());
		}
	}
}
