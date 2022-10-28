using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace Delfinovin.Controllers
{
	public class ControllerProfile
	{
		[JsonIgnore]
		public string ProfileName { get; set; } = "None";

		public float TriggerDeadzone { get; set; } = 0.15f;
		public float TriggerThreshold { get; set; } = 0.65f;
		public float LeftStickDeadzone { get; set; } = 0f;
		public float LeftStickRange { get; set; } = 0.75f;
		public float RightStickDeadzone { get; set; } = 0f;
		public float RightStickRange { get; set; } = 0.75f;
		public bool EnableDigitalPress { get; set; } = false;
		public bool SwapControlSticks { get; set; } = false;
		public bool EnableRumble { get; set; } = false;

		public ControllerProfile()
		{

		}
	}
}
