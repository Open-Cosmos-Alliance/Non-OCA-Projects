using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS.IO
{
    public abstract class ioStream
    {
        public int Position;
        public byte[] Data;
        public  void Write(byte i)
        {
            Data[Position] = i;
            Position++;
        }
        public byte Read()
        {
            Position++;
            return Data[Position - 1];
        }
       
        public virtual void Close()
        {

        }
        bool Resize = true;
        public void init(int size)
        {
            Resize = false;
            Data = new byte[size];
        }
        
    }
    public class MemoryStream : ioStream
    {
        public override void Close()
        {
        }
        public MemoryStream(int size)
        {
            base.init(size);
        }
    }
    public class FileStream : ioStream
    {
        private string fname = "";
        string fmode = "";
        public FileStream(string fname,string mode)
        {
            this.fname = fname;
            this.init(7000);
            fmode = mode;
            if (mode == "r")
            {
                this.init(GruntyOS.HAL.FileSystem.Root.readFile(fname).Length);
                this.Data = GruntyOS.HAL.FileSystem.Root.readFile(fname);
                return;
            }
        }
        public override void Close()
        {
            if (fmode == "w")
            {
                MemoryStream ms = new MemoryStream(this.Position);
                for (int i = 0; i < this.Position; i++)
                {
                    ms.Write(this.Data[i]);
                }
                this.Data = ms.Data;
                GruntyOS.HAL.FileSystem.Root.saveFile(this.Data, fname);
            }
             

        }
    }
}
