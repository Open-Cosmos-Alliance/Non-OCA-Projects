using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS.IO
{

    public class TextReader
    {
        public int pos = 0;
        private char[] dat;
        public int Length;
        public char Read()
        {
            pos++;
            return dat[pos - 1];
        }
        public char Peek()
        {
            if (pos < dat.Length)
            {
                return dat[pos];
            }
            else
            {
                return (char)255;
            }
        }
        public TextReader(string str)
        {
            dat = str.ToCharArray();
            Length = dat.Length;
        }
    }
    public class BinaryReader
    {
        public ioStream BaseStream;
        public BinaryReader(ioStream stream)
        {
            stream.Position = 0;
            this.BaseStream = stream;
        }
        public int ReadInt32()
        {
            int val = BitConverter.ToInt32(BaseStream.Data, (int)BaseStream.Position);
            BaseStream.Position += 4;
            return val;
        }
        public void Close()
        {
            BaseStream.Close();
        }
        public string ReadString()
        {
            byte length = BaseStream.Read();
            string Ret = "";
            for (int i = 0; i < length; i++)
            {
                Ret += ((char)BaseStream.Read()).ToString();
            }
            return Ret;
        }
    }
    public class File
    {
        public static string Open(string fname)
        {
            string str = "";
            foreach (byte b in GruntyOS.HAL.FileSystem.Root.readFile(fname))
            {
                str += ((char)b).ToString();
            }
            return str;

        }
        public static void Delete(string File)
        {
            GruntyOS.HAL.FileSystem.Root.Delete(File);
        }
        public static void Chmod(string name, string chmod)
        {
            ((GruntyOS.HAL.GLNFS)GruntyOS.HAL.FileSystem.Root).Chmod(name, chmod);
        }
        public static void Save(string name, string text)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(name, "w"));
            foreach (byte b in text)
            {
                bw.BaseStream.Write(b);
            }
            bw.BaseStream.Close();
        }
    }
    class BinaryWriter
    {
        public ioStream BaseStream;
        public void Write(byte data)
        {
            BaseStream.Write(data);
        }
        public void Write(char data)
        {
            BaseStream.Write((byte)data);
        }
        public void Write(int data)
        {
            byte[] bits = BitConverter.GetBytes(data);
            foreach (byte b in bits)
            {
                BaseStream.Write(b);
            }
        }
        public void Write(short data)
        {
            byte[] bits = BitConverter.GetBytes(data);
            foreach (byte b in bits)
            {
                BaseStream.Write(b);
            }
        }
        public void Write(byte[] data)
        {
            foreach (byte b in data)
            {
                BaseStream.Write(b);
            }
        }
        public void Write(string data)
        {
            BaseStream.Write((byte)data.Length);
            foreach (byte b in data)
            {
                BaseStream.Write(b);
            }
        }

        public BinaryWriter(ioStream file)
        {

            BaseStream = file;
        }
    }
}
