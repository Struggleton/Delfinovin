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

        public static bool GetDeadzone(float x, float y)
        {
            if (ApplicationSettings.StickDeadzone <= 0.00f) // this means they use no deadzone. Use normal input.
                return false;

            float rad = 32767 * ApplicationSettings.StickDeadzone;
            if ((x - 0f) * (x - 0f) + (y - 0f) * (y - 0f) < rad * rad)
                return true;
            else
                return false;
        }
    }
}
