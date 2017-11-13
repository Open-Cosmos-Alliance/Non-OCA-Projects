using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using Emu6502;

namespace CBM11
{
    public class Kernel : Sys.Kernel
    {
        protected override void BeforeRun()
        {

        }

        protected override void Run()
        {
            Computer comp = new Computer();
            comp.Start();          
        }
    }
}
