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

		public bool LoadFromFile(string fileName)
		{
			if (!File.Exists(fileName))
				return false;

			string[] settings = File.ReadAllLines(fileName);
			if (settings.Length != 0)
			{
				foreach (string setting in settings)
				{
					string[] settingPair = setting.Replace(": ", ":").Split(':');
					PropertyInfo[] properties = this.GetType().GetProperties();

					foreach (PropertyInfo prop in properties)
					{
						if (settingPair[0] == prop.Name)
						{
							// Convert string to proper type
							prop.SetValue(this, Convert.ChangeType(settingPair[1], prop.PropertyType, CultureInfo.InvariantCulture));
						}
					}
				}
				return true;
			}
			return false;
		}

		public void SaveFile(string fileName)
		{
			StringBuilder sb = new StringBuilder();
			Type type = typeof(ControllerSettings);
			PropertyInfo[] properties = type.GetProperties();
			foreach (PropertyInfo prop in properties)
			{
				object value = prop.GetValue(this, null);
				string name = prop.Name;
				sb.AppendLine($"{name}: {value}");
			}
			File.WriteAllText(fileName, sb.ToString());
		}
	}
}
