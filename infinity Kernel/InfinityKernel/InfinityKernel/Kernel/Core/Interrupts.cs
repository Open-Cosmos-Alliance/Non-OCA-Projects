using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys = Cosmos.System;
using Cosmos.IL2CPU.Plugs;
using Cosmos.Core;
using Cosmos;

namespace GruntyOS
{
    public delegate void interupt_Handler(ref Cosmos.Core.INTs.IRQContext aContext);
    public unsafe partial class Kernel
    {
        public static interupt_Handler[] interruptHandlers = new interupt_Handler[0xFF];
    }
}
namespace GruntyOS.Core
{
   
    /*
     * This plugs some shitty code in the Cosmos lib
     * 
     */
    [Plug(Target = typeof(Cosmos.Core.INTs))]
    public class INTs
    {
        static bool already = false;
        public static void HandleInterrupt_Default(ref Cosmos.Core.INTs.IRQContext aContext)
        {
            Kernel.interruptHandlers[aContext.Interrupt](ref aContext);
        }
        public static void HandleInterrupt_00(ref Cosmos.Core.INTs.IRQContext aContext)
        {
            Kernel.Panic(aContext);
        }
        public static void HandleInterrupt_01(ref Cosmos.Core.INTs.IRQContext aContext)
        {
            Kernel.Panic(aContext);
        }
        public static void HandleInterrupt_02(ref Cosmos.Core.INTs.IRQContext aContext)
        {
            Kernel.Panic(aContext);
        }
        public static void HandleInterrupt_03(ref Cosmos.Core.INTs.IRQContext aContext)
        {
            Kernel.Panic(aContext);
        }
        public static void HandleInterrupt_04(ref Cosmos.Core.INTs.IRQContext aContext)
        {
            Kernel.Panic(aContext);
        }
        public static void HandleInterrupt_05(ref Cosmos.Core.INTs.IRQContext aContext)
        {
            Kernel.Panic(aContext);
        }
        public static void HandleInterrupt_06(ref Cosmos.Core.INTs.IRQContext aContext)
        {
            Kernel.Panic(aContext);
        }
        public static void HandleInterrupt_07(ref Cosmos.Core.INTs.IRQContext aContext)
        {
            Kernel.Panic(aContext);
        }
        public static void HandleInterrupt_08(ref Cosmos.Core.INTs.IRQContext aContext)
        {
            Kernel.Panic(aContext);
        }
        public static void HandleInterrupt_09(ref Cosmos.Core.INTs.IRQContext aContext)
        {
            Kernel.Panic(aContext);
        }
        public static void HandleInterrupt_0A(ref Cosmos.Core.INTs.IRQContext aContext)
        {
            Kernel.Panic(aContext);
        }
        public static void HandleInterrupt_0B(ref Cosmos.Core.INTs.IRQContext aContext)
        {
            Kernel.Panic(aContext);
        }
        public static void HandleInterrupt_0C(ref Cosmos.Core.INTs.IRQContext aContext)
        {
            Kernel.Panic(aContext);
        }
        public static void HandleInterrupt_0D(ref Cosmos.Core.INTs.IRQContext aContext)
        {
            Kernel.Panic(aContext);
        }
    }
 
}
