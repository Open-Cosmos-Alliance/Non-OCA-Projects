using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Core.Processes.MemoryMan;
using Console = DuNodes_Core.Terminal.Console;
using Sys = Cosmos.System;
namespace DuNodes.Base
{
    public static class Init
    {
        public static void Initialisation(Kernel kernel)
        {
            Console.WriteLine(".Booting DuNodes Alpha 0.1 R01", ConsoleColor.Blue, true);
            Console.WriteLine("..Checking prerequisites", ConsoleColor.Blue, true);
            Console.WriteLine("...RAM : "+ DuNodes_Core.Extensions.KernelExtensions.GetMemory(kernel) +"", ConsoleColor.Blue, true);
            if (DuNodes_Core.Extensions.KernelExtensions.GetMemory(kernel) < 20)
            {
                Console.WriteLine("Minimum 20MB RAM TO LAUNCH", ConsoleColor.Red);
                DuNodes_Core.Extensions.KernelExtensions.SleepSeconds(kernel, 5);
                DuNodes_Core.Extensions.KernelExtensions.Shutdown(kernel);
            }
            Console.WriteLine("....Creating Env", ConsoleColor.Blue, true);
            DuNodes_Core.Env.Kernel = kernel;
            //Load everything we want before creating memmanger
            Console.WriteLine("....", ConsoleColor.Blue, true);
            MemoryManager.Init();
           
        }
    }
}
