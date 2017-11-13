using System;

namespace CICGui.Math
{
    public static class Math
    {
        public static int Max(int x, int y)
        {
            if (x > y)
            {
                return x;
            }
            return y;
        }

        public static int Min(int x, int y)
        {
            if (x < y)
            {
                return x;
            }
            return y;
        }

        public static uint Pow(uint x, uint y)
        {
            if (y < 2)
            {
                return x;
            }
            uint o = x * x;
            y -= 2;
            if (y > 0)
            {
                o = PowHlpr(o, x, y);
            }
            return o;
        }

        private static uint PowHlpr(uint o, uint x, uint y)
        {
            uint num = o * x;
            y--;
            if (y > 0)
            {
                num = PowHlpr(num, x, y);
            }
            return num;
        }

        public static uint ToUint(string s)
        {
            uint length = (uint) s.Length;
            uint num2 = Pow(10, length);
            uint num3 = 0;
            for (int i = 0; i < length; i++)
            {
                switch (s[i])
                {
                    case '0':
                        break;

                    case '1':
                        num3 += num2;
                        break;

                    case '2':
                        num3 += 2 * num2;
                        break;

                    case '3':
                        num3 += 3 * num2;
                        break;

                    case '4':
                        num3 += 4 * num2;
                        break;

                    case '5':
                        num3 += 5 * num2;
                        break;

                    case '6':
                        num3 += 6 * num2;
                        break;

                    case '7':
                        num3 += 7 * num2;
                        break;

                    case '8':
                        num3 += 8 * num2;
                        break;

                    case '9':
                        num3 += 9 * num2;
                        break;

                    default:
                        throw new Exception("Input string was not in correct format.");
                }
                num2 /= 10;
            }
            return (num3 / 10);
        }
    }
}

