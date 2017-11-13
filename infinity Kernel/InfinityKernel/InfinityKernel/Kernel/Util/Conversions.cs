/*******************************************************
 * Copyright (C) 2012-2013 GruntXProductions <sloan@gruntxproductions.net>
 * 
 * This file is part of Grunty OS Infinity
 * 
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential.
 *******************************************************/

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
        public static char getHexValue(byte value)
        {
            if (value < 0) return '0';
            if (value > 16) return '0';
            if (value <= 9) return (char)('0' + value);
            value -= 10;
            return (char)('A' + value);
        }
        public static string asciiToHexadecimal(uint inputChar,bool pad = false)
        {
            string hexString = "";
           int i = 0;
           byte[] remainders = new byte[8];
           int result = (int)inputChar;
           while (result != 0) {
              remainders[i++] = (byte)(result % 16);
              result /= (int)16;
           }
           int j = 0;
           for (i = 8; i >= 0; --i) {
              char c = getHexValue(remainders[i]);
              hexString += c;
           }
           if (pad)
           {
               while (hexString.Length < 8)
                   hexString = "0" + hexString;
           }
           return hexString;
        }
        public static int HexStringToInt(string dat)
        {
            dat = dat.ToUpper();
            string Reverse = "";
            foreach (byte d in dat)
            {
                Reverse = ((char)d).ToString() + Reverse;
            }
            dat = Reverse;
            int final = 0;
            int mul = 1;
            foreach (byte b in dat)
            {
                int RealDigit = 0;
                if ((byte)'A' == b)
                    RealDigit = 10;
                else if ((byte)'B' == b)
                    RealDigit = 11;
                else if ((byte)'C' == b)
                    RealDigit = 12;
                else if ((byte)'D' == b)
                    RealDigit = 13;
                else if ((byte)'E' == b)
                    RealDigit = 14;
                else if ((byte)'F' == b)
                    RealDigit = 15;
                else
                    RealDigit = b - 48;

                final += RealDigit * mul;
                mul = mul * 16;
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
        public static string[] Lookup = new string[] { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F", "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF", "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF", "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF", "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF", "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF", "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF" };
        public static string ByteToHex(int dat)
        {

            string s = Lookup[dat];
            return s;
        }
    }
}
