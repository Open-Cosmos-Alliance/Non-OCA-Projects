using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Core.Processes.MemoryMan.Block;

namespace Cosmos.Core.Processes.MemoryMan
{
    public static class MemoryManager
    {
        //Take corememory allocated in bootstrap and lock everything before it.
        //For everythings that os launched after goes in process and do have memory manager
        //For every unknow call (not from process), put them in core memory
        private static bool isInitialized { get; set; }
        private static CoreMemory coreMemory { get; set; }
        private static List<Process> processes { get; set; }

        public static void Init()
        {
            coreMemory = new CoreMemory();
            coreMemory.startCore = 0;
            coreMemory.endCore = Heap.mStartAddress;
            Console.WriteLine("Starting / Init Memory Manager " + Heap.mStartAddress + " " + Heap.mStart + " " + Heap.mLength);
            isInitialized = true;
        }

        public static uint AllocNewObject(uint aSize)
        {
            if (isInitialized)
            {
                return Heap.MemAlloc(aSize);
            }
            else
            {
                return Heap.MemAlloc(aSize);
            }
        }

        public static void IncRefCount(uint aObject)
        {
            //
        }
        public static void DecRefCount(uint aObject)
        {
            //
        }

        public static uint ClearRam()
        {
            Console.WriteLine("init : " + isInitialized);
            return NoMemory.ClearMemory(coreMemory.startCore, coreMemory.endCore);

        }
    }
}
