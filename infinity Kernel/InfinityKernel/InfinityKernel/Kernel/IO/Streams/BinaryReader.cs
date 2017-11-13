using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS.IO
{
    public class BinaryReader
    {
        public Stream BaseStream;
        public BinaryReader(Stream stm)
        {
            BaseStream = stm;
        }
        public byte ReadByte()
        {
            return (byte)BaseStream.Read();
        }
        public int ReadInt32()
        {
            int val = BitConverter.ToInt32(new byte[] { (byte)BaseStream.Read(), (byte)BaseStream.Read(), (byte)BaseStream.Read(), (byte)BaseStream.Read() }, 0);
            return val;
        }
        public uint ReadUInt32()
        {
            uint val = BitConverter.ToUInt32(new byte[] { (byte)BaseStream.Read(), (byte)BaseStream.Read(), (byte)BaseStream.Read(), (byte)BaseStream.Read() },0);
            return val;
        }
        public void Close()
        {
            BaseStream.Close();
        }
        public string ReadString()
        {
            byte length = (byte)BaseStream.Read();
            string Ret = "";
            for (int i = 0; i < length; i++)
            {
                Ret += ((char)BaseStream.Read()).ToString();
            }
            return Ret;
        }
    }
}
