using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Media;

namespace DelfinovinUI
{
	public static class Extensions
	{
        public static short ByteToShort(byte b, bool invert)
        {
            // return a sign extend/scale a byte to a short
            //return (short)(((b << 8) | b) ^ 0x8000);

            var intValue = b - 0x80;
            if (intValue == -128) 
                intValue = -127;

            // I have no clue what this value does. Thanks to Nefarius
            // for this scaling function
            var wtfValue = intValue * 258.00787401574803149606299212599f;

            return (short)(invert ? -wtfValue : wtfValue);
        }

        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            // ensure values fit within given bounds
            if (value.CompareTo(max) > 0) return max;
            if (value.CompareTo(min) < 0) return min;
            return value;
        }

        public static bool GetBit(byte b, int bitNumber)
        {
            var bit = ((b >> bitNumber) & 1) != 0;
            return bit;
        }

        public static byte BoolToByte(bool b)
        {
            return b ? (byte)0x01 : (byte)0x00;
        }

        public static byte ClampTriggers(byte triggerSlider, float triggerDeadzone, float triggerThreshold)
        {
            float compare = triggerSlider / 255f;
            if (triggerDeadzone > compare)
                return 0;

            if (triggerThreshold < compare)
                return 255;
            return triggerSlider;
        }

        public static string[] GetResourcesUnder(string folder)
        {
            folder = folder.ToLower() + "/";

            var assembly = Assembly.GetCallingAssembly();
            var resourcesName = assembly.GetName().Name + ".g.resources";
            var stream = assembly.GetManifestResourceStream(resourcesName);
            var resourceReader = new ResourceReader(stream);

            var resources =
                from p in resourceReader.OfType<DictionaryEntry>()
                let theme = (string)p.Key
                where theme.StartsWith(folder)
                select theme.Substring(folder.Length);

            return resources.ToArray();
        }

        public static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static Color GetColorFromHex(uint hexCode)
        {
            byte[] bytes = BitConverter.GetBytes(hexCode);
            return Color.FromArgb(255, bytes[2], bytes[1], bytes[0]);
        }
    }
}
