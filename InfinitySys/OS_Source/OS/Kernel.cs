using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using Screen = Cosmos.Hardware.VGAScreen;
using System.Threading;
using Cosmos.Hardware.BlockDevice;
using GruntyOS.HAL;
using GruntyOS.IO;
using GruntyOS.Crypto;
using InfinityOS;
using System;

namespace OS
{
    public class Kernel : Sys.Kernel
    {
        protected override void BeforeRun()
        {
            #region ram check
            /* NOTES:
             * Work on new mouse (because it is lagging)
             * Make a link system
             * Make a file system
             * Make a new mouse buffer/backup (what ever you want to call it)
             * Write down the color codes
             * Make a new mouse cursor
             * Make some fonts
             * Make a click checker if (mouse == clicked) do something (see linking system)
            */
            Console.Write("# Checking RAM " + Cosmos.Core.CPU.GetAmountOfRAM() + 2 + "MB");
            if (Cosmos.Core.CPU.GetAmountOfRAM() + 2 > 256)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" OK!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("Its recommended to have at least 256 mb RAM, will continue any way.");
            }
            Console.ForegroundColor = ConsoleColor.White;
            init(); // Initilize filesystem!!!!!
            Console.WriteLine("Press Enter to continue.");
            Console.ReadKey();
            #endregion


        }
        private static void init()
        {

            #region Setup
            // Initilize filesystem
            bool ATAfound = false;
            bool login = false;

            for (int i = 0; i < BlockDevice.Devices.Count; i++)
            {
                BlockDevice xDevice = BlockDevice.Devices[i];
                if (xDevice is Partition)
                {

                    GLNFS fd = new GLNFS((Partition)xDevice);
                    Memory.devices.Add(fd);
                    Console.WriteLine("ATA Device Found! , /dev/sda" + i.ToString());
                    if (GLNFS.isGFS((Partition)xDevice))
                    {
                        while (!login)
                        {
                            GruntyOS.HAL.FileSystem.Root = fd;
                            Console.Write("Username:");
                            string user = Console.ReadLine();
                            if (user == "reinstall") // Just in case you f**cked something up....
                                InfinityOS.Application.Setup.Init();
                            Console.Write("Password:");
                            string pass = Console.ReadLine();
                            GruntyOS.IO.BinaryReader br = new GruntyOS.IO.BinaryReader(new GruntyOS.IO.FileStream("etc/passwd/" + user + ".dat", "r"));
                            int Hash = br.ReadInt32();
                            br = new GruntyOS.IO.BinaryReader(new GruntyOS.IO.FileStream("etc/profile/" + user + ".dat", "r"));
                            if (Hash == GruntyOS.Crypto.Hash.GHash(pass))
                            {
                                Console.WriteLine("Correct password!");
                                login = true;
                            }

                        }
                        login = true;
                    }

                    ATAfound = true;
                }
                else
                {

                }
            }
            if (!ATAfound)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: No compatible memory devices found! , skipping setup.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            if (!login)
                InfinityOS.Application.Setup.Init();
            else // Test the file system...
            {
                foreach (string str in GruntyOS.HAL.FileSystem.Root.ListFiles(""))
                {
                    Console.WriteLine(str);
                }
            }
            Console.ReadLine();
            #endregion
            


        }

        private static void cli()
        {
            Boolean cli = true;
            string dir = "";

            while (cli)
            {
                Console.WriteLine("");
                Console.Write(GruntyOS.CurrentUser.Username + "-" + dir + "> ");
                string input = Console.ReadLine();
                if (input == "exit")
                {
                    cli = false;
                }
                else if (input.Substring(0, 2) == "ls")
                {

                    Console.WriteLine("Files in " + dir);
                    foreach (string fileelement in GruntyOS.HAL.FileSystem.Root.ListFiles(dir))
                    {
                        Console.WriteLine(fileelement);

                    }

                }
                else if (input.Substring(0, 2) == "cd")
                {

                    if (input.Substring(3, 2) == "..")
                    {

                    }
                    else
                    {
                        dir = dir + input.Substring(3, input.Length - 3);
                        Console.Write("Changed directory to " + dir);

                    }
                }



            }
        }


        #region public
        //Public the u-integers
        public uint p1 = 0; //First Taskbar color (bottom)
        public uint p2 = 30; //Second Taskbar color (top)
        public int bc = 200; //Background
        public uint mx; //Mouse X
        public Boolean skipredrawmouse = false;
        public uint my; //Mouse Y
        public uint omx; //Old Mouse X
        public uint omy; //Old Mouse Y
        public uint mc = 0; //Mouse Cursor Color
        public uint ram = Cosmos.Core.CPU.GetAmountOfRAM() + 2; //Ram
        public Cosmos.Hardware.VGAScreen screen;
        public Cosmos.Hardware.Keyboard keyboard;
        public Cosmos.Hardware.TextScreen textscreen;
        public Boolean highgraphics = false;
        public int timehour;
        public int timeminute;

        //Declare programs

        string[] terminal = new string[1024];




        //Declare all the font uint[]

        public uint[] a = new uint[8];
        public uint[] b = new uint[8];
        public uint[] c = new uint[8];
        public uint[] d = new uint[8];
        public uint[] e = new uint[8];
        public uint[] f = new uint[8];
        public uint[] g = new uint[8];
        public uint[] h = new uint[8];
        public uint[] i = new uint[8];
        public uint[] j = new uint[8];
        public uint[] k = new uint[8];
        public uint[] l = new uint[8];
        public uint[] m = new uint[8];
        public uint[] n = new uint[8];
        public uint[] o = new uint[8];
        public uint[] p = new uint[8];
        public uint[] q = new uint[8];
        public uint[] r = new uint[8];
        public uint[] s = new uint[8];
        public uint[] t = new uint[8];
        public uint[] u = new uint[8];
        public uint[] v = new uint[8];
        public uint[] w = new uint[8];
        public uint[] sx = new uint[8];
        public uint[] sy = new uint[8];
        public uint[] z = new uint[8];
        public uint[] n0 = new uint[8]; //remember the "n" before the number!
        public uint[] n1 = new uint[8];
        public uint[] n2 = new uint[8];
        public uint[] n3 = new uint[8];
        public uint[] n4 = new uint[8];
        public uint[] n5 = new uint[8];
        public uint[] n6 = new uint[8];
        public uint[] n7 = new uint[8];
        public uint[] n8 = new uint[8];
        public uint[] n9 = new uint[8];
        public uint[] colon = new uint[8];
        public uint[] semicolon = new uint[8];
        public uint[] slash = new uint[8];
        public uint[] quote = new uint[8]; //what are these called? btw Im not english.
        public uint[] backslash = new uint[8];
        public uint[] arrowright = new uint[8];
        public uint[] arrowleft = new uint[8];
        public uint[] space = new uint[8];
        public uint[] comma = new uint[8];
        public uint[] dot = new uint[8];
        public uint[] excla = new uint[8];
        public uint[] icon = new uint[10];
        public uint wx, wy;
        public string keyboardtext;

