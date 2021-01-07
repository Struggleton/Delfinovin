using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delfinovin
{
    public static class Extensions
    {
        public static short ByteToShort(byte b)
        {
            return (short)(((b << 8) | b) ^ 0x8000);
        }

        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            T result = value;
            if (value.CompareTo(max) > 0) result = max;
            if (value.CompareTo(min) < 0) result = min;
            return result;
        }
    }
}
