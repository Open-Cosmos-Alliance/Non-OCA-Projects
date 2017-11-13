using System;
namespace GruntyOS.IO.Devices
{
    public class randomDev : Device
    {
        public randomDev()
        {
            this.Name = "random";
            this.Type = deviceType.other;
        }
        public override Stream Open(int modes = 4)
        {
            return new randomStream();
        }
    }
    public class randomStream : Stream
    {
        public randomStream()
        {
            base.Register();
        }
        public override void writeByte(uint ptr, byte data)
        {

        }
        public override int readByte(uint ptr)
        {
            return (byte)(int)(ptr | (uint)(Cosmos.Hardware.RTC.Second));
        }
    }
}

