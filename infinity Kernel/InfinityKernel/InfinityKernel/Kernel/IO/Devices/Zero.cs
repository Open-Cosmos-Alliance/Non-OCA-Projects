using System;
namespace GruntyOS.IO.Devices
{
    public class zeroDev : Device
    {
        public zeroDev()
        {
            this.Name = "zero";
            this.Type = deviceType.other;
        }
        public override Stream Open(int modes = 4)
        {
            return new zeroStream();
        }
    }
    public class zeroStream : Stream
    {
        public zeroStream()
        {
            base.Register();
        }
        public override void writeByte(uint ptr, byte data)
        {

        }
        public override int readByte(uint ptr)
        {
            return 0;
        }
    }
}

