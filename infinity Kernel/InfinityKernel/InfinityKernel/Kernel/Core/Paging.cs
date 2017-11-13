using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.IL2CPU.Plugs;
using GruntyOS.IO.Filesystem;
using CPUx86 = Cosmos.Assembler.x86;
using GruntyOS.IO;
using GruntyOS.IO.Pipes;

namespace GruntyOS
{
    public unsafe static class Paging
    {
        public static uint* root;
        public static void Setup()
        {
            uint* page_directory = (uint*)(Cosmos.Core.Heap.MemAlloc(0x2000));
            uint i = 0;
            for (i = 0; i < 1024; i++)
            {
                //attribute: supervisor level, read/write, not present.
                page_directory[i] = 0 | 2;
            }
            uint *first_page_table = page_directory + 0x1000;
            i = 0;
            uint address = 0; 
       
            
            //we will fill all 1024 entries, mapping 4 megabytes
            for(i = 0; i < 1024; i++)
            {
                first_page_table[i] = address | 3; // attributes: supervisor level, read/write, present.
                address = address + 4096; //advance the address to the next page boundary
            }
            page_directory[0] = (uint)first_page_table;
            page_directory[0] |= 3;// attributes: supervisor level, read/write, present
            Enable((uint)page_directory);
            stdio.printf("Paging enabled (w00t w00t!)");
            while (true) ;

        }
        [PlugMethod(Assembler = typeof(EndOfKernel))]
        public static uint getEndOfKernel()
        {
            return 0;
        }
        [PlugMethod(Assembler = typeof(plugPageEnable))]
        public static void Enable(uint pageDirectory) { }
        [PlugMethod(Assembler = typeof(plugPageDisable))]
        public static void Disable() { }
        [Plug(Target = typeof(Paging))]
        class plugPageDisable : AssemblerMethod
        {
            public override void AssembleNew(object aAssembler, object aMethodInfo)
            {
                new CPUx86.Mov { DestinationReg = CPUx86.Registers.EAX, SourceReg = CPUx86.Registers.CR0 };
                new CPUx86.And { DestinationReg = CPUx86.Registers.EAX, SourceValue = 0x7FFFFFFF };
                new CPUx86.Mov { DestinationReg = CPUx86.Registers.CR0, SourceReg = CPUx86.Registers.EAX };
                
            }
        }
        [Plug(Target = typeof(Paging))]
        class EndOfKernel : AssemblerMethod
        {
            public override void AssembleNew(object aAssembler, object aMethodInfo)
            {
                new CPUx86.Mov { DestinationReg = CPUx86.Registers.EAX, SourceRef = Cosmos.Assembler.ElementReference.New("_end_code") };
                new CPUx86.Return();
            }
        }
        [Plug(Target = typeof(Paging))]
        class plugPageEnable : AssemblerMethod
        {
            public override void AssembleNew(object aAssembler, object aMethodInfo)
            {
                new CPUx86.Mov { DestinationReg = CPUx86.Registers.EAX, SourceReg = CPUx86.Registers.ESP, SourceIsIndirect = true, SourceDisplacement = 0x8 };
                new CPUx86.Mov { DestinationReg = CPUx86.Registers.CR3, SourceReg = CPUx86.Registers.EAX };
                new CPUx86.Mov { DestinationReg = CPUx86.Registers.EAX, SourceReg = CPUx86.Registers.CR0 };
                new CPUx86.Or { DestinationReg = CPUx86.Registers.EAX, SourceValue = 0x80000000 };
                new CPUx86.Mov { DestinationReg = CPUx86.Registers.CR0, SourceReg = CPUx86.Registers.EAX };
                
            }
        }
    }
}
