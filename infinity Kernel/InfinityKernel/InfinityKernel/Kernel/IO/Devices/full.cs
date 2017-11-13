using System;
namespace GruntyOS.IO.Devices
{
    public class  fullDev : Device
    {
        public fullDev()
        {
            this.Name = "full";
            this.Type = deviceType.other;
        }
        public override Stream Open(int modes = 4)
        {
            return new fullStream();
        }
    }
    public class fullStream : Stream
    {
        public fullStream()
        {
            base.Register();
        }
        public override void writeByte(uint ptr, byte data)
        {

        }
        public override int readByte(uint ptr)
        {
            return -1;
        }
    }
}

