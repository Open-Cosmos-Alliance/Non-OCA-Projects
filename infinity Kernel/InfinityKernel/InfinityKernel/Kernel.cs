/*
 * Copyright (c) 2013 GruntXProductions Grunty OS Project
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */



/* This contains the bulk of core
 * system code required to boot 
 * Infinity
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.IL2CPU.Plugs;
using GruntyOS.IO.Filesystem;
using CPUx86 = Cosmos.Assembler.x86;
using GruntyOS.IO;
using GruntyOS.IO.Pipes;
namespace GruntyOS
{


    public enum Runlevel
    {
        Halt = 0,
        SingleUserMode = 1,
        Boot = 5,
        Reboot = 6,
    }
        
    public unsafe static partial class Kernel
    {
        public static RootFS Root;
        public static TextStream Log; 
        public static DevFS devFS;
        public static int runLevel = 5;
        public delegate void syncHandler();
        private static Cosmos.Core.IOPort io;
        private static List<syncHandler> SyncHandlers = new List<syncHandler>();
        public static void registerSyncCallback(syncHandler syncCallback)
        {
            mutexLock();
            SyncHandlers.Add(syncCallback);
            mutexUnlock();
        }
        /// <summary>
        /// Function is just like the stdio.printf,
        /// however this will log the data. And print
        /// if booting
        /// </summary>
        public static void printf(string format, params object[] args)
        {
            bool echo = (runLevel == 5 || runLevel == 0 || runLevel == 6);
            if (format[0] == '<' && format[2] == '>')
            {
                int level = Conversions.StringToInt(format[1].ToString());
                string[] prefixes = new string[] { "\\[0;31m[\\[1;37mFATAL\\[0;31m]\\[0;37m", "\\[0;31m[\\[1;37mfatal\\[0;31m]\\[0;37m", "\\[0;31m[\\[1;37mcritical\\[0;31m]\\[0;37m", "\\[0;31m[\\[1;37mfailed\\[0;31m]\\[0;37m", "\\[0;31m[\\[1;37mwarning\\[0;31m]\\[0;37m", "\\[0;36m[\\[1;37minfo\\[0;36m]\\[0;37m", "\\[0;36m[\\[1;37mok\\[0;36m]\\[0;37m", "[debug]" };
                string whiteSpaces = prefixes[level] + "                              ";
                whiteSpaces = whiteSpaces.Substring(0, 40);
                if(echo) // if rebooting, booting, or shuting down also print to stdout
                    stdio.printf(whiteSpaces + format.Substring(3), args);
                stdio.fprintf(Log, whiteSpaces + format.Substring(3), args);
            }
            else
            {
                if(echo)
                    stdio.printf(format, args);
                stdio.fprintf(Log, format, args);
            }
           // stdio.fprintf(Log, format + "\n", args);
        }
        private static string getTimeStamp()
        {
            return  ((int)Cosmos.Hardware.RTC.Hour).ToString() + ":" + ((int)Cosmos.Hardware.RTC.Minute).ToString() + ":" + ((int)Cosmos.Hardware.RTC.Second).ToString() + "-" + 
                ((int)Cosmos.Hardware.RTC.Month).ToString() + "/" + ((int)Cosmos.Hardware.RTC.DayOfTheMonth).ToString() + "/" +  ((int)Cosmos.Hardware.RTC.Century).ToString();
        }
        public static void doCall(uint addr)
        {
            Caller call = new Caller();
            call.CallCode(addr);
        }
        public static void Panic(Cosmos.Core.INTs.IRQContext aContext)
        {
             Paging.Disable();
            string[] errs = new string[] { "divide by zero", "single step", "non maskable interrupt", "break flow", "OVERFLOW", "NULL", "invalid opcode", "", "double fault", "INVALID TSS", "SEGMENT NOT PRESENT", "STACK EXCEPTION", "GENERAL PROTECTION FAULT" };
       
            stdio.printf("\nbug: %s\n\n",errs[aContext.Interrupt]);
            byte* badCode = (byte*)aContext.EIP - 20;
            uint* badStack = (uint*)aContext.ESP - 20;
            stdio.printf("EIP is 0x%p\n", aContext.EIP);
            stdio.printf("eax: %x edx: %x ecx: %x ebx: %x \n", aContext.EAX, aContext.EDX, aContext.ECX, aContext.EBX);
            stdio.printf("esi: %x edi: %x ebp: %x esp: %x \n", aContext.ESI, aContext.EDI, aContext.EBP, aContext.ESP);
        
            stdio.printf("\n\nstack: \n    ");
            for (int i = 1; i < 21; i++)
            {
                stdio.printf("%p  ", badStack[i]);
                if (i % 5 == 0 && i > 4)
                    stdio.printf("\n    ");
            }
            Trace(6);
            stdio.printf("\ncode: ");
            for (int i = 0; i < 20; i++)
                stdio.printf("%s ", Conversions.ByteToHex(badCode[i]).ToString());
            stdio.printf("\n ");
            stdio.printf("\nfatal exception: kernel panic!");
            while (true) ;
            
        }
        /// <summary>
        /// Sync will save all buffered data
        /// it should be called when shutting down
        /// </summary>
        public static void Sync()
        {
            for (int i = 0; i < SyncHandlers.Count; i++)
                SyncHandlers[i]();
            for (int i = 0; i < services.Count; i++)
                services[i].onSync();
        }
        public static ushort Inw(ushort port)
        {
            if (io.Port != port)
                io = new Cosmos.Core.IOPort(port);
            return io.Word;

        }
        public static void Outb(ushort port, byte data)
        {
            if (io.Port != port)
                io = new Cosmos.Core.IOPort(port);
            io.Byte = data;
          

        }
        public static void Outw(ushort port, ushort data)
        {
            if (io.Port != port)
                io = new Cosmos.Core.IOPort(port);
            io.Word = data;

        }
        public static void Trace(uint MaxFrames)
        {
            uint * ebp = &MaxFrames - 2;
            stdio.printf("\nCall trace:\n");
            for(uint frame = 0; frame < MaxFrames; ++frame)
            {
                uint eip = ebp[1];
                if(eip == 0)
                    break;
                ebp = (uint*)(ebp[0]);
                uint * arguments = &ebp[2];
                stdio.printf("    [<%p>]\n", eip);
            }
            stdio.printf("=========================\n");
        }
        public static byte Inb(ushort port)
        {
            if (io.Port != port)
                io = new Cosmos.Core.IOPort(port);
            return io.Byte;

        }
        /* (This call code is from NoobOS, well actually no but it was Aurora's 
         * idea so I am going to make sure I give him credit). You must include
         * this too if you use this part
         * ---------------------------------------------------------------------
         * Copyright (C) 2012 NoobOS
         *
         * This program is free software; you can redistribute it and/or
         * modify it under the terms of the GNU General Public License
         * as published by the Free Software Foundation; either version 2
         * of the License, or (at your option) any later version.
         * 
         * This program is distributed in the hope that it will be useful,
         * but WITHOUT ANY WARRANTY; without even the implied warranty of
         * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
         * GNU General Public License for more details.
         * 
         * You should have received a copy of the GNU General Public License
         * along with this program; if not, write to the Free Software
         * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
         */
        class Caller
        {
            [PlugMethod(Assembler = typeof(plugCall))]
            public void CallCode(uint address) { }
        }
        [Plug(Target = typeof(Caller))]
        class plugCall : AssemblerMethod
        {
            public override void AssembleNew(object aAssembler, object aMethodInfo)
            {
                new CPUx86.Mov { SourceReg = CPUx86.Registers.EBP, SourceDisplacement = 8, SourceIsIndirect = true, DestinationReg = CPUx86.Registers.EAX };
                new CPUx86.Call { DestinationReg = CPUx86.Registers.EAX };
                
            }
        }
        
    }
}

