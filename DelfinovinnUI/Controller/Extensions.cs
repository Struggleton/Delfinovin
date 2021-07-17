using System;

namespace DelfinovinnUI
{
	public static class Extensions
	{
        public static short ByteToShort(byte b, bool invert)
        {
            // return a sign extend/scale a byte to a short
            //return (short)(((b << 8) | b) ^ 0x8000);

            var intValue = b - 0x80;
            if (intValue == -128) intValue = -127;

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
    }
}
