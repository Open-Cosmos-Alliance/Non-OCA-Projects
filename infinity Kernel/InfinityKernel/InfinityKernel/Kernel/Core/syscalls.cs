using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GruntyOS.IO;

namespace GruntyOS.Core
{
    public unsafe class syscalls
    {
        private static string charToString(byte* ptr)
        {
            string ret = "";
            for (int i = 0; ptr[i] != 0; i++)
                ret += ((char)ptr[i]).ToString();
            return ret;
        }

        public static void handleSyscall(ref Cosmos.Core.INTs.IRQContext aContext)
        {
            if (aContext.EAX == 1)
            {
            }
            else if (aContext.EAX == 4)
            {
                int descriptor = (int)aContext.EBX;
                ushort size = (ushort)(uint)aContext.EDX;
                byte* data = (byte*)aContext.ECX;
                
                Stream stream = Stream.fromDescriptor(descriptor);
                for (int i = 0; i < size; i++)
                {
                    stream.Write(data[i]);
                }
            }
            else if (aContext.EAX == 3)
            {
                
                int descriptor = (int)aContext.EBX;
                byte* data = (byte*)(int)aContext.ECX;
                uint size = (ushort)aContext.EDX;
                Stream stream = Stream.fromDescriptor(descriptor);
                byte[] d = new byte[(int)size];
                stream.ReadBytes((int)size,d);
                for (uint i = 0; i < size; i++)
                {
                    data[i] = d[i] ;
                }
                 
            }
            else if (aContext.EAX == 5)
            {
                byte* file = (byte*)aContext.EBX;
                int flags = (int)aContext.ECX;
                int mode = (ushort)aContext.EDX;
                aContext.EAX = (uint)stdio.open(charToString(file),mode);
                
            }
            else if (aContext.EAX == 6)
            {
                int file = (int)aContext.EBX;
                Stream.fromDescriptor(file).Close();
            }
        }
    }
}
