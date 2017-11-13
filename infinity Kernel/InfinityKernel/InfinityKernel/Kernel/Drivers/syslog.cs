using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GruntyOS.IO;
using GruntyOS.IO.Networking;
using GruntyOS.IO.Pipes;
namespace GruntyOS.Core
{
    public class syslog : Service
    {
        private Socket server;
        public void recievePacket(byte[] pack)
        {
            string msg = "";
            for (int i = 0; i < pack.Length; i++)
                msg += ((char)pack[i]).ToString();
            stdio.printf("Recieved data %s\n", msg);
            //recieveMessage(msg);
        }
        public syslog()
        {
            this.Name = "syslogd";
        }
        string msg = "";
        private void recieveMessage(string message)
        {
            string[] colums = message.Split(' ');
            string timeStamp = colums[0];
            string logHost = colums[1];
            string application = colums[2];
            string priorty = colums[3];
            string msgID = colums[4];
            stdio.printf("Time:%s \nLoghost:%s \nApplication:%s \nPriority: %s \nMessageID:%s\n", timeStamp, logHost, application, priorty, msgID);
            msg = "";
            // 5/2/4-20:3:4 Hello World! From the kernel!"
        }
        public override bool Init()
        {
            try
            {
                printk("<6>Creating syslog socket\n");
                server = Kernel.Root.mksocket("/dev/syslog");
                server.Port = 512;
                UDPServer Server = new UDPServer(server);
                Server.onRead = recievePacket;
                printk("<5>Syslog is listening on UDP port 512\n");
               // UDPStream udp = new UDPStream(server, 6666);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
