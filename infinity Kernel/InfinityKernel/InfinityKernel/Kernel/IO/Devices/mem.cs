using System;
namespace GruntyOS.IO.Devices
{
    public unsafe class memDev : Device
    {
        public memDev()
        {
            this.Name = "mem";
            this.Type = deviceType.blockDevice;
        }
        public override Stream Open(int modes = 4)
        {
            return new MemoryStream((byte*)0);
        }
    }
}

