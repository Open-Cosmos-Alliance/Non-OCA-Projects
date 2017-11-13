using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GruntyOS.IO.Filesystem;
using GruntyOS.IO;
using GruntyOS.IO.Pipes;

namespace GruntyOS
{
    public unsafe static partial class Kernel
    {
        /// <summary>
        /// This does not actually 'shutdown' the OS, instead
        /// it uninitializes the kernel and puts the kernel 
        /// in a state that will prevent any sort of dataloss
        /// or corruption.
        /// </summary>
        public static void Shutdown()
        {
            printf("<6>Attempting to sync\n");
            Sync();
            printf("<6>Unmounting all filesystems!");
            List<RootFS.mountPoint> mps =  Root.getMountPoints();
            for (int i = 0; i < mps.Count; i++)
            {
                printf("<6>Forcing unmount %s!!!\n",mps[i].Path);
                Root.Umount(mps[i].Path, true);
            }
            printf("<6>Kernel ready for shutdown!\n");
        }
        /// <summary>
        /// Kernel initialization code, boots the kernel
        /// </summary>
        public static void Init()
        {
            setUID(0); // Make sure we are root
            setGID(0);
            InfinityAppDomain kernelDomain = InfinityAppDomain.CreateDomain("infinity");
            SetAppDomain(kernelDomain);
            /* Very important:
            * We need to make sure there is something to display 
            * text too. So lets set up the standard streams....
            */
            stdio.In = new GruntyOS.IO.Devices.ttyStream();
            stdio.Out = new GruntyOS.IO.Devices.ttyStream();
            stdio.Err = new GruntyOS.IO.Devices.ttyStream();
            /* 
             * We also need the kernel log, so make new instance of that too
             */
            Kernel.Log = new TextStream();
            // rootFS is the virtual filesystem, if it is not created
            // then nothing can be mounted, and no filesystems can 
            // exist....
            Root = new RootFS();
            /* 
             * System calls allow other programs to interact with the kernel,
             * so lets set those up too.
             */
            interruptHandlers[0x80] = GruntyOS.Core.syscalls.handleSyscall;
            stdio.printf("Welcome to \\[0;34mGrunty OS \\[0;36mInfinity\\[0m!\n\n");
            //stdio.fprintf(stdio.Out, "Creating virtual filesystem\n");
            printf("Creating root filesystem\n");

            InitServices();
            
        }
    }
}
