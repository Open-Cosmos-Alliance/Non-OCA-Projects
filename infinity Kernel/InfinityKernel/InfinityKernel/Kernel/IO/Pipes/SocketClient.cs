using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS.IO.Pipes
{
    public class SocketClient : Stream
    {
        private Stream Server;
        public SocketClient(int desc)
        {
            Server = Stream.fromDescriptor(desc);
            this.modeRead = false;
            base.Register(FileMode.RD_WR | FileMode.NO_SEEK);
        }
        public override int readByte(uint ptr)
        {
            return Server.Read();
        }
        public override void writeByte(uint ptr, byte data)
        {
            Server.Write(data);
        }
    }
}
