using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS.IO.Pipes
{
    /*
     * Since there is not multithreading, I am using this 
     * delegate for reading data from the server....
     */
    public delegate void pipeDataRecieved(byte b);
    public class PipeServer : Stream
    {
        public pipeDataRecieved dataRecieved ;
        public PipeServer()
        {
            this.modeRead = false;
            base.Register(FileMode.WR_ONLY | FileMode.NO_SEEK);
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
