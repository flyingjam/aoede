using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoede
{
    static class Misc
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static bool isUnicodeNull(string str)
        {
            foreach(char c in str)
            {
                if ((int)c == 0)
                    return true;
            }

            return false;
        }
    }
}
