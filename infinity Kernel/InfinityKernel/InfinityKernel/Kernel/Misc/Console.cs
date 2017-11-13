/*
 * Basically I decieded instead of rewritting stupid
 * .net classes I decieded to write my own based off 
 * of the C std library
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GruntyOS.IO;

namespace GruntyOS
{
  
    public unsafe class stdio
    {
        public static Stream Out, In, Err;
        public static Stream fopen(string file,int modes = 4)
        {
            return Kernel.Root.openFile(file,modes);
        }
        public static int open(string file, int modes = 4)
        {
            return Kernel.Root.openFile(file,modes).Descriptor;
        }
        public static Stream[] Pipe()
        {
            Stream[] pipes = new Stream[2];
            return pipes;
        }
        public static void clrscr()
        {
            printf("\\c");
        }
        public static string readln()
        {
            string str = "";
            uint start = Out.Pointer;
            char c;
            do
            {
                c = (char)(byte)In.Read();
                if (c == 8)
                {
                    if (Out.Pointer > start)
                    {
                        Out.Pointer--;
                        str = str.Substring(0, str.Length - 1);
                        Out.Write(0);
                        Out.Pointer--;
                    }
                }
                else
                {
                    if (c != '~')
                        Out.Write((byte)c);
                    else
                    {
                        Out.Write((byte)'\\');
                        Out.Write((byte)'e');
                    }
                }
                if (c != '\n')
                {
                    str += c.ToString();

                }
            }
            while (c != '\n');
            return str;
        }
        private static void printf_noEscape(string txt,params object[] args)
        {
            
        }
        public static void* malloc(size_t size)
        {
            return MemoryManager.malloc(size);
        }
        public static void free(void* ptr)
        {
            MemoryManager.free(ptr);
        }
        public static void memcopy(Stream src, void* dest, uint length)
        {
            for (uint i = 0; i < length; i++)
                ((byte*)dest)[i] = (byte)src.Read();
        }
        public static void memcopy(void* src, void* dest, uint length)
        {
            for (uint i = 0; i < length; i++)
                ((byte*)dest)[i] = ((byte*)src)[i];
        }
        public static string sprintf(string format, params object[] args)
        {
            string ret = "";
            int formatNum = 0;
            for (int i = 0; i < format.Length; i++)
            {
                if (format[i] != '%')
                {
                    ret += format[i].ToString();
                }
                else
                {
                    i++;
                    if (format[i] == 's')
                    {
                        ret += (string)args[formatNum];
                    }
                    else if (format[i] == 'u')
                    {
                        ret += ((uint)args[formatNum]).ToString();
                    }
                    else if (format[i] == 'x')
                    {
                        if (args[formatNum] is uint)
                        {
                            uint u = (uint)Conversions.StringToInt(args[formatNum].ToString());
                            ret += Conversions.asciiToHexadecimal(u);
                        }
                        else if (args[formatNum] is int)
                        {
                            uint u = (uint)Conversions.StringToInt(args[formatNum].ToString());
                            ret += Conversions.asciiToHexadecimal(u);
                        }
                    }
                    else if (format[i] == 'p')
                    {
                        if (args[formatNum] is uint)
                        {
                            uint u = (uint)Conversions.StringToInt(args[formatNum].ToString());
                            ret += Conversions.asciiToHexadecimal(u,true);
                        }
                        else if (args[formatNum] is int)
                        {
                            uint u = (uint)Conversions.StringToInt(args[formatNum].ToString());
                            ret += Conversions.asciiToHexadecimal(u,true);
                        }
                    }
                    else if (format[i] == 'd')
                    {
                        ret += args[formatNum].ToString();
                    }
                    formatNum++;
                }
            }
            return ret;
        }
        public static void fprintf(Stream str, string format,params object[] args)
        {
            string val = sprintf(format, args);
           
            foreach (char c in val)
                str.Write((byte)c);
        }
        public static void fprintf(int desc, string format, params object[] args)
        {
            fprintf(Stream.fromDescriptor(desc), format,args);
        }
        public static void printf(string format, params object[] args)
        {
            fprintf(1, format, args);
        }
        public void print(string text)
        {
        }
    }
}
