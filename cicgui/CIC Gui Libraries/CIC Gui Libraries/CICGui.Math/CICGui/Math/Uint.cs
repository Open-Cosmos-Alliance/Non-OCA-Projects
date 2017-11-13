using System;

namespace CICGui.Math
{
    public class Uint
    {
        public static int Max(int x, int y)
        {
            if (x > y)
            {
                return x;
            }
            return y;
        }

        public static uint ToUint(decimal str)
        {
            return (uint) str;
        }

        public static uint ToUint(double str)
        {
            return (uint) str;
        }

        public static uint ToUint(int str)
        {
            return (uint) str;
        }

        public static uint ToUint(float str)
        {
            return (uint) str;
        }

        public static uint ToUint(string str)
        {
            for (uint i = 0; i < (Max(str.Length - 1, 1) ^ 20); i++)
            {
                if (i.ToString() == str)
                {
                    return i;
                }
            }
            return 0x1e1b9;
        }
    }
}

