using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using GruntyOS.IO;
using GruntyOS.IO.Filesystem;
using GruntyOS;

namespace InfinityKernel
{
    public unsafe class Boot : Sys.Kernel
    {
        protected override void BeforeRun()
        {
            Kernel.registerService(new GruntyOS.Core.DeviceManager());
            Kernel.registerService(new GruntyOS.Core.syslog());
            global::GruntyOS.Kernel.registerService(new GruntyOS.HAL.ATA());
        }

        protected override void Run()
        {
           
            /* Very important:
             * We need to make sure there is something to display 
             * text too. So lets set up the standard streams....
             */
            
            
            // rootFS is the virtual filesystem, if it is not created
            // then nothing can be mounted, and no filesystems can 
            // exist....
            Cosmos.Hardware.VGAScreen vga = new Cosmos.Hardware.VGAScreen();
            vga.SetPaletteEntry(0, (byte)6, (byte)35, (byte)64);
            GruntyOS.Kernel.Init();
            byte* test = (byte*)stdio.malloc(34);
            stdio.printf("object1 is at: %d\n", (uint)test);
          
            byte* test2 = (byte*)stdio.malloc(24);
            stdio.printf("object2 is at: %d\n", (uint)test2);
            stdio.free(test);
            byte* otherStuff = (byte*)stdio.malloc(10);
            stdio.printf("The other crap is at: %d", (uint)otherStuff);
            while (true) ;
            GruntyOS.Kernel.RunApplet(new Gshell(), new string[] { "" });
  
            while (true)
            {
             //   Console.Write(((char)(byte)stream.Read()).ToString());
            }
        }
    }
}
