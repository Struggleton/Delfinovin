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

		public static bool MinimizeToTray { get; set; } = true;
		public static string DefaultProfile1 { get; set; } = "";
		public static string DefaultProfile2 { get; set; } = "";
		public static string DefaultProfile3 { get; set; } = "";
		public static string DefaultProfile4 { get; set; } = "";

		public static void LoadSettings()
		{
			if (!File.Exists(SETTING_FILENAME))
				SaveSettings();

			string[] settings = File.ReadAllLines(SETTING_FILENAME);
			if (settings.Length == 0)
				return;

			foreach (string setting in settings)
			{
				string[] settingPair = setting.Replace(": ", ":").Split(':'); // Make sure there is no white space between separator 
				Type type = typeof(ApplicationSettings);
				PropertyInfo[] properties = type.GetProperties();

				foreach (PropertyInfo prop in properties)
				{
					if (settingPair[0] == prop.Name)
					{
						prop.SetValue(null, Convert.ChangeType(settingPair[1], prop.PropertyType, CultureInfo.InvariantCulture));
					}
				}
			}
		}

		public static void SaveSettings()
		{
			StringBuilder sb = new StringBuilder();
			Type type = typeof(ApplicationSettings);
			PropertyInfo[] properties = type.GetProperties();
			foreach (PropertyInfo prop in properties)
			{
				object value = prop.GetValue(null, null);
				string name = prop.Name;
				sb.AppendLine($"{name}: {value}");
			}
			File.WriteAllText(SETTING_FILENAME, sb.ToString());
		}
	}
}
