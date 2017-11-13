using System;
using System.Collections.Generic;
using System.Text;
using DuNodes.Base;
using DuNodes_Core.Terminal.CommandManager;
using Sys = Cosmos.System;

using DuNodes_Core;
using DuNodes_Core.Extensions;
using Console = DuNodes_Core.Terminal.Console;


namespace DuNodes
{
    public class Kernel : Sys.Kernel
    {
        protected override void BeforeRun()
        {
            Init.Initialisation(this);
        }

        protected override void AfterRun()
        {
            base.AfterRun();
        }

        protected override void Run()
        {
            Command cmd = new Command();
            while (true)
            {
                Console.Write("DNodes # ");
                var input = Console.ReadLine();
                cmd.Handle(input);
            }
        }
    }
}