        public bool usingkeyboard = false;
        public bool atwindow = false;
        public bool refresh = true;


        //Declare all the pictures/icons

        public string[] terminalicon = new string[21];

        public string Username;
        #endregion
        
        protected override void Run()
        {

            #region programs
            //Maybe we could load the programs into a form for array to contain the title, program name, and code...
            //So we need to write some kind of interpreter maybe for VIPER :D
            //Just delete, modify, remove, add any code
            //Thanks for the help:D


            //- Anders Thuesen


            terminal[0] = "terminal"; //Name
            terminal[1] = "50"; //Height
            terminal[2] = "60"; //Width
            terminal[3] = "0";//Background color

            // 

            #endregion
            #region load font
            //Set all the font variables
            a[0] = 2222222;
            a[1] = 2222222;
            a[2] = 2211222;
            a[3] = 2122122;
            a[4] = 2122122;
            a[5] = 2122122;
            a[6] = 2211212;
            a[7] = 2222222;
            a[8] = 2222222;



            b[0] = 2222222;
            b[1] = 2122222;
            b[2] = 2111222;
            b[3] = 2122122;
            b[4] = 2122122;
            b[5] = 2122122;
            b[6] = 2111222;
            b[7] = 2222222;
            b[8] = 2222222;

            c[0] = 2222222;
            c[1] = 2222222;
            c[2] = 2221122;
            c[3] = 2212222;
            c[4] = 2212222;
            c[5] = 2212222;
            c[6] = 2221122;
            c[7] = 2222222;
            c[8] = 2222222;


            d[0] = 2222222;
            d[1] = 2222122;
            d[2] = 2211122;
            d[3] = 2122122;
            d[4] = 2122122;
            d[5] = 2122122;
            d[6] = 2211122;
            d[7] = 2222222;
            d[8] = 2222222;



            e[0] = 2222222;
            e[1] = 2222222;
            e[2] = 2211222;
            e[3] = 2122122;
            e[4] = 2111122;
            e[5] = 2122222;
            e[6] = 2211222;
            e[7] = 2222222;
            e[8] = 2222222;


            f[0] = 2222222;
            f[1] = 2221222;
            f[2] = 2212222;
            f[3] = 2211222;
            f[4] = 2212222;
            f[5] = 2212222;
            f[6] = 2212222;
            f[7] = 2222222;
            f[8] = 2222222;

            g[0] = 2222222;
            g[1] = 2211222;
            g[2] = 2122122;
            g[3] = 2122122;
            g[4] = 2211122;
            g[5] = 2222122;
            g[6] = 2222122;
            g[7] = 2122122;
            g[8] = 2211222;


            h[0] = 2222222;
            h[1] = 2122222;
            h[2] = 2122222;
            h[3] = 2111222;
            h[4] = 2122122;
            h[5] = 2122122;
            h[6] = 2122122;
            h[7] = 2222222;
            h[8] = 2222222;

            i[0] = 2222222;
            i[1] = 2221222;
            i[2] = 2222222;
            i[3] = 2221222;
            i[4] = 2221222;
            i[5] = 2221222;
            i[6] = 2221222;
            i[7] = 2222222;
            i[8] = 2222222;


            j[0] = 2222222;
            j[1] = 2222122;
            j[2] = 2222222;
            j[3] = 2222122;
            j[4] = 2222122;
            j[5] = 2222122;
            j[6] = 2222122;
            j[7] = 2122122;
            j[8] = 2211222;



            k[0] = 2222222;
            k[1] = 2212222;
            k[2] = 2212222;
            k[3] = 2212122;
            k[4] = 2211222;
            k[5] = 2212122;
            k[6] = 2212122;
            k[7] = 2222222;
            k[8] = 2222222;


            l[0] = 2222222;
            l[1] = 2221222;
            l[2] = 2221222;
            l[3] = 2221222;
            l[4] = 2221222;
            l[5] = 2221222;
            l[6] = 2221222;
            l[7] = 2222222;
            l[8] = 2222222;


            m[0] = 2222222;
            m[1] = 2222222;
            m[2] = 2112122;
            m[3] = 2121212;
            m[4] = 2121212;
            m[5] = 2121212;
            m[6] = 2121212;
            m[7] = 2222222;
            m[8] = 2222222;

            n[0] = 2222222;
            n[1] = 2222222;
            n[2] = 2111222;
            n[3] = 2122122;
            n[4] = 2122122;
            n[5] = 2122122;
            n[6] = 2122122;
            n[7] = 2222222;
            n[8] = 2222222;

            o[0] = 2222222;
            o[1] = 2222222;
            o[2] = 2211222;
            o[3] = 2122122;
            o[4] = 2122122;
            o[5] = 2122122;
            o[6] = 2211222;
            o[7] = 2222222;
            o[8] = 2222222;

            p[0] = 2222222;
            p[1] = 2222222;
            p[2] = 2211222;
            p[3] = 2122122;
            p[4] = 2122122;
            p[5] = 2122122;
            p[6] = 2111222;
            p[7] = 2122222;
            p[8] = 2122222;


            q[0] = 2222222;
            q[1] = 2222222;
            q[2] = 2211122;
            q[3] = 2122122;
            q[4] = 2122122;
            q[5] = 2211122;
            q[6] = 2222122;
            q[7] = 2221112;
            q[8] = 2222122;

            r[0] = 2222222;
            r[1] = 2222222;
            r[2] = 2212122;
            r[3] = 2211222;
            r[4] = 2212222;
            r[5] = 2212222;
            r[6] = 2212222;
            r[7] = 2222222;
            r[8] = 2222222;

            s[0] = 2222222;
            s[1] = 2222222;
            s[2] = 2211122;
            s[3] = 2122222;
            s[4] = 2211222;
            s[5] = 2222122;
            s[6] = 2111222;
            s[7] = 2222222;
            s[8] = 2222222;

            t[0] = 2222222;
            t[1] = 2221222;
            t[2] = 2221222;
            t[3] = 2211122;
            t[4] = 2221222;
            t[5] = 2221222;
            t[6] = 2221222;
            t[7] = 2222222;
            t[8] = 2222222;

            u[0] = 2222222;
            u[1] = 2222222;
            u[2] = 2122122;
            u[3] = 2122122;
            u[4] = 2122122;
            u[5] = 2122122;
            u[6] = 2211222;
            u[7] = 2222222;
            u[8] = 2222222;

            v[0] = 2222222;
            v[1] = 2222222;
            v[2] = 2212122;
            v[3] = 2212122;
            v[4] = 2212122;
            v[5] = 2212122;
            v[6] = 2221222;
            v[7] = 2222222;
            v[8] = 2222222;


            w[0] = 2222222;
            w[1] = 2222222;
            w[2] = 2122212;
            w[3] = 2122212;
            w[4] = 2122212;
            w[5] = 2121212;
            w[6] = 2212122;
            w[7] = 2222222;
            w[8] = 2222222;


            sx[0] = 2222222;
            sx[1] = 2222222;
            sx[2] = 2212122;
            sx[3] = 2212122;
            sx[4] = 2221222;
            sx[5] = 2212122;
            sx[6] = 2212122;
            sx[7] = 2222222;
            sx[8] = 2222222;


            sy[0] = 2222222;
            sy[1] = 2222222;
            sy[2] = 2122122;
            sy[3] = 2122122;
            sy[4] = 2211122;
            sy[5] = 2222122;
            sy[6] = 2111222;
            sy[7] = 2222222;
            sy[8] = 2222222;


            z[0] = 2222222;
            z[1] = 2222222;
            z[2] = 2111122;
            z[3] = 2222122;
            z[4] = 2211222;
            z[5] = 2122222;
            z[6] = 2111122;
            z[7] = 2222222;
            z[8] = 2222222;



            space[0] = 2222222;
            space[1] = 2222222;
            space[2] = 2222222;
            space[3] = 2222222;
            space[4] = 2222222;
            space[5] = 2222222;
            space[6] = 2222222;
            space[7] = 2222222;
            space[8] = 2222222;



            comma[0] = 2222222;
            comma[1] = 2222222;
            comma[2] = 2222222;
            comma[3] = 2222222;
            comma[4] = 2222222;
            comma[5] = 2222222;
            comma[6] = 2221222;
            comma[7] = 2212222;
            comma[8] = 2222222;


            dot[0] = 2222222;
            dot[1] = 2222222;
            dot[2] = 2222222;
            dot[3] = 2222222;
            dot[4] = 2222222;
            dot[5] = 2222222;
            dot[6] = 2221222;
            dot[7] = 2222222;
            dot[8] = 2222222;

            excla[0] = 2222222;
            excla[1] = 2221222;
            excla[2] = 2221222;
            excla[3] = 2221222;
            excla[4] = 2221222;
            excla[5] = 2222222;
            excla[6] = 2221222;
            excla[7] = 2222222;
            excla[8] = 2222222;

            n0[0] = 2222222;
            n0[1] = 2211122;
            n0[2] = 2122212;
            n0[3] = 2112212;
            n0[4] = 2121212;
            n0[5] = 2122112;
            n0[6] = 2211122;
            n0[7] = 2222222;
            n0[8] = 2222222;

            n1[0] = 2222222;
            n1[1] = 2221222;
            n1[2] = 2211222;
            n1[3] = 2221222;
            n1[4] = 2221222;
            n1[5] = 2221222;
            n1[6] = 2211122;
            n1[7] = 2222222;
            n1[8] = 2222222;

            n2[0] = 2222222;
            n2[1] = 2211122;
            n2[2] = 2122212;
            n2[3] = 2222122;
            n2[4] = 2221222;
            n2[5] = 2212222;
            n2[6] = 2111112;
            n2[7] = 2222222;
            n2[8] = 2222222;


            n3[0] = 2222222;
            n3[1] = 2111222;
            n3[2] = 2222122;
            n3[3] = 2111222;
            n3[4] = 2222122;
            n3[5] = 2222122;
            n3[6] = 2111222;
            n3[7] = 2222222;
            n3[8] = 2222222;

            n4[0] = 2222222;
            n4[1] = 2121222;
            n4[2] = 2121222;
            n4[3] = 2121222;
            n4[4] = 2111222;
            n4[5] = 2221222;
            n4[6] = 2221222;
            n4[7] = 2222222;
            n4[8] = 2222222;


            n5[0] = 2222222;
            n5[1] = 2111122;
            n5[2] = 2122222;
            n5[3] = 2122222;
            n5[4] = 2111222;
            n5[5] = 2222122;
            n5[6] = 2111222;
            n5[7] = 2222222;
            n5[8] = 2222222;

            n6[0] = 2222222;
            n6[1] = 2211222;
            n6[2] = 2122122;
            n6[3] = 2122222;
            n6[4] = 2111222;
            n6[5] = 2122122;
            n6[6] = 2211222;
            n6[7] = 2222222;
            n6[8] = 2222222;

            n7[0] = 2222222;
            n7[1] = 2111122;
            n7[2] = 2222122;
            n7[3] = 2221222;
            n7[4] = 2212222;
            n7[5] = 2212222;
            n7[6] = 2212222;
            n7[7] = 2222222;
            n7[8] = 2222222;


            n8[0] = 2222222;
            n8[1] = 2211222;
            n8[2] = 2122122;
            n8[3] = 2211222;
            n8[4] = 2122122;
            n8[5] = 2122122;
            n8[6] = 2211222;
            n8[7] = 2222222;
            n8[8] = 2222222;

            n9[0] = 2222222;
            n9[1] = 2211222;
            n9[2] = 2122122;
            n9[3] = 2122122;
            n9[4] = 2211122;
            n9[5] = 2222122;
            n9[6] = 2222122;
            n9[7] = 2222222;
            n9[8] = 2222222;

            slash[0] = 2222212;
            slash[1] = 2222212;
            slash[2] = 2222122;
            slash[3] = 2222122;
            slash[4] = 2221222;
            slash[5] = 2212222;
            slash[6] = 2212222;
            slash[7] = 2122222;
            slash[8] = 2122222;



            backslash[0] = 2122222;
            backslash[1] = 2122222;
            backslash[2] = 2212222;
            backslash[3] = 2212222;
            backslash[4] = 2221222;
            backslash[5] = 2222122;
            backslash[6] = 2222122;
            backslash[7] = 2222212;
            backslash[8] = 2222212;


            arrowright[0] = 2222222;
            arrowright[1] = 2222222;
            arrowright[2] = 2212222;
            arrowright[3] = 2221222;
            arrowright[4] = 2222122;
            arrowright[5] = 2221222;
            arrowright[6] = 2212222;
            arrowright[7] = 2222222;
            arrowright[8] = 2222222;

            arrowleft[0] = 2222222;
            arrowleft[1] = 2222222;
            arrowleft[2] = 2222122;
            arrowleft[3] = 2221222;
            arrowleft[4] = 2212222;
            arrowleft[5] = 2221222;
            arrowleft[6] = 2222122;
            arrowleft[7] = 2222222;
            arrowleft[8] = 2222222;


            colon[0] = 2222222;
            colon[1] = 2222222;
            colon[2] = 2222222;
            colon[3] = 2221222;
            colon[4] = 2222222;
            colon[5] = 2222222;
            colon[6] = 2221222;
            colon[7] = 2222222;
            colon[8] = 2222222;

            semicolon[0] = 2222222;
            semicolon[1] = 2222222;
            semicolon[2] = 2222222;
            semicolon[3] = 2221222;
            semicolon[4] = 2222222;
            semicolon[5] = 2222222;
            semicolon[6] = 2221222;
            semicolon[7] = 2212222;
            semicolon[8] = 2222222;

            quote[0] = 2212122;
            quote[1] = 2212122;
            quote[2] = 2222222;
            quote[3] = 2222222;
            quote[4] = 2222222;
            quote[5] = 2222222;
            quote[6] = 2222222;
            quote[7] = 2222222;
            quote[8] = 2222222;

            icon[0] = 2222211222;
            icon[1] = 2222112222;
            icon[2] = 2222122222;
            icon[3] = 2211211222;
            icon[4] = 2111111122;
            icon[5] = 1111111112;
            icon[6] = 1111111222;
            icon[7] = 1111111222;
            icon[8] = 1111111112;
            icon[9] = 2111111122;
            icon[10] = 2211211222;

            //This is a picture!!

            terminalicon[0] = "111111111111111111111111111111";
            terminalicon[1] = "100000000000000000000000000001";
            terminalicon[2] = "100000000000000000000000000001";
            terminalicon[3] = "100000000000000000000000000001";
            terminalicon[4] = "100100000000000000000000000001";
            terminalicon[5] = "100010000000000000000000000001";
            terminalicon[6] = "100001000000000000000000000001";
            terminalicon[7] = "100000100000000000000000000001";
            terminalicon[8] = "100001000000000000000000000001";
            terminalicon[9] = "100010000000000000000000000001";
            terminalicon[10] = "100100000111111000000000000001";
            terminalicon[11] = "100000000000000000000000000001";
            terminalicon[12] = "100000000000000000000000000001";
            terminalicon[13] = "100000000000000000000000000001";
            terminalicon[14] = "100000000000000000000000000001";
            terminalicon[15] = "100000000000000000000000000001";
            terminalicon[16] = "100000000000000000000000000001";
            terminalicon[17] = "100000000000000000000000000001";
            terminalicon[18] = "100000000000000000000000000001";
            terminalicon[19] = "100000000000000000000000000001";
            terminalicon[20] = "100000000000000000000000000001";
            terminalicon[21] = "111111111111111111111111111111";
            #endregion
            #region initilize
            //Initilize VGAScreen

            screen = new Cosmos.Hardware.VGAScreen();
            screen.SetMode320x200x8();
            //Initilize TextScreen
            textscreen = new Cosmos.Hardware.TextScreen();

            //Initilize mouse
            Cosmos.Hardware.Mouse mouse;
            mouse = new Cosmos.Hardware.Mouse();
            mouse.Initialize();
            //Initilize MouseBackup
            mouse.X = 0;
            mouse.Y = 0;
            uint[] mousebackup;
            mousebackup = new uint[25];
            mousebackup[0] = (uint)mouse.X;
            mousebackup[1] = (uint)mouse.Y;
            mousebackup[2] = screen.GetPixel320x200x8((uint)mouse.X, (uint)mouse.Y);
            mousebackup[3] = screen.GetPixel320x200x8((uint)mouse.X + 1, (uint)mouse.Y);
            mousebackup[4] = screen.GetPixel320x200x8((uint)mouse.X + 2, (uint)mouse.Y);
            mousebackup[5] = screen.GetPixel320x200x8((uint)mouse.X + 3, (uint)mouse.Y);
            mousebackup[6] = screen.GetPixel320x200x8((uint)mouse.X + 4, (uint)mouse.Y);
            mousebackup[7] = screen.GetPixel320x200x8((uint)mouse.X, (uint)mouse.Y + 1);
            mousebackup[8] = screen.GetPixel320x200x8((uint)mouse.X + 1, (uint)mouse.Y + 1);
            mousebackup[9] = screen.GetPixel320x200x8((uint)mouse.X + 2, (uint)mouse.Y + 1);
            mousebackup[10] = screen.GetPixel320x200x8((uint)mouse.X + 3, (uint)mouse.Y + 1);
            mousebackup[11] = screen.GetPixel320x200x8((uint)mouse.X, (uint)mouse.Y + 2);
            mousebackup[12] = screen.GetPixel320x200x8((uint)mouse.X + 1, (uint)mouse.Y + 2);
            mousebackup[13] = screen.GetPixel320x200x8((uint)mouse.X + 2, (uint)mouse.Y + 2);
            mousebackup[14] = screen.GetPixel320x200x8((uint)mouse.X, (uint)mouse.Y + 3);
            mousebackup[15] = screen.GetPixel320x200x8((uint)mouse.X + 1, (uint)mouse.Y + 3);
            mousebackup[16] = screen.GetPixel320x200x8((uint)mouse.X, (uint)mouse.Y + 4);
            //mousebackup[17] = screen.GetPixel320x200x8((uint)mouse.X + 3, (uint)mouse.Y + 3);
            //mousebackup[18] = screen.GetPixel320x200x8((uint)mouse.X + 4, (uint)mouse.Y + 4);
            //Initilize Keyboard

            keyboard = new Cosmos.Hardware.Keyboard();

            //Initilize Desktop Enviroment
            refreshScreen();

            #endregion
            #region mouse loop

            while (true)
            {

                if (mouse.Y < 196)
                {
                    my = (uint)mouse.Y;
                }
                else if (mouse.Y > 196)
                {
                    my = 196;
                }

                mx = (uint)mouse.X;
                if (mx != omx || my != omy)
                {
                    omx = mx;
                    omy = my;

                    if (skipredrawmouse)
                    {
                        skipredrawmouse = false;

                    }
                    else
                    {

                        screen.SetPixel320x200x8(mousebackup[0], mousebackup[1], mousebackup[2]);
                        screen.SetPixel320x200x8(mousebackup[0] + 1, mousebackup[1], mousebackup[3]);
                        screen.SetPixel320x200x8(mousebackup[0] + 2, mousebackup[1], mousebackup[4]);
                        screen.SetPixel320x200x8(mousebackup[0] + 3, mousebackup[1], mousebackup[5]);
                        screen.SetPixel320x200x8(mousebackup[0] + 4, mousebackup[1], mousebackup[6]);
                        screen.SetPixel320x200x8(mousebackup[0], mousebackup[1] + 1, mousebackup[7]);
                        screen.SetPixel320x200x8(mousebackup[0] + 1, mousebackup[1] + 1, mousebackup[8]);
                        screen.SetPixel320x200x8(mousebackup[0] + 2, mousebackup[1] + 1, mousebackup[9]);
                        screen.SetPixel320x200x8(mousebackup[0] + 3, mousebackup[1] + 1, mousebackup[10]);
                        screen.SetPixel320x200x8(mousebackup[0], mousebackup[1] + 2, mousebackup[11]);
                        screen.SetPixel320x200x8(mousebackup[0] + 1, mousebackup[1] + 2, mousebackup[12]);
                        screen.SetPixel320x200x8(mousebackup[0] + 2, mousebackup[1] + 2, mousebackup[13]);
                        screen.SetPixel320x200x8(mousebackup[0], mousebackup[1] + 3, mousebackup[14]);
                        screen.SetPixel320x200x8(mousebackup[0] + 1, mousebackup[1] + 3, mousebackup[15]);
                        screen.SetPixel320x200x8(mousebackup[0], mousebackup[1] + 4, mousebackup[16]);
                    }

                    //screen.SetPixel320x200x8(mousebackup[0] + 3, mousebackup[1] + 3, mousebackup[17]);
                    //screen.SetPixel320x200x8(mousebackup[0] + 4, mousebackup[1] + 4, mousebackup[18]);

                    checkRefresh();
                    //checkTimeRefresh();

                    mousebackup[0] = mx;
                    mousebackup[1] = my;
                    mousebackup[2] = screen.GetPixel320x200x8(mx, my);
                    mousebackup[3] = screen.GetPixel320x200x8(mx + 1, my);
                    mousebackup[4] = screen.GetPixel320x200x8(mx + 2, my);
                    mousebackup[5] = screen.GetPixel320x200x8(mx + 3, my);
                    mousebackup[6] = screen.GetPixel320x200x8(mx + 4, my);
                    mousebackup[7] = screen.GetPixel320x200x8(mx, my + 1);
                    mousebackup[8] = screen.GetPixel320x200x8(mx + 1, my + 1);
                    mousebackup[9] = screen.GetPixel320x200x8(mx + 2, my + 1);
                    mousebackup[10] = screen.GetPixel320x200x8(mx + 3, my + 1);
                    mousebackup[11] = screen.GetPixel320x200x8(mx, my + 2);
                    mousebackup[12] = screen.GetPixel320x200x8(mx + 1, my + 2);
                    mousebackup[13] = screen.GetPixel320x200x8(mx + 2, my + 2);
                    mousebackup[14] = screen.GetPixel320x200x8(mx, my + 3);
                    mousebackup[15] = screen.GetPixel320x200x8(mx + 1, my + 3);
                    mousebackup[16] = screen.GetPixel320x200x8(mx, my + 4);
                    //mousebackup[17] = screen.GetPixel320x200x8(mx + 3, my + 3);
                    //mousebackup[18] = screen.GetPixel320x200x8(mx + 4, my + 4);


                    screen.SetPixel320x200x8(mx, my, mc);
                    screen.SetPixel320x200x8(mx + 1, my, mc);
                    screen.SetPixel320x200x8(mx + 2, my, mc);
                    screen.SetPixel320x200x8(mx + 3, my, mc);
                    screen.SetPixel320x200x8(mx + 4, my, mc);
                    screen.SetPixel320x200x8(mx, my + 1, mc);
                    screen.SetPixel320x200x8(mx + 1, my + 1, mc);
                    screen.SetPixel320x200x8(mx + 2, my + 1, mc);
                    screen.SetPixel320x200x8(mx + 3, my + 1, mc);
                    screen.SetPixel320x200x8(mx, my + 2, mc);
                    screen.SetPixel320x200x8(mx + 1, my + 2, mc);
                    screen.SetPixel320x200x8(mx + 2, my + 2, mc);
                    screen.SetPixel320x200x8(mx, my + 3, mc);
                    screen.SetPixel320x200x8(mx + 1, my + 3, mc);
                    screen.SetPixel320x200x8(mx, my + 4, mc);
                    //screen.SetPixel320x200x8(mx + 3, my + 3, mc);
                    //screen.SetPixel320x200x8(mx + 4, my + 4, mc);



                }
                else //The mouse is still
                {
                    //checkTimeRefresh();
                }


                if (mouse.Buttons == Cosmos.Hardware.Mouse.MouseState.Right)
                {
                    drawWindow("terminal", mx, my, 75, 180);

                }

                #region keyboard
                if (checkKey(ConsoleKey.A))
                {
                    writeToKeyboard("a");
                }
                else if (checkKey(ConsoleKey.B))
                {
                    writeToKeyboard("b");
                }
                else if (checkKey(ConsoleKey.C))
                {
                    writeToKeyboard("c");
                }
                else if (checkKey(ConsoleKey.D))
                {
                    writeToKeyboard("d");
                }
                else if (checkKey(ConsoleKey.E))
                {
                    writeToKeyboard("e");
                }
                else if (checkKey(ConsoleKey.F))
                {
                    writeToKeyboard("f");
                }
                else if (checkKey(ConsoleKey.G))
                {
                    writeToKeyboard("g");
                }
                else if (checkKey(ConsoleKey.H))
                {
                    writeToKeyboard("h");
                }
                else if (checkKey(ConsoleKey.I))
                {
                    writeToKeyboard("i");
                }
                else if (checkKey(ConsoleKey.J))
                {
                    writeToKeyboard("j");
                }
                else if (checkKey(ConsoleKey.K))
                {
                    writeToKeyboard("k");
                }
                else if (checkKey(ConsoleKey.L))
                {
                    writeToKeyboard("l");
                }
                else if (checkKey(ConsoleKey.M))
                {
                    writeToKeyboard("m");
                }
                else if (checkKey(ConsoleKey.N))
                {
                    writeToKeyboard("n");
                }
                else if (checkKey(ConsoleKey.O))
                {
                    writeToKeyboard("o");
                }
                else if (checkKey(ConsoleKey.P))
                {
                    writeToKeyboard("p");
                }
                else if (checkKey(ConsoleKey.Q))
                {
                    writeToKeyboard("q");
                }
                else if (checkKey(ConsoleKey.R))
                {
                    writeToKeyboard("r");
                }
                else if (checkKey(ConsoleKey.S))
                {
                    writeToKeyboard("s");
                }
                else if (checkKey(ConsoleKey.T))
                {
                    writeToKeyboard("t");
                }
                else if (checkKey(ConsoleKey.U))
                {
                    writeToKeyboard("u");
                }
                else if (checkKey(ConsoleKey.V))
                {
                    writeToKeyboard("v");
                }
                else if (checkKey(ConsoleKey.W))
                {
                    writeToKeyboard("w");
                }
                else if (checkKey(ConsoleKey.X))
                {
                    writeToKeyboard("x");
                }
                else if (checkKey(ConsoleKey.Y))
                {
                    writeToKeyboard("y");
                }
                else if (checkKey(ConsoleKey.Z))
                {
                    writeToKeyboard("z");
                }
                else if (checkKey(ConsoleKey.D0))
                {
                    writeToKeyboard("0");
                }
                else if (checkKey(ConsoleKey.D1))
                {
                    writeToKeyboard("1");
                }
                else if (checkKey(ConsoleKey.D2))
                {
                    writeToKeyboard("2");
                }
                else if (checkKey(ConsoleKey.D3))
                {
                    writeToKeyboard("3");
                }
                else if (checkKey(ConsoleKey.D4))
                {
                    writeToKeyboard("4");
                }
                else if (checkKey(ConsoleKey.D5))
                {
                    writeToKeyboard("5");
                }
                else if (checkKey(ConsoleKey.D6))
                {
                    writeToKeyboard("6");
                }
                else if (checkKey(ConsoleKey.D7))
                {
                    writeToKeyboard("7");
                }
                else if (checkKey(ConsoleKey.D8))
                {
                    writeToKeyboard("8");
                }
                else if (checkKey(ConsoleKey.D9))
                {
                    writeToKeyboard("9");
                }
                else if (checkKey(ConsoleKey.Backspace))
                {
                    keyboardtext = keyboardtext.Substring(0, keyboardtext.Length - 1);
                }
                #endregion

            }
            #endregion
        }
        #region methods
        public void drawRect(uint rectx, uint recty, uint rectc, uint length, uint height)
        {
            for (uint l = 0; l <= length; l++)
            {
                for (uint h = 0; h <= height; h++)
                {
                    screen.SetPixel320x200x8(rectx + l, recty + h, rectc);
                }
            }


        }
        public void drawLine(uint x, uint y, uint length, uint color, bool vertical)
        {
            if (vertical)
            {
                //Is vertical
                for (uint l = 0; l <= length; l++)
                {
                    screen.SetPixel320x200x8(x, y + l, color);
                }
            }
            else
            {
                //Is NOT vertical
                for (uint l = 0; l <= length; l++)
                {
                    screen.SetPixel320x200x8(x + l, y, color);
                }

            }


        }
        public void refreshScreen()
        {
            setColor();
            screen.Clear(bc);
            drawEnviroment(p1, p2);
        }
        public void drawEnviroment(uint color1, uint color2)
        {
            //Draw background
            drawBackgroud();
            drawDesktopIcons();
            //Draw the top bar
            updateTaskbar();
            //Draw Window

        }
        public void drawText(string text, uint x, uint y, uint color)
        {

            for (int count = 0; count <= text.Length; count++)
            {
                drawLetter(text.Substring(count, 1), x, y, color);
                x += 7;

            }
        }
        public Boolean checkKey(ConsoleKey Key)
        {

            ConsoleKey i = ConsoleKey.A;
            keyboard.GetKey(out i);
            if (i == Key)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public void drawLetter(string letter, uint x, uint y, uint color)
        {

            if (letter.Length != 1)
            {

            }
            else
            {

                //Do the magic
                if (letter == " ")
                {
                    drawArray(space, x, y, color);
                }
                else if (letter == "/")
                {
                    drawArray(slash, x, y, color);
                }
                else if (letter == @"""")
                {
                    drawArray(quote, x, y, color);
                }
                else if (letter == @"\")
                {
                    drawArray(backslash, x, y, color);
                }
                else if (letter == ":")
                {
                    drawArray(colon, x, y, color);
                }
                else if (letter == ";")
                {
                    drawArray(semicolon, x, y, color);
                }
                else if (letter == ">")
                {
                    drawArray(arrowright, x, y, color);
                }
                else if (letter == "<")
                {
                    drawArray(arrowleft, x, y, color);
                }
                else if (letter == ",")
                {
                    drawArray(comma, x, y, color);
                }
                else if (letter == ".")
                {
                    drawArray(dot, x, y, color);
                }
                else if (letter == "!")
                {
                    drawArray(excla, x, y, color);
                }
                else if (letter == "a")
                {
                    drawArray(a, x, y, color);
                }
                else if (letter == "b")
                {
                    drawArray(b, x, y, color);
                }
                else if (letter == "c")
                {
                    drawArray(c, x, y, color);
                }
                else if (letter == "d")
                {
                    drawArray(d, x, y, color);
                }
                else if (letter == "e")
                {
                    drawArray(e, x, y, color);
                }
                else if (letter == "f")
                {
                    drawArray(f, x, y, color);
                }
                else if (letter == "g")
                {
                    drawArray(g, x, y, color);
                }
                else if (letter == "h")
                {
                    drawArray(h, x, y, color);
                }
                else if (letter == "i")
                {
                    drawArray(i, x, y, color);
                }
                else if (letter == "j")
                {
                    drawArray(j, x, y, color);
                }
                else if (letter == "k")
                {
                    drawArray(k, x, y, color);
                }
                else if (letter == "l")
                {
                    drawArray(l, x, y, color);
                }
                else if (letter == "m")
                {
                    drawArray(m, x, y, color);
                }
                else if (letter == "n")
                {
                    drawArray(n, x, y, color);
                }
                else if (letter == "o")
                {
                    drawArray(o, x, y, color);
                }
                else if (letter == "p")
                {
                    drawArray(p, x, y, color);
                }
                else if (letter == "q")
                {
                    drawArray(q, x, y, color);
                }
                else if (letter == "r")
                {
                    drawArray(r, x, y, color);
                }
                else if (letter == "s")
                {
                    drawArray(s, x, y, color);
                }
                else if (letter == "t")
                {
                    drawArray(t, x, y, color);
                }
                else if (letter == "u")
                {
                    drawArray(u, x, y, color);
                }
                else if (letter == "v")
                {
                    drawArray(v, x, y, color);
                }
                else if (letter == "w")
                {
                    drawArray(w, x, y, color);
                }
                else if (letter == "x")
                {
                    drawArray(sx, x, y, color);
                }
                else if (letter == "y")
                {
                    drawArray(sy, x, y, color);
                }
                else if (letter == "z")
                {
                    drawArray(z, x, y, color);
                }
                else if (letter == "0")
                {
                    drawArray(n0, x, y, color);
                }
                else if (letter == "1")
                {
                    drawArray(n1, x, y, color);
                }
                else if (letter == "2")
                {
                    drawArray(n2, x, y, color);
                }
                else if (letter == "3")
                {
                    drawArray(n3, x, y, color);
                }
                else if (letter == "4")
                {
                    drawArray(n4, x, y, color);
                }
                else if (letter == "5")
                {
                    drawArray(n5, x, y, color);
                }
                else if (letter == "6")
                {
                    drawArray(n6, x, y, color);
                }
                else if (letter == "7")
                {
                    drawArray(n7, x, y, color);
                }
                else if (letter == "8")
                {
                    drawArray(n8, x, y, color);
                }
                else if (letter == "9")
                {
                    drawArray(n9, x, y, color);
                }

            }
        }
        public void checkRefresh()
        {
            if (refresh)
            {
                refreshScreen();
                refresh = false;
            }
        }
        public void drawArray(uint[] letter, uint x, uint y, uint color)
        {
            for (int i = 0; i <= letter.Length; i++) //This is the Y
            {

                for (int j = 0; j <= letter.GetLength(0); j++) //This is X
                {
                    if (letter[i].ToString().Substring(j, 1) == "1")
                    {
                        screen.SetPixel320x200x8(x + (uint)j, y + (uint)i, color);
                    }
                    else if (letter[i].ToString().Substring(j, 1) == "2")
                    {

                    }
                    else
                    {
                        break;
                    }

                }

            }



        }
        public void drawPicture(string[] letter, uint x, uint y)
        {
            for (int i = 0; i <= letter.Length; i++) //This is the Y
            {

                for (int j = 0; j <= (letter.GetLength(0) + 1); j++) //This is X
                {
                    if (letter[i].Substring(j, 1) == "N")
                    {

                    }
                    else if (letter[i].Substring(j, 1) == "1")
                    {
                        screen.SetPixel320x200x8(x + (uint)j, y + (uint)i, 255);
                    }
                    else if (letter[i].Substring(j, 1) == "0")
                    {
                        screen.SetPixel320x200x8(x + (uint)j, y + (uint)i, 0);
                    }

                }

            }



        }
        public void checkTimeRefresh()
        {
            if (timehour != hour() || timeminute != minute())
            {
                timehour = hour();
                timeminute = minute();
                refreshScreen();

            }
        }
        public int minute()
        {
            return Cosmos.Hardware.RTC.Minute;
        }
        public int hour()
        {
            return Cosmos.Hardware.RTC.Hour;
        }
        public string getTime()
        {
            if ((int)minute().ToString().Length < 2)
            {
                //add a zero to the string
                if ((int)hour().ToString().Length < 2)
                {
                    return "0" + hour().ToString() + ":" + "0" + minute().ToString();
                }
                else
                {
                    return hour().ToString() + ":" + "0" + minute().ToString();
                }

            }
            else
            {
                if ((int)hour().ToString().Length < 2)
                {
                    return "0" + hour().ToString() + ":" + minute().ToString();
                }
                else
                {
                    return hour().ToString() + ":" + minute().ToString();
                }
            }


        }
        public void drawGradiant(uint x, uint y, uint startcolor, uint height, uint length)
        {
            uint color = startcolor;
            if (y + height >= 200 || x + length >= 320)
            {
                drawText("your y or x value is to big with the length and height!", x, y, 0);
            }
            else
            {
                for (uint i = startcolor; color <= i + height; color++) //Fustrating:D I know... just wanted to test your brain.
                {
                    drawLine(x, y, length, color, false);
                    y++;
                }
            }


        }
        public void setColor()
        {
            for (int i = 0; i <= 255; i++)
            {
                //Makes the entries 0-255 the rba color of rgb(0,0,0) to rgb(255, 255, 255) (Black to white)
                screen.SetPaletteEntry(i, (byte)i, (byte)i, (byte)i);
            }
        }
        public void drawCircle(int x0, int y0, int radius, uint color)
        {
            int f = 1 - radius;
            int ddF_x = 1;
            int ddF_y = -2 * radius;
            int x = 0;
            int y = radius;

            screen.SetPixel320x200x8((uint)x0, (uint)(y0 + radius), color);
            screen.SetPixel320x200x8((uint)x0, (uint)(y0 - radius), color);
            screen.SetPixel320x200x8((uint)(x0 + radius), (uint)y0, color);
            screen.SetPixel320x200x8((uint)(x0 - radius), (uint)y0, color);

            while (x < y)
            {
                // ddF_x == 2 * x + 1;
                // ddF_y == -2 * y;
                // f == x*x + y*y - radius*radius + 2*x - y + 1;
                if (f >= 0)
                {
                    y--;
                    ddF_y += 2;
                    f += ddF_y;
                }
                x++;
                ddF_x += 2;
                f += ddF_x;
                screen.SetPixel320x200x8((uint)(x0 + x), (uint)(y0 + y), color);
                screen.SetPixel320x200x8((uint)(x0 - x), (uint)(y0 + y), color);
                screen.SetPixel320x200x8((uint)(x0 + x), (uint)(y0 - y), color);
                screen.SetPixel320x200x8((uint)(x0 - x), (uint)(y0 - y), color);
                screen.SetPixel320x200x8((uint)(x0 + y), (uint)(y0 + x), color);
                screen.SetPixel320x200x8((uint)(x0 - y), (uint)(y0 + x), color);
                screen.SetPixel320x200x8((uint)(x0 + y), (uint)(y0 - x), color);
                screen.SetPixel320x200x8((uint)(x0 - y), (uint)(y0 - x), color);
            }
        }
        public void drawBackgroud()
        {
            if (highgraphics)
            {

                Boolean startblack = true;
                for (uint height = 0; height <= 200; height++)
                {

                    for (uint width = 0; width <= 320; width++)
                    {
                        if (startblack)
                        {
                            screen.SetPixel320x200x8(width, height, 0);
                            startblack = false;

                        }
                        else
                        {
                            screen.SetPixel320x200x8(width, height, 255);
                            startblack = true;
                        }
                    }
                }
            }
            else
            {
                screen.Clear(127);
            }
        }
        public void updateTaskbar()
        {
            drawRect(0, 0, 255, 319, 13);
            drawLine(0, 14, 319, 0, false);
            drawArray(icon, 14, 1, 0);
            drawText("file", 40, 4, 0); //A space of 14
            drawText("edit", 86, 4, 0);
            drawText("view", 132, 4, 0);
            drawText("special", 177, 4, 0);
            //drawText(getTime(), 274, 4, 0); //Time


            //Add corner pixels
            //Right top corner pixel
            screen.SetPixel320x200x8(0, 0, 0);
            screen.SetPixel320x200x8(1, 0, 0);
            screen.SetPixel320x200x8(2, 0, 0);
            screen.SetPixel320x200x8(0, 1, 0);
            screen.SetPixel320x200x8(0, 2, 0);
            screen.SetPixel320x200x8(1, 1, 0);
            //Left top corner pixel
            screen.SetPixel320x200x8(319, 0, 0);
            screen.SetPixel320x200x8(318, 0, 0);
            screen.SetPixel320x200x8(317, 0, 0);
            screen.SetPixel320x200x8(319, 1, 0);
            screen.SetPixel320x200x8(319, 2, 0);
            screen.SetPixel320x200x8(318, 1, 0);
        }
        public void drawDesktopIcons()
        {

            drawPicture(terminalicon, 265, 30);
            drawRect(240, 51, 255, 70, 10);
            drawText("terminal", 248, 53, 0);

        }
        public void checkClickLink(uint clickx, uint clicky)
        {
        }
        public void drawWindow(string name, uint x, uint y, uint height, uint width)
        {

            usingkeyboard = true;
            atwindow = true;
            wx = x;
            wy = y;

            if (highgraphics)
            {
                if (y + height + 3 > 200)
                {
                    y = 200 - height - 11;
                }
                if (y < 11)
                {
                    y = 14;
                }
                refreshScreen();

                drawRect(x, y, 255, width, 10);
                drawRect(x, y + 10, 255, width, height);
                drawLine(x + width, y, height + 10, 0, true);
                drawLine(x, y + 10, width, 0, false);
                drawLine(x, y + 10 + height, width, 0, false);
                drawLine(x, y, 10 + height, 0, true);
                drawLine(x, y, width, 0, false);
                drawText(name, x + 23, y + 1, 0);
                drawRect(x + 8, y + 2, 0, 6, 6);
                drawRect(x + 9, y + 3, 255, 4, 4);

                //Draw the window bar effects

                drawLine(x + 3, y + 3, 3, 0, false);
                drawLine(x + 3, y + 5, 3, 0, false);
                drawLine(x + 3, y + 7, 3, 0, false);


                drawLine(x + 16, y + 3, 5, 0, false);
                drawLine(x + 16, y + 5, 5, 0, false);
                drawLine(x + 16, y + 7, 5, 0, false);

                drawLine((uint)(x + 23 + (name.Length * 8)), y + 3, (uint)(width - 25 - (name.Length * 8)), 0, false);
                drawLine((uint)(x + 23 + (name.Length * 8)), y + 5, (uint)(width - 25 - (name.Length * 8)), 0, false);
                drawLine((uint)(x + 23 + (name.Length * 8)), y + 7, (uint)(width - 25 - (name.Length * 8)), 0, false);

                drawText(keyboardtext, x + 20, y + 30, 0);
                skipredrawmouse = true;

            }
            else
            {
                screen.Clear(127);

                if (y + height + 3 > 200)
                {
                    y = 200 - height - 11;
                }
                if (y < 11)
                {
                    y = 14;
                }
                refreshScreen();

                drawRect(x, y, 255, width, 10);
                drawRect(x, y + 10, 255, width, height);
                drawLine(x + width, y, height + 10, 0, true);
                drawLine(x, y + 10, width, 0, false);
                drawLine(x, y + 10 + height, width, 0, false);
                drawLine(x, y, 10 + height, 0, true);
                drawLine(x, y, width, 0, false);
                drawText(name, x + 23, y + 1, 0);
                drawRect(x + 8, y + 2, 0, 6, 6);
                drawRect(x + 9, y + 3, 255, 4, 4);

                //Draw the window bar effects

                drawLine(x + 3, y + 3, 3, 0, false);
                drawLine(x + 3, y + 5, 3, 0, false);
                drawLine(x + 3, y + 7, 3, 0, false);


                drawLine(x + 16, y + 3, 5, 0, false);
                drawLine(x + 16, y + 5, 5, 0, false);
                drawLine(x + 16, y + 7, 5, 0, false);

                drawLine((uint)(x + 23 + (name.Length * 8)), y + 3, (uint)(width - 25 - (name.Length * 8)), 0, false);
                drawLine((uint)(x + 23 + (name.Length * 8)), y + 5, (uint)(width - 25 - (name.Length * 8)), 0, false);
                drawLine((uint)(x + 23 + (name.Length * 8)), y + 7, (uint)(width - 25 - (name.Length * 8)), 0, false);


                drawText(keyboardtext, x + 20, y + 30, 0);


                skipredrawmouse = true;
            }

        }
        public void writeToKeyboard(string text)
        {
            keyboardtext = keyboardtext + "" + text; 
        }
        #endregion
        }
    }

