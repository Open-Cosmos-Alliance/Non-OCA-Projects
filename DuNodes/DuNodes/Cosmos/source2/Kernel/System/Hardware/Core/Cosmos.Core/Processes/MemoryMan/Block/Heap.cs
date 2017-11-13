using System;

namespace Cosmos.Core.Processes.MemoryMan.Block {
    // This class must be static, as for creating objects, we needd the hea
    public static class Heap {
        public static bool EnableDebug = false;
        public static uint mStart; //Real start of memory
        public static uint mStartAddress; //Actual position of cursor
        public static uint mLength; //Longueur actuelle de la ram 
        public static uint mEndOfRam; //Adresse de fin de la ram

        private static void DoInitialize(uint aStartAddress, uint aEndOfRam)
        {
            mStart = mStartAddress = aStartAddress + (4 - (aStartAddress % 4));
            mLength = aEndOfRam - aStartAddress;
            mLength = (mLength / 4) * 4;
            mStartAddress += 1024;
            mEndOfRam = aEndOfRam;
            mStartAddress = (mStartAddress / 4) * 4;
            mLength -= 1024;
            //UpdateDebugDisplay();
        }

        private static bool mInitialized = false;
        internal static void Initialize()
        {
            if (!mInitialized)
            {
                mInitialized = true;
                DoInitialize(CPU.GetEndOfKernel(), (CPU.GetAmountOfRAM() - 1) * 1024 * 1024);
               
            }
        }
        //TODO:REAL MEMORY MANAGEMENT
        private static void ClearMemory(uint aStartAddress, uint aLength)
        {
            //TODO: Move to memory. Internal access only...
            Global.CPU.ZeroFill(aStartAddress, aLength);
        }

        public static uint GetAmountOfFreeRAM()
        {
            //mStart
           //TODO: ONLY DEBUG MODE
//                    Console.Write("[Heap Usage: " +mStartAddress);        
//                    Console.Write("/" + mEndOfRam);
//                    Console.WriteLine("] bytes");
//                    Console.Write("[?? Usage: " + mStart);
//                    Console.Write("/" + mLength);
//                    Console.WriteLine("] bytes");
            return (mEndOfRam - mStartAddress)/1024/1024;

        }

        public static uint GetAmountOfUsedRAM()
        {
            if (mInitialized)
                Initialize();
            //mStart
            //TODO: ONLY DEBUG MODE
            //                    Console.Write("[Heap Usage: " +mStartAddress);        
            //                    Console.Write("/" + mEndOfRam);
            //                    Console.WriteLine("] bytes");
            //                    Console.Write("[?? Usage: " + mStart);
            //                    Console.Write("/" + mLength);
            //                    Console.WriteLine("] bytes");
            return (mStartAddress) / 1024 / 1024;

        }


        private static void WriteNumber(uint aNumber, byte aBits)
        {
            uint xValue = aNumber;
            byte xCurrentBits = aBits;
            Console.Write("0x");
            while (xCurrentBits >= 4) {
                xCurrentBits -= 4;
                byte xCurrentDigit = (byte)((xValue >> xCurrentBits) & 0xF);
                string xDigitString = null;
                switch (xCurrentDigit) {
                    case 0:
                        xDigitString = "0";
                        goto default;
                    case 1:
                        xDigitString = "1";
                        goto default;
                    case 2:
                        xDigitString = "2";
                        goto default;
                    case 3:
                        xDigitString = "3";
                        goto default;
                    case 4:
                        xDigitString = "4";
                        goto default;
                    case 5:
                        xDigitString = "5";
                        goto default;
                    case 6:
                        xDigitString = "6";
                        goto default;
                    case 7:
                        xDigitString = "7";
                        goto default;
                    case 8:
                        xDigitString = "8";
                        goto default;
                    case 9:
                        xDigitString = "9";
                        goto default;
                    case 10:
                        xDigitString = "A";
                        goto default;
                    case 11:
                        xDigitString = "B";
                        goto default;
                    case 12:
                        xDigitString = "C";
                        goto default;
                    case 13:
                        xDigitString = "D";
                        goto default;
                    case 14:
                        xDigitString = "E";
                        goto default;
                    case 15:
                        xDigitString = "F";
                        goto default;
                    default:
                        if (xDigitString == null) {
                            System.Diagnostics.Debugger.Break();
                        }
                        Console.Write(xDigitString);
                        break;
                }
            }
        }

        private static bool mDebugDisplayInitialized = false;

        // this method displays the used/total memory of the heap on the first line of the text screen
        private static void UpdateDebugDisplay()
        {
            if (EnableDebug)
            {
                if (!mDebugDisplayInitialized)
                {
                    mDebugDisplayInitialized = true;
                    int xOldPositionLeft = Console.CursorLeft;
                    int xOldPositionTop = Console.CursorTop;
                    Console.CursorLeft = 0;
                    Console.CursorTop = 0;
                    Console.Write("[Heap Usage: ");
                   
                    Console.Write("/");
                    
                    Console.Write("] bytes");
                    while (Console.CursorLeft < (Console.WindowWidth-1))
                    {
                        Console.Write(" ");
                    }
                    Console.CursorLeft = xOldPositionLeft;
                    Console.CursorTop = xOldPositionTop;
                }
                else
                {
                    int xOldPositionLeft = Console.CursorLeft;
                    int xOldPositionTop = Console.CursorTop;
                    Console.CursorLeft = 13;
                    Console.CursorTop = 0;
                    WriteNumber(mStartAddress,
                                32);
                    Console.CursorLeft = xOldPositionLeft;
                    Console.CursorTop = xOldPositionTop;
                }
            }
        }

        public static uint MemAlloc(uint aLength) {
            Initialize();
            uint xTemp = mStartAddress;

            if ((xTemp + aLength) > (mStart + mLength))
            {
//                Console.WriteLine("Too large memory block allocated!");
//                WriteNumber(aLength, 32);
//                while (true)
//                    ;
                Console.WriteLine("No More RAM available. Freeing some....");
                uint cleared = MemoryManager.ClearRam();
                Console.WriteLine("Cleared " + cleared + " ram");
            }
            mStartAddress += aLength;
           // UpdateDebugDisplay();
            ClearMemory(xTemp, aLength);
            return xTemp;
        }
    }
}
