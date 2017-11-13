using System;
namespace GruntyOS.IO.Devices
{
    public class ramDev : Device
    {
        private static byte Count = 1;
        public ramDev()
        {
            this.Name = "ram" + Count.ToString();
            Count++;
            this.Type = deviceType.blockDevice;
        }
        public override Stream Open(int modes = 4)
        {
            return new ramStream();
        }
    }
    public class ramStream : Stream
    {
        private byte[] disk;
        public ramStream()
        {
            disk = new byte[0xFFFFF];

            base.Register();
        }
        public override void writeByte(uint ptr, byte data)
        {
            disk[ptr] = data;
        }
        public override int readByte(uint ptr)
        {
            if (ptr < disk.Length)
                return disk[ptr];
            else
                return -1;
        }
    }
}

