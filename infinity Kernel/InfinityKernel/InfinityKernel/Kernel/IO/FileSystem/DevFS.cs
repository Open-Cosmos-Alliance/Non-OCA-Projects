
/*
 * This is the device filesystem that is  
 * mounted in /dev. Be carefull, this is 
 * very important....
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GruntyOS.IO.Pipes;

namespace GruntyOS.IO.Filesystem
{
    public class DevFS : Filesystem
    {
        private class DevFifo : Device
        {
            public PipeServer server;
            public DevFifo(PipeServer srv, string name)
            {
                server = srv;
                this.Name = name;

            }
            public override Stream Open(int modes = 4)
            {
                return new PipeClient(server.Descriptor);
            }
        }
        private class DevSocket : Device
        {
            public GruntyOS.IO.Networking.Socket server;
            public DevSocket(GruntyOS.IO.Networking.Socket srv, string name)
            {
                server = srv;
                this.Name = name;

            }
            public override Stream Open(int modes = 4)
            {
                return Stream.fromDescriptor(server.Descriptor);
            }
        }

        public List<Device> Devices = new List<Device>();
        public override void Chmod(string file, string perms)
        {
            throw new NotImplementedException();
        }
        public override void Chown(string file, string owner)
        {
            throw new NotImplementedException();
        }
        public override GruntyOS.IO.Networking.Socket mksocket(string file)
        {
            GruntyOS.IO.Networking.Socket ret = new  GruntyOS.IO.Networking.Socket();
            DevSocket fifo = new DevSocket(ret, file);
            this.Devices.Add(fifo);
            return ret;
        }
        public override PipeServer mkfifo(string name)
        {
            PipeServer ret = new PipeServer();
            DevFifo fifo = new DevFifo(ret, name);
            this.Devices.Add(fifo);
            return ret;
        }
        public void Init()
        {
            /*
             *  Only the first 3 are important, the rest
             *  are just misc stuff...
             */

           
        }
        public override List<FileInfo> List(string dir)
        {
            List<FileInfo> ret = new List<FileInfo>();
            for (int i = 0; i < Devices.Count; i++)
            {
                FileInfo f = new FileInfo();
                if (Devices[i] is DevFifo)
                    f.Type = fileType.Pipe;
                else
                    f.Type = fileType.BlockDevice;
                f.Name = Devices[i].Name;
                ret.Add(f);
            }
            return ret;
        }
        public override Stream openFile(string file,int modes)
        {
            for (int i = 0; i < Devices.Count; i++)
            {

                if (Devices[i].Name == file)
                    return Devices[i].Open(modes);
            }
            Console.WriteLine("fail");
            throw new Exception("File not found");
        }
        public override void Unlink(string file)
        {
            throw new NotImplementedException();
        }
    }
}
