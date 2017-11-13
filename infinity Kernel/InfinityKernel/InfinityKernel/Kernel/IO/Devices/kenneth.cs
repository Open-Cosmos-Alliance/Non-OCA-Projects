using System;
namespace GruntyOS.IO.Devices
{
    public class zdimensionDev : Device
    {
        public zdimensionDev()
        {
            this.Name = "zdimension";
            this.Type = deviceType.other;
        }
        public override Stream Open(int modes = 4)
        {
            return new zdimensionStream();
        }
    }
    public class zdimensionStream : Stream
    {
        public zdimensionStream()
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

