using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Delfinovin
{
    public static class Extensions
    {
        public static float Remap(this float from, float fromMin, float fromMax, float toMin, float toMax)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return to;
        }

        public static bool GetBit(byte b, int bitNumber)
        {
            var bit = ((b >> bitNumber) & 1) != 0;
            return bit;
        }

        public static Vector2 ClampToByte(Vector2 stick)
        {
            Vector2 point = new Vector2(stick.X, stick.Y);
            point.X = Clamp(point.X, 0, 255);
            point.Y = Clamp(point.Y, 0, 255);

            return point;
        }

        public static Vector2 ScaleStickVector(Vector2 byteStick)
        {
            Vector2 point = new Vector2(byteStick.X, byteStick.Y);
            point.X = ByteToShort((byte)point.X, false);
            point.Y = ByteToShort((byte)point.Y, false);

            return point;
        }

        public static short ByteToShort(byte b, bool invert)
        {
            var intValue = b - 0x80;
            if (intValue == -128)
                intValue = -127;

            // I have no clue what this value does. Thanks to Nefarius
            // for this scaling function
            var wtfValue = intValue * 258.00787401574803149606299212599f;

            return (short)(invert ? -wtfValue : wtfValue);
        }

        public static Vector2 ApplyDeadzone(Vector2 stick, float deadzone)
        {
            Vector2 point = new Vector2(stick.X, stick.Y);
            if (CheckRadialDeadzone(stick.X, stick.Y, deadzone))
            {
                point.X = 0;
                point.Y = 0;
            }

            return point;
        }

        private static bool CheckRadialDeadzone(float x, float y, float deadzone)
        {
            if (deadzone <= 0f)
                return false;

            float rad2 = 32767f * 32767f * deadzone * deadzone;

            // Calculate the distance using the Manhattan distance
            float distance2 = x * x + y * y;

            // Compare the distance to the radius
            return distance2 < rad2;
        }


        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            // ensure values fit within given bounds
            if (value.CompareTo(max) > 0) return max;
            if (value.CompareTo(min) < 0) return min;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ClampToCircle(Vector2 position, float range)
        {
            Vector2 point = new Vector2(position.X, position.Y) * range;

            if (point.Length() > short.MaxValue)
            {
                point = point / point.Length() * short.MaxValue;
            }

            return new Vector2((float)point.X, (float)point.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ClampToCircle(Vector2 position)
        {
            Vector2 point = new Vector2(position.X, position.Y);
            if (point.Length() > short.MaxValue)
            {
                point = point / point.Length() * short.MaxValue;
            }

            return new Vector2((float)point.X, (float)point.Y);
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
    }
}
