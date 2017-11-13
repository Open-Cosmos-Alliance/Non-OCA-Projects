using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos.Core.Processes
{
    public class  Process
    {
        public List<Threads.Thread> threads;
        public List<MemoryMan.AllocatedMemory> AllocatedMemories;
    }
}
