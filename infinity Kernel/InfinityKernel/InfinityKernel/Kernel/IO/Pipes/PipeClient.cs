using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS.IO.Pipes
{
    public class PipeClient : Stream
    {
        private Stream Server;
        public PipeClient(int desc)
        {
            Server = Stream.fromDescriptor(desc);
            base.Register(FileMode.WR_ONLY | FileMode.NO_SEEK);
        }
        public override void writeByte(uint ptr, byte data)
        {
            Server.Write(data);
        }
    }
}
