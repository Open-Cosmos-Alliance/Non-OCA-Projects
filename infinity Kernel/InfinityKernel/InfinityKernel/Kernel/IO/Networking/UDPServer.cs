using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GruntyOS.IO;

namespace GruntyOS.IO.Networking
{

    public delegate void UDPPacketRecieved(byte[] data);
    public class UDPServer
    {

        private int length = 0;
        private byte[] UDPHeader = new byte[8];
        private int maxLength;
        private int sourcePort, destPort;
        private byte[] contents;
        private int dataPtr = 0;
        private Socket server;
        public UDPPacketRecieved onRead;
        public UDPServer(Socket sock)
        {
            server = sock;
            server.dataRecieved = dataRecieved;
        }
   
        private void parseHeader()
        {
            sourcePort = System.BitConverter.ToUInt16(UDPHeader, 0);
            destPort = System.BitConverter.ToUInt16(UDPHeader, 2);
            maxLength = System.BitConverter.ToUInt16(UDPHeader, 4);
            ushort checkSum = System.BitConverter.ToUInt16(UDPHeader, 6);
            byte[] ret = new byte[length];
        }
        private void dataRecieved(byte data)
        {
            if (length < 8)
            {
                UDPHeader[length] = data;
                length++;
                if (length == 8)
                {
                    parseHeader();
                    contents = new byte[maxLength];
                    
                }
            }
            else
            {
                contents[dataPtr] = data;
                dataPtr++;
                if (dataPtr == maxLength)
                {
                    if(destPort == server.Port)
                    onRead(contents);
                    dataPtr = 0;
                    length = 0;
                }
            }
        }
        
    }
}
