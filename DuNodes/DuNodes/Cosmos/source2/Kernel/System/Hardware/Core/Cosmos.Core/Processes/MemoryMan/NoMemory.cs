using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Core.Processes.MemoryMan.Block;

namespace Cosmos.Core.Processes.MemoryMan
{
    class NoMemory
    {
        public static uint ClearMemory(uint startCore, uint uEndCore)
        {
            ///TODO : DEBUG MODE
            Console.WriteLine("startcore : " + startCore + " uEndCore : " + uEndCore + " Heap.mlengh" + Heap.mLength + " Heap.mStartAddress" + Heap.mStartAddress + " Heap.mStart" + Heap.mStart + " Heap other" + Heap.mEndOfRam);
           // Global.CPU.ZeroFill(uEndCore, Heap.mLength - uEndCore);
            Heap.mStartAddress = Heap.mStartAddress - (Heap.mLength - uEndCore);
            Heap.mLength = Heap.mLength - (Heap.mLength - uEndCore);
            return (Heap.mLength - uEndCore);
        }
    }
}
