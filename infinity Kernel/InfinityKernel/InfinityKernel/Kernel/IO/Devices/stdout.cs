using System;
using GruntyOS.IO;

namespace GruntyOS.IO.Devices
{
	public class stdout: Device
	{
        public stdout()
        {
            this.Type = deviceType.charDevice;
        }
        public override Stream Open(int modes = 4)
		{
            return stdio.Out;
		}
	}
    public unsafe class stdoutStream : Stream
    {

        ushort* video_memory = (ushort*)0xB8000;
        private Stream TTY;
        public stdoutStream(string tty)
        {
            base.Register();
            TTY = stdio.fopen(tty);
        }
        public override int readByte(uint ptr)
        {
            return base.readByte(ptr);
        }
        public override void writeByte(uint ptr, byte data)
        {
            TTY.Pointer = this.Pointer;
            TTY.writeByte(ptr, data);
            this.Pointer = TTY.Pointer;
        }
    }
}

