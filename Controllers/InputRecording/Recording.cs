using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Delfinovin.Controllers
{
    /// <summary>
    /// A class that represents a list of inputs
    /// to be played back.
    /// </summary>
    public class Recording
    {
        [JsonIgnore]
        public string RecordingName { get; set; } = "None";

        public List<InputRecord> Records { get; set; } = new List<InputRecord>();

        public Recording()
        {

        }
    }

    /// <summary>
    /// A struct that represents a set of button inputs
    /// and stick values to be played back.
    /// </summary>
    public struct InputRecord
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public GamecubeControllerButtons ButtonsPressed { get; set; }
        public int TimePressed { get; set; }

        [JsonConverter(typeof(Vector2Converter))]
        public Vector2 Triggers { get; set; }
        [JsonConverter(typeof(Vector2Converter))]
        public Vector2 LStick { get; set; }
        [JsonConverter(typeof(Vector2Converter))]
        public Vector2 RStick { get; set; }
    }
}
