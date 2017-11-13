using System;

namespace GruntyOS.IO.Devices
{
	public class stderr : Device
	{
        public stderr()
        {
            this.Name = "stderr";
            this.Type = deviceType.charDevice;
        }
        public override Stream Open(int modes = 4)
		{
            return stdio.Err;
		}	
	}
	public class errStream : GruntyOS.IO.Stream
	{

        private Stream TTY;
		public errStream(string tty)
		{
            base.Register();
		}
		public override int readByte (uint ptr)
		{
			return Stream.fromDescriptor(1).readByte(ptr);
		}
		public override void writeByte (uint ptr, byte data)
		{
			
		}
	}
}

