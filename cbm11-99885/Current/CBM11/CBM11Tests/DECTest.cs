using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Emu6502;

namespace CBM11Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class DECTest
    {
        public DECTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void DEC_ZeroPage()
        {
            ConsoleIO io = new ConsoleIO();
            Memory mem = new Memory(ref io);
            CPU cpu = new CPU(ref mem);

            //DEC $0005 twice
            mem.write(0x0000, 0xC6); mem.write(0x0001, 0x05);  // DEC $05
            mem.write(0x0002, 0xC6); mem.write(0x0003, 0x05);  // DEC $05
            
            // HALT - MUST do this or else CPU will never return.
            mem.write(0x0004, 0x02);  

            cpu.start();

            Assert.AreEqual(254, mem.read(0x0005), "Zero page value is incorrect.");
            Assert.AreEqual(cpu.registers.pc, 5, "Program counter is incorrect.");


        }
    }
}
