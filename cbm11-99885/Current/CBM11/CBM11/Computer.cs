using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emu6502
{
    class Computer
    {
        private ConsoleIO io = new ConsoleIO();
        private CPU cpu = null;
        private Memory memory = null;

        public Computer()
        {
            
        }

        public void Start()
        {
            // Memory can map to IO, so we have to send in a display object
            memory = new Memory(ref io);

            /*memory.write(5, 169);
            memory.write(6, 12);
            memory.write(7, 141);
            memory.write(8, 232);
            memory.write(9, 3);*/

            // The cpu can access memory
            cpu = new CPU(ref memory);

            cpu.start(0);
        }
    }
}
