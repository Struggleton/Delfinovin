using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelfinovinActX2
{
    public class GamecubeDeadzones
    {
        public static bool CompareTriggers(byte triggerSlider)
        {
            return (triggerSlider / 255f) > ApplicationSettings.TriggerDeadzone;
        }

        public static byte ClampTriggers(byte triggerSlider)
        {
            // convert trigger byte to float
            float compare = triggerSlider / 255f;

            // If the trigger deadzone is greater, return no input
            if (ApplicationSettings.TriggerDeadzone > compare)
                return 0;
            // If the trigger threshold is met, send a full input
            else if (ApplicationSettings.TriggerThreshold < compare)
                return 255;

            return triggerSlider;
        }
    }
}
