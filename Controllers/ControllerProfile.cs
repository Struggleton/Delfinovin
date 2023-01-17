using Nefarius.ViGEm.Client.Targets.Xbox360;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Delfinovin.Controllers
{
	/// <summary>
	/// Represent a set of settings a user can set in the application.
	/// </summary>
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
		public bool SwapControlSticks { get; set; } = false;
		public bool EnableRumble { get; set; } = false;
		public bool Favorited { get; set; } = false;

		public Dictionary<XboxControllerButtons, GamecubeControllerButtons> ButtonMapping { get; set; } = new ()
		{
			{ XboxControllerButtons.A, GamecubeControllerButtons.A },
			{ XboxControllerButtons.B, GamecubeControllerButtons.B },
			{ XboxControllerButtons.X, GamecubeControllerButtons.X },
			{ XboxControllerButtons.Y, GamecubeControllerButtons.Y },
			{ XboxControllerButtons.Start, GamecubeControllerButtons.Start },

			{ XboxControllerButtons.LeftShoulderAnalog, GamecubeControllerButtons.LAnalog },
			{ XboxControllerButtons.RightShoulderAnalog, GamecubeControllerButtons.RAnalog },
			{ XboxControllerButtons.RightShoulder, GamecubeControllerButtons.Z },

			{ XboxControllerButtons.DPadUp, GamecubeControllerButtons.DpadUp },
			{ XboxControllerButtons.DPadDown, GamecubeControllerButtons.DpadDown },
			{ XboxControllerButtons.DPadLeft, GamecubeControllerButtons.DpadLeft },
			{ XboxControllerButtons.DPadRight, GamecubeControllerButtons.DpadRight },
		};

		public ControllerProfile()
		{

		}

        // We have to do this so JSON deserialization doesn't fail. If I didn't have to do this
        // I would just make the buttonmappingentry use Xbox360Property and cast 
        public static Dictionary<XboxControllerButtons, Xbox360Property> XboxPropertyMapping = new Dictionary<XboxControllerButtons, Xbox360Property>()
        {
            { XboxControllerButtons.A, Xbox360Button.A },
            { XboxControllerButtons.B, Xbox360Button.B },
            { XboxControllerButtons.X, Xbox360Button.X },
            { XboxControllerButtons.Y, Xbox360Button.Y },

            { XboxControllerButtons.Start, Xbox360Button.Start },
            { XboxControllerButtons.Guide, Xbox360Button.Guide },
            { XboxControllerButtons.Back, Xbox360Button.Back },

            { XboxControllerButtons.LeftThumbButton, Xbox360Button.LeftThumb },
            { XboxControllerButtons.RightThumbButton, Xbox360Button.RightThumb },

            { XboxControllerButtons.LeftShoulder, Xbox360Button.LeftShoulder },
            { XboxControllerButtons.RightShoulder, Xbox360Button.RightShoulder },

            { XboxControllerButtons.LeftShoulderAnalog, Xbox360Slider.LeftTrigger },
            { XboxControllerButtons.RightShoulderAnalog, Xbox360Slider.RightTrigger },

            { XboxControllerButtons.DPadDown, Xbox360Button.Down },
            { XboxControllerButtons.DPadUp, Xbox360Button.Up },
            { XboxControllerButtons.DPadLeft, Xbox360Button.Left },
            { XboxControllerButtons.DPadRight, Xbox360Button.Right },
        };
    }
}
