using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GruntyOS.IO;
namespace GruntyOS
{
    public unsafe partial class Kernel
    {
        /// <summary>
        /// Executes a program from a byte stream
        /// </summary>
        /// <param name="executable">A stream containg executable code</param>
        /// <param name="args">Command line arguments</param>
        public static void exec(byte[] machineCode)
        {
            byte* program = (byte*)0x100;
            for (int i = 0; i < machineCode.Length; i++)
                program[i] = machineCode[i];
            doCall(0x100);
        }
        /// <summary>
        /// Execute a file
        /// </summary>
        public static void exec(string file)
        {
        }
    }
}
