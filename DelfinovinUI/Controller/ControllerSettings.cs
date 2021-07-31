using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace DelfinovinUI
{
	public class ControllerSettings
	{
		public float TriggerDeadzone { get; set; } = 0.15f;
		public float TriggerThreshold { get; set; } = 0.65f;
		public float LeftStickDeadzone { get; set; } = 0f;
		public float LeftStickRange { get; set; } = 1f;
		public float RightStickDeadzone { get; set; } = 0f;
		public float RightStickRange { get; set; } = 1f;
		public bool EnableDigitalPress { get; set; } = false;
		public bool EnableRumble { get; set; } = false;
		public bool SwapAB { get; set; } = true;
		public bool SwapXY { get; set; } = true;

		// A false value return from this method means that
		// the file did not exist or did not contain any content.
		public bool LoadFromFile(string fileName)
		{
			// If the file doesn't exist, end execution
			// and return a false value.
			if (!File.Exists(fileName))
				return false;

			// Read all the settings to an array.
			string[] settings = File.ReadAllLines(fileName);

			// If the file isn't longer than 0,
			// Stop execution and return a false value.
			if (settings.Length == 0)
				return false;


			foreach (string setting in settings)
			{
				// Make sure there is no white space between separator 
				string[] settingPair = setting.Replace(": ", ":").Split(':');

				// This is so we can have Reflection assign values automatically
				// Without having to hardcode the reading/writing of values.
				PropertyInfo[] properties = this.GetType().GetProperties();
				foreach (PropertyInfo prop in properties)
				{
					// If the name corresponds to the property, 
					// Assign its value from the file to it.
					if (settingPair[0] == prop.Name)
					{
						// Convert string to proper type.
						// InvariantCulture fixes this not working on other 
						// languages that use periods (.) instead of commas (,).
						prop.SetValue(this, Convert.ChangeType(settingPair[1], prop.PropertyType, CultureInfo.InvariantCulture));
					}
				}
			}

			// We loaded all of the settings properly, return true.
			return true;
		}
	

		public void SaveFile(string fileName)
		{
			// We're going to use Reflection to gather all the properties
			// and their values and save them to a file.
			StringBuilder sb = new StringBuilder();
			Type type = typeof(ControllerSettings);
			PropertyInfo[] properties = type.GetProperties();

			foreach (PropertyInfo prop in properties)
			{
				// Get the name of the property and its value
				object value = prop.GetValue(this, null);
				string name = prop.Name;
				sb.AppendLine($"{name}: {value}");
			}

			// Save all the settings to a file. Overwrite it if it exists.
			File.WriteAllText(fileName, sb.ToString());
		}
	}
}
