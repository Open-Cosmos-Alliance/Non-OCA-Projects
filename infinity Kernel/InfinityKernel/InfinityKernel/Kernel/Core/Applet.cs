using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GruntyOS.IO;

namespace GruntyOS
{
    public static partial class Kernel
    {
        public static void RunApplet(Applet app,string[] args)
        {
            app.Run(args, stdio.In, stdio.Out, stdio.Err);
        }
    }
    public class Applet
    {
        protected Stream stdin;
        protected Stream stdout;
        protected Stream stderr;
        protected InfinityAppDomain Domain;
        public string Name;
        protected virtual void Run(string[] args)
        {
        }
        public void Run(string[] args, Stream In, Stream Out,Stream Err)
        {
            this.Domain = InfinityAppDomain.CreateDomain(Name);
            uint outPos = stdio.Out.Pointer;
            Kernel.SetAppDomain(this.Domain);
            new GruntyOS.IO.Devices.stdoutStream("/dev/tty1");
            new GruntyOS.IO.Devices.stdoutStream("/dev/tty1");
            new GruntyOS.IO.Devices.stdoutStream("/dev/tty1");
            Kernel.currentDomain.openedStreams[0] = new GruntyOS.IO.Devices.stdinStream("/dev/tty1");
            Kernel.currentDomain.openedStreams[1] = new GruntyOS.IO.Devices.stdoutStream("/dev/tty1");
            Kernel.currentDomain.openedStreams[1].Pointer = outPos;
            stdio.Out = Kernel.currentDomain.openedStreams[1];
            this.Run(args);
        }

        protected void printf(string format, params object[] args)
        {
            stdio.printf(format, args);
        }
    }
   
}
