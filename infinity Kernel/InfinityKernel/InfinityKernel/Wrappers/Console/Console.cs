/*
 * Honestly I see no reason for making any of these
 * wrappers, but... some people will get pissed if
 * I don't include so here you go xD
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public partial class Console
    {
        private static ConsoleColor _foreColor;
        private static ConsoleColor _backColor;
        public static ConsoleColor ForegroundColor
        {
            get
            {
                return _foreColor;
            }
            set
            {
                _foreColor = value;
                if ((int)value > 7)
                {
                    value = (ConsoleColor)(value - 7);
                    GruntyOS.stdio.printf("\\[1;" + 30 + (int)value + "m");
                }
                else
                    GruntyOS.stdio.printf("\\[0;" + 30 + (int)value + "m");
            }
        }
        public static ConsoleColor BackgroundColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                _backColor = value;
                if ((int)value > 7)
                {
                    value = (ConsoleColor)(value - 7);
                    GruntyOS.stdio.printf("\\[1;" + 40 + (int)value + "m");
                }
                else
                    GruntyOS.stdio.printf("\\[0;" + 40 + (int)value + "m");
            }
        }
        
        public static void Clear()
        {
            GruntyOS.stdio.printf("\\c");
        }
        public static void Write(string text)
        {
            GruntyOS.stdio.printf("%s", text);
        }
        public static void WriteLine(string text)
        {
            Write(text + "\n");
        }
    }
}
