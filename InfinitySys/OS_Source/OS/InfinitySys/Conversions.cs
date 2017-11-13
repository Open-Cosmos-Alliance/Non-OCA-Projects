using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS
{
    public class Conversions
    {
        public static int StringToInt(string dat)
        {
            string Reverse = "";
            foreach (byte b in dat)
            {
                Reverse = ((char)b).ToString() + Reverse;
            }
            dat = Reverse;
            int final = 0;
            int mul = 1;
            foreach (byte b in dat)
            {
                int RealDigit = b - 48;
                final += RealDigit * mul;
                mul = mul * 10;
            }
            return final;
        }
        public static int BoolToInt(bool b)
        {
            if (b)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        public static bool IntToBool(int i)
        {
            if (i == 1)
            {
                return true;
            }
            else
                return false;
        }
        public static int StringToHex(string dat)
        {
            int final = 0;
            int mul = dat.Length;
            foreach (byte b in dat)
            {

                int RealDigit = b - 48;
                if (b == 'A')
                {

                }
                if (mul == 1)
                {
                    final += RealDigit;
                }
                else
                    final += RealDigit * (10 * mul);
                mul--;
            }
            return final;
        }
    }
}
