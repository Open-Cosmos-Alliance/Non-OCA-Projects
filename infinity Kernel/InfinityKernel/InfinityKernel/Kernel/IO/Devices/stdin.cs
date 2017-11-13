using System;
using Cosmos.Hardware;

namespace GruntyOS.IO.Devices
{
	public class stdin : Device
	{
        public stdin()
        {
            this.Name = "stdin";
            this.Type = deviceType.charDevice;
        }
        public override Stream Open(int modes = 4)
		{
            return stdio.In;
		}
	}
	public class stdinStream : Stream
	{

        private Stream TTY;
		public stdinStream(string tty)
		{
            TTY = stdio.fopen(tty);
			this.modeWrite = false;
			base.Register();
		}

        public override void ReadBytes(int length,byte[] dat)
        {
            string data = stdio.readln();
            for (int i = 0; i < length; i++)
            {
                dat[i] = (byte)data[i];
            }
        }
		public override int readByte (uint ptr)
		{
            return TTY.Read();
		}
	}
}

