using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Emu6502;

namespace CBM11Tests
{
    [TestClass]
    public class LDATest
    {
        [TestMethod]
        public void LDA_Immediate()
        {
            ConsoleIO io = new ConsoleIO();
            Memory mem = new Memory(ref io);
            CPU cpu = new CPU(ref mem);

            mem.write(0x0000, 0xA9); mem.write(0x0001, 0x01);  // LDA #$01

            // HALT - MUST do this or else CPU will never return.
            mem.write(0x0002, 0x02);

            cpu.start();

            Assert.AreEqual(cpu.registers.a, 1, "Accumulator value is incorrect.");
            Assert.AreEqual(cpu.registers.pc, 3, "Program counter is incorrect.");
        }
    }
}
