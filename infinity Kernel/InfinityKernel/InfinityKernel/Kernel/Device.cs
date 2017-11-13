
using System;

namespace GruntyOS.IO
{
    public enum deviceType
    {
        charDevice = 1,
        blockDevice = 2,
        other = 3
    }
	public abstract class Device
	{
		public string Name;
        public deviceType Type;
		public abstract Stream Open(int modes);
	}
}

