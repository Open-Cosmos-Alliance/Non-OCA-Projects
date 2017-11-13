using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS.Core
{
    public unsafe class MemoryBlock
    {
        public uint Address;
        public bool Free = false;
        public uint Size;
        public MemoryBlock(uint size)
        {
            Size = size;
        }
        public byte this[uint index]
        {
            get
            {
                return ((byte*)Address)[index];
            }
            set
            {
                ((byte*)Address)[index] = value;
            }
        }
    }
}
