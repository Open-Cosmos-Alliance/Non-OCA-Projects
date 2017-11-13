using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GruntyOS.IO;

namespace GruntyOS.IO.Networking
{
    public delegate void UDPDataRecieved(byte[] data);
    public class UDPStream : Stream
    {
        private Stream baseStream;
        private ushort port;
        private ushort thisPort;
        private static ushort thePort = 0;
        public UDPDataRecieved onDataRecieve;
        public UDPStream(Stream str,ushort aPort)
        {
            // Since this isnt for IP I dont think that the port really matters...
            thisPort = ++thePort;
            port = aPort;
            baseStream = str;
            base.Register();
        }
        public byte[] readPacket()
        {
         
            ushort sourcePort = System.BitConverter.ToUInt16(new byte[] { (byte)baseStream.Read(), (byte)baseStream.Read() }, 0);
            ushort destPort = System.BitConverter.ToUInt16(new byte[] { (byte)baseStream.Read(), (byte)baseStream.Read() }, 0);
            ushort length = System.BitConverter.ToUInt16(new byte[] { (byte)baseStream.Read(), (byte)baseStream.Read() }, 0);
            ushort checkSum = System.BitConverter.ToUInt16(new byte[] { (byte)baseStream.Read(), (byte)baseStream.Read() }, 0);
            byte[] ret = new byte[length];
            for (int i = 0; i < length; i++)
            {
                int dat = baseStream.Read();
                if (dat == -1)
                    throw new Exception("End of stream!");
                ret[i] = (byte)dat;
            }
            return ret;
                
        }
        public virtual void writeData(byte[] data)
        {
            byte[] header = new byte[8 + data.Length];
            for (int i = 0; i < 2; i++)
                header[i] = BitConverter.GetBytes(thisPort)[i];
            for (int i = 0; i < 2; i++)
                header[i + 2] = BitConverter.GetBytes(port)[i];
            for (int i  =0; i < 2; i++)
                header[i + 4] = BitConverter.GetBytes(data.Length)[i];
            for (int i = 0; i < 2; i++)
                header[i + 6] = 0; // ignore checksum

            for (int d = 0; d < data.Length; d++)
            {
                header[8 + d] = data[d];
                ++d;
            }
            for (int i = 0; i < header.Length; i++)
                baseStream.Write(header[i]);
            if (onDataRecieve != null)
                onDataRecieve(data);

        }
        public override void writeByte(uint ptr, byte data)
        {
            writeData(new byte[] { data });
        }
        public override int readByte(uint ptr)
        {
            return base.readByte(ptr);
        }
    }
}
