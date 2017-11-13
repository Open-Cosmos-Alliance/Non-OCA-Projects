using System;
namespace GruntyOS.IO.Devices
{
	public class nullDev : Device
	{
		public nullDev()
		{
            this.Name = "null";
            this.Type = deviceType.other;
		}
        public override Stream Open(int modes = 4)
		{
			return new nullStream();
		}
	}
	public class nullStream : Stream
	{
		public nullStream()
		{
			base.Register();
		}
		public override void writeByte (uint ptr, byte data)
		{
			
		}
		public override int readByte (uint ptr)
		{
			return 0;
		}
	}
}

