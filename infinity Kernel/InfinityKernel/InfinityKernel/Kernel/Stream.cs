using System;
using System.Collections.Generic;

namespace GruntyOS.IO
{
    public static class FileMode
    {
        public const byte RD_ONLY = 1;
        public const byte WR_ONLY = 2;
        public const byte RD_WR = 4;
        public const byte APPEND = 8;
        public const byte CREATE = 16;
        public const byte NO_SEEK = 32;
    }
	public class Stream
	{

        public bool modeRead = true;
        public bool modeWrite = true;
        public bool modeAppend = false;
        public bool supportSeek = true;
        public int Descriptor;
        private uint _pointer = 0;
        public uint Length;
        private static List<Stream> openedStreams
        {
            get
            {
                return Kernel.currentDomain.openedStreams;
            }
            set
            {
                Kernel.currentDomain.openedStreams = value;
            }
        }
		public static Stream fromDescriptor(int desc,byte mode = 0)
		{
            if (openedStreams.Count >= desc)
            {
                Stream stm = openedStreams[desc];
                if (mode != 0)
                    stm.setModes(mode);
                return stm;
            }
            else return null;
		}
        public uint Pointer
        {
            get
            {
                if (supportSeek)
                    return _pointer;
                else
                    return 0;
            }
            set
            {
                if (supportSeek)
                    _pointer = value;
                
            }
        }
        bool GetBit( byte b, int bitNumber)
        {
            return (b & (1 << bitNumber - 1)) != 0;
        }
        public static void reassign(int des, Stream newstream)
        {
            openedStreams[des] = newstream;
        }
        protected void setModes(byte modes)
        {
            if (GetBit(modes, 0))
            {
                modeRead = true;
                modeWrite = false;
            }
            if (GetBit(modes, 1))
            {
                modeRead = false;
                modeWrite = true;
            }
            if (GetBit(modes, 2))
            {
                modeRead = true;
                modeWrite = true;
            }

            if (GetBit(modes, 3))
            {
                this.Pointer = Length;
            }
            if (GetBit(modes, 5))
            {
                supportSeek = false;
            }
        }
		protected void Register(byte modes = 4)
		{

			this.Descriptor = openedStreams.Count;
            this.setModes(modes);
			openedStreams.Add(this);
            //stdio.printf
		}
        public virtual void ReadBytes(int length,byte[] dat)
        {
            if (!modeRead)
                throw new Exception("Can not read from stream!");
            for (int i = 0; i < length; i++)
            {
                dat[i] = (byte)Read();
            }
        }
        public void Write(byte data)
		{
            if(!modeWrite)
                 throw new Exception("Can not write to stream!");
			openedStreams[Descriptor].writeByte(Pointer,data);
			Pointer++;
		}
		public int Read()
        {
            if (!modeRead)
                throw new Exception("Can not read from stream!");
			Pointer++;
			return openedStreams[Descriptor].readByte(Pointer - 1);
		}
		public void Close()
		{
			openedStreams[Descriptor].onClose();
			for(int i = 0; i < openedStreams.Count;i++)
			{
                if (i == Descriptor)
                    openedStreams[i] = null;
			}
		}
		public virtual void onClose()
		{
		}
		public virtual int readByte(uint ptr)
		{
			return -1;
		}
        
		public virtual void writeByte(uint ptr,byte dat)
		{
		}
		
	}
}

