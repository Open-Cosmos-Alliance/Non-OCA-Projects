using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS.Crypto
{
    public class Hash
    {
        public static int GHash(string s)
        {
            // I know this is a horrible hash algol , I need to implement MD5!!!!
            int hash = 23;
            int pos = 0;
            foreach (char c in s.ToCharArray())
            {
                pos++;
                hash = hash + (pos | c);
            }
            return hash * s.Length;
        }
        public static int GHash(byte[] s)
        {
            // I know this is a horrible hash algol , I need to implement MD5!!!!
            int hash = 23;
            int pos = 0;
            foreach (byte c in s)
            {
                pos++;
                hash = hash + (pos | c);
            }
            return hash * s.Length;
        }
    }
}
