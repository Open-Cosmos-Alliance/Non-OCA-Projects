using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GruntyOS.Core;
namespace GruntyOS
{
    public static unsafe class MemoryManager
    {
        private static List<MemoryBlock> memory = new List<MemoryBlock>();
        public static void free(void* data)
        {
            for (int i = 0; i < memory.Count; i++)
            {
                if (memory[i].Address <= (uint)data && (uint)(memory[i].Size + memory[i].Address) >= (uint)data)
                {
                    memory[i].Free = true;
                    break;
                }
            }
        }
        public static void* malloc(size_t size)
        {
            for (int i = 0; i < memory.Count; i++)
            {
                if (memory[i].Free && memory[i].Size >= size.value)
                {
                    memory[i].Free = false;
                    return (void*)memory[i].Address;
                }
            }
            MemoryBlock mb = new MemoryBlock(size.value);
            mb.Address = Cosmos.Core.Heap.MemAlloc(size.value);
            memory.Add(mb);
            return (void*)mb.Address;
        }
    }
}
