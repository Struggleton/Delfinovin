using System;

namespace Delfinovin
{
    public static class Extensions
    {
        public static short ByteToShort(byte b)
        {
            // return a sign extend/scale a byte to a short
            return (short)(((b << 8) | b) ^ 0x8000);
        }

        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            // ensure values fit within given bounds
            if (value.CompareTo(max) > 0) return max;
            if (value.CompareTo(min) < 0) return min;
            return value;
        }
    }
}
