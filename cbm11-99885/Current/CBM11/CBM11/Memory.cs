using System;
//using System.Data;

namespace Emu6502
{
	/// <summary>
	/// Summary description for Memory.
	/// </summary>
	public class Memory
	{
        public ConsoleIO consoleIO = null;

		private byte[] ram = new byte[65536];

		public byte[] GetBytes
		{
			get 
			{
				return ram;
			}
		}

		#region Constructors

        public Memory(ref ConsoleIO io)
		{
            //Provides for IO memory mapping
            consoleIO = io;
		}


		#endregion

		#region Public Methods

		public byte read(ushort location)
		{
			return ram[location];
		}

		public void write(ushort location, byte val)
		{
            if (location >= 1000 && location <= 2000)
            {
                int offset = location - 1000;
                consoleIO.Poke((ushort)offset, val);
                ram[location] = val;
            }
            else
            {
                switch (location)
                {
                    default:
                        ram[location] = val;
                        break;
                }
            }

		}

		public void copy(ushort start, byte[] bytes)
		{
			int x=0;
			ushort end = (ushort)(start + (ushort)bytes.GetUpperBound(0));
			for(;start<=end;start++)
				ram[start] = bytes[x++];
		}

		public void map(ref ConsoleIO io, ushort startAddr)
        {
            consoleIO = io;
        }

		#endregion
	}
}
