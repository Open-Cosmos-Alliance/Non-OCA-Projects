using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS.IO.Networking
{

    public delegate void pipeDataRecieved(byte b);
    public class Socket : Stream
    {
        public pipeDataRecieved dataRecieved;
        public ushort Port;
        public Socket()
        {
            this.modeRead = false;
            base.Register(FileMode.RD_WR | FileMode.NO_SEEK);
        }
        public override int readByte(uint ptr)
        {
            return base.readByte(ptr);
        }
        public override void writeByte(uint ptr, byte data)
        {
            this.dataRecieved(data);
        }
    }
}
