using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GruntyOS.IO;

namespace GruntyOS.IO
{
    public class TextStream : Stream
    {
        private string accum = "";
        public TextStream()
        {
            this.modeRead = true;
            this.modeWrite = true;
            Register();
        }
        public TextStream(string text)
        {
            this.modeWrite = false;
        }
        
        public override void writeByte(uint ptr, byte data)
        {
            accum += data;
        }
        public override int readByte(uint ptr)
        {
            if (ptr > accum.Length)
                return -1;
            return (int)(char)accum[(int)ptr];
        }
    }
}
