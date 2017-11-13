using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS.IO
{
    public unsafe class MemoryStream :Stream
    {
        byte* base_ptr;
        public MemoryStream(byte* data)
        {
            base_ptr = data;
        }
        public override void writeByte(uint ptr, byte dat)
        {
            base_ptr[ptr] = dat;
        } 
        public override int readByte(uint ptr)
        {
            return base_ptr[ptr];
        }
    }
}
