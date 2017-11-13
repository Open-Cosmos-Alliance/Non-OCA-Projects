using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GruntyOS.IO;
using GruntyOS.Crypto;

namespace GruntyOS
{
    public class CurrentUser
    {
        public static string Username;
        public static int PasswordHash;
        public static int Privilages;
        public static bool promptPassword()
        {
            ConsoleColor fg = Console.ForegroundColor;
            Console.Write("Password: ");
            Console.ForegroundColor = Console.BackgroundColor;
            string pass = Console.ReadLine();
            Console.ForegroundColor = fg;
            BinaryReader br = new BinaryReader(new FileStream("etc/passwd/" + Username + ".dat", "r"));
            if (br.ReadInt32() == Crypto.Hash.GHash(pass))
            {
                return true;
            }
            return false;
        }
    }
}
