using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS.Core
{
    public class DeviceManager : Service
    {
        public override bool Init()
        {
            this.Name = "DevMan";
            this.priority = Priority.HIGH;
            printk("<6>Creating device filesystem\n");
            Kernel.devFS = new IO.Filesystem.DevFS();
            printk("<6>Creating pseudo-devices\n");
            GruntyOS.IO.Devices.tty term = new GruntyOS.IO.Devices.tty();
            GruntyOS.IO.Devices.stdin In = new GruntyOS.IO.Devices.stdin();
            GruntyOS.IO.Devices.stdout Out = new GruntyOS.IO.Devices.stdout();
            GruntyOS.IO.Devices.stderr Err = new GruntyOS.IO.Devices.stderr();
            GruntyOS.IO.Devices.nullDev Null = new GruntyOS.IO.Devices.nullDev();
            GruntyOS.IO.Devices.fullDev Full = new GruntyOS.IO.Devices.fullDev();
            GruntyOS.IO.Devices.randomDev Random = new GruntyOS.IO.Devices.randomDev();
            GruntyOS.IO.Devices.zeroDev zero = new GruntyOS.IO.Devices.zeroDev();
            GruntyOS.IO.Devices.keyboardDev keyboard = new IO.Devices.keyboardDev();
            GruntyOS.IO.Devices.memDev RAM = new IO.Devices.memDev();
            GruntyOS.IO.Devices.cupDev cupDev = new IO.Devices.cupDev(); // VERY IMPORTANT! DO NOT REMOVE!!!!!
            /*
             * This line is VERY important, as it creates the standard 
             * streams need for console I/O. It also needs to be done
             * first before anyother streams are opened (well should
             * be done first)
             */

            Kernel.devFS.Devices.Add(In);       //                      /dev/stdin
            Kernel.devFS.Devices.Add(Out);      //                      /dev/stdout
            Kernel.devFS.Devices.Add(Err);      //                      /dev/stderr
            Kernel.devFS.Devices.Add(Null);     //                      /dev/null 
            Kernel.devFS.Devices.Add(zero);     //                      /dev/zero
            Kernel.devFS.Devices.Add(Full);     //                      /dev/full
            Kernel.devFS.Devices.Add(Random);   //                      /dev/random
            Kernel.devFS.Devices.Add(keyboard); //                      /dev/keyboard
            Kernel.devFS.Devices.Add(term);     //                      /dev/tty0
            Kernel.devFS.Devices.Add(RAM);      //                      /dev/mem
            for (int i = 0; i < 8; i++)
                Kernel.devFS.Devices.Add(new GruntyOS.IO.Devices.tty());
            Kernel.devFS.Devices.Add(cupDev);   //                      /dev/cup
            /*
             *  Now we also need some room in RAM for the init
             *  rd and other ram disks. Lets create 4 ram disks...
             */
            printk("<5>Creating RAM disks\n");
            for (int i = 0; i < 4; i++)
            {
                Kernel.devFS.Devices.Add(new GruntyOS.IO.Devices.ramDev());
            }
            Kernel.Root.Mount(Kernel.devFS, "/dev");
            /*
             * Now that there is a TTY device that we can write 
             * too lets make some new std streams that point
             * to our new tty
             */
            stdio.In = new GruntyOS.IO.Devices.stdinStream("/dev/tty1");
            stdio.Out = new GruntyOS.IO.Devices.stdoutStream("/dev/tty1");
            stdio.Err = new GruntyOS.IO.Devices.errStream("/dev/tty1");
            stdio.Out.Pointer = GruntyOS.IO.Stream.fromDescriptor(1).Pointer;
            GruntyOS.IO.Stream.reassign(0, stdio.In);
            GruntyOS.IO.Stream.reassign(1, stdio.Out);
            GruntyOS.IO.Stream.reassign(2, stdio.Err);
            return true;
        }
        
    }
}
