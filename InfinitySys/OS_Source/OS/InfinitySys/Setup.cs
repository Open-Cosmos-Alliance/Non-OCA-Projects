using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Hardware;
using Cosmos.Hardware.BlockDevice;
using GruntyOS.HAL;
using GruntyOS.IO;

namespace InfinityOS.Application
{
    class Setup
    {

        public static void Init()
        {
            Console.CursorLeft = 0;
            Console.BackgroundColor = ConsoleColor.DarkBlue;

            Console.Clear();
            fill(ConsoleColor.DarkBlue, 0, 85, 0, 26);
            Console.CursorTop = 0;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;

            Console.WriteLine("System 1 Setup Wizard - Step 1: Pick a partition.                               ");
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            List<string> parts = new List<string>();
            List<Partition> partitions = new List<Partition>();
            int d = 1;
            for (int i = 0; i < BlockDevice.Devices.Count; i++)
            {
                BlockDevice xDevice = BlockDevice.Devices[i];
                if (xDevice is Partition)
                {
                    parts.Add("/dev/sda" + d.ToString());
                  
                    partitions.Add((Partition)xDevice);
                    d++;
                }
            }
            fill(ConsoleColor.Gray, 15, 40, 4, 13);
            Console.CursorLeft = 15;
            Console.CursorTop = 4;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Please choose a partition");
            uint selected = Menu(parts.ToArray(), 15, 5);

            fill(ConsoleColor.Gray, 15, 40, 4, 13);
            Console.CursorTop = 0;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("GruntyOS Setup Wizard - Step 2: Create an account                               ");

            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.CursorLeft = 15;
            Console.CursorTop = 4;
            Console.WriteLine("Please create an account");
            Console.CursorLeft = 15;
            Console.CursorTop = 6;
            Console.WriteLine("Password:");
            Console.CursorTop = 5;
            Console.CursorLeft = 15;
            Console.Write("Username:");
            string User = Console.ReadLine();
            Console.CursorLeft = 15;
            Console.Write("Password:");
            string Pass = Console.ReadLine();
            GLNFS fd = new GLNFS(partitions.ToArray()[selected]);
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Clear();
            Console.CursorTop = 0;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("GruntyOS Setup Wizard - Step 3: Installing Grunty OS                            ");
            GruntyOS.HAL.FileSystem.Root = fd;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Formating selected partition to GLNFS..");
            GruntyOS.CurrentUser.Username = "System";
            fd.Format("GruntyOS");
            Console.WriteLine("Creating Directory /bin/");
            fd.makeDir("bin");
            Console.WriteLine("Creating Directory /etc/");
            Console.WriteLine("Creating Directory /usr/");
            fd.makeDir("usr");
            Console.WriteLine("Creating Directory /usr/include/");
            fd.makeDir("/usr/include");
            Console.WriteLine("Creating Directory /usr/doc/");
            fd.makeDir("/usr/doc");
            File.Save("usr/doc/readme.txt", @"Welcome to Grunty OS! A Cosmos based OS. The primary UI for Grunty OS is command line shell called 'GSHELL' , but
GruntyOS does offer a GUI. You can access this GUI by typing 'GUI'. The command line shell has similar syntax to
BASH but also implents many commands from Windows. For a list of commands look at the file commands.txt. For more
detailed information about specific parts of the operating system setup has created several usefull documents in the
/documents/ folder. ");
            File.Save("usr/doc/viper.txt", @"Viper is a runtime enviroment , similar to Java VM that Grunty OS uses.At the core of Viper is the Viper virtual machine , the program that executes Viper bytecode. Viper programs are compiled into bytecode files with the extention .GEX. Grunty OS is equiped with a viper assembler , which can create Viper programs. You can use this with the vasm command.");
            File.Save("usr/doc/file_structure.txt", @"The file structure of Grunty OS is very similar to unix / linux. The root consists of the following directories.

 
/bin/   -   Binaries , command line programs are stored here
/etc/   -   Configuration files and user profile information are stored here.
/home/  -   This is were users folders are located. 
");
            File.Save("usr/doc/GLNFS.txt", @"GLNFS stands for Grunty OS Linked node file system and is the file system for Grunty OS. The File system consists of nodes (directories) containing a list of information. This information contains pointers to other nodes, and file information. ");

            Console.WriteLine("Creating File /usr/include/viper.h");
            fd.makeDir("etc");
            Console.WriteLine("Creating Directory /etc/profile/");
            fd.makeDir("etc/profile");
            BinaryWriter bw = new BinaryWriter(new FileStream("etc/profile/" + User + ".login", "w"));

            bw.Write(GruntyOS.Clock.GetDateTimeString());
            bw.BaseStream.Close();
            Console.WriteLine("Creating Directory /etc/passwd/");
            fd.makeDir("etc/passwd");
            Console.WriteLine("Creating File: /etc/passwd/" + User + ".dat");
            bw = new BinaryWriter(new FileStream("etc/passwd/" + User + ".dat", "w"));
            bw.Write(GruntyOS.Crypto.Hash.GHash(Pass));
            bw.BaseStream.Close();
            Console.WriteLine("Creating File: /etc/profile/" + User + ".dat");
            bw = new BinaryWriter(new FileStream("etc/profile/" + User + ".dat", "w"));
            bw.Write(1);
            bw.BaseStream.Close();
            Console.WriteLine("Creating Directory /home/");
            fd.makeDir("home");
            fd.saveFile(new byte[] { 0x47, 0x45, 0x58, 0x33, 0x20, 0x53, 0x43, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x4D, 0x79, 0x20, 0x50, 0x72, 0x6F, 0x67, 0x72, 0x61, 0x6D, 0x06, 0x4E, 0x6F, 0x6E, 0x61, 0x6D, 0x65, 0x01, 0x64, 0x10, 0x70, 0x72, 0x6F, 0x67, 0x72, 0x61, 0x6D, 0x2E, 0x62, 0x79, 0x74, 0x65, 0x63, 0x6F, 0x64, 0x65, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x17, 0x43, 0x72, 0x65, 0x61, 0x74, 0x65, 0x64, 0x20, 0x41, 0x74, 0x3A, 0x20, 0x30, 0x30, 0x3A, 0x35, 0x33, 0x3A, 0x35, 0x31, 0x20, 0x41, 0x4D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x50, 0x72, 0x6F, 0x67, 0x72, 0x61, 0x6D, 0x2E, 0x62, 0x79, 0x74, 0x65, 0x63, 0x6F, 0x64, 0x65, 0x2B, 0x01, 0x00, 0x00, 0x76, 0x43, 0x6C, 0x61, 0x73, 0x73, 0x07, 0x2E, 0x66, 0x69, 0x65, 0x6C, 0x64, 0x73, 0x00, 0x05, 0x2E, 0x74, 0x65, 0x78, 0x74, 0xCC, 0x00, 0x00, 0x00, 0x09, 0x2D, 0x47, 0x72, 0x75, 0x6E, 0x74, 0x79, 0x20, 0x4F, 0x53, 0x20, 0x4C, 0x69, 0x6E, 0x65, 0x20, 0x45, 0x64, 0x69, 0x74, 0x6F, 0x72, 0x20, 0x43, 0x6F, 0x6D, 0x6D, 0x61, 0x6E, 0x64, 0x73, 0x3A, 0x20, 0x2D, 0x2D, 0x53, 0x41, 0x56, 0x45, 0x20, 0x2D, 0x2D, 0x45, 0x58, 0x49, 0x54, 0x34, 0x00, 0x00, 0x00, 0x00, 0x35, 0x00, 0x00, 0x00, 0x00, 0x03, 0x07, 0x50, 0x72, 0x6F, 0x67, 0x72, 0x61, 0x6D, 0xBD, 0x00, 0x00, 0x00, 0x09, 0x01, 0x6A, 0x34, 0x01, 0x00, 0x00, 0x00, 0x09, 0x00, 0x34, 0x02, 0x00, 0x00, 0x00, 0x09, 0x02, 0x2D, 0x20, 0x34, 0x03, 0x00, 0x00, 0x00, 0x35, 0x03, 0x00, 0x00, 0x00, 0x03, 0x07, 0x50, 0x72, 0x6F, 0x67, 0x72, 0x61, 0x6D, 0xB4, 0x00, 0x00, 0x00, 0x0D, 0x34, 0x01, 0x00, 0x00, 0x00, 0x35, 0x01, 0x00, 0x00, 0x00, 0x34, 0x05, 0x00, 0x00, 0x00, 0x09, 0x06, 0x2D, 0x2D, 0x45, 0x58, 0x49, 0x54, 0x35, 0x05, 0x00, 0x00, 0x00, 0x41, 0x1D, 0x01, 0x00, 0x00, 0x00, 0x08, 0x9D, 0x00, 0x00, 0x00, 0x06, 0xB2, 0x00, 0x00, 0x00, 0x35, 0x02, 0x00, 0x00, 0x00, 0x35, 0x01, 0x00, 0x00, 0x00, 0x15, 0x34, 0x02, 0x00, 0x00, 0x00, 0x06, 0x55, 0x00, 0x00, 0x00, 0x13, 0x13, 0x34, 0x06, 0x00, 0x00, 0x00, 0x31, 0x12, 0x13, 0x13, 0x34, 0x07, 0x00, 0x00, 0x00, 0x31, 0x12, 0x65, 0x00, 0x00, 0x00, 0x13, 0x13, 0x13, 0x13, 0x05, 0x2E, 0x6D, 0x65, 0x74, 0x61, 0x3C, 0x00, 0x00, 0x00, 0x3C, 0x70, 0x72, 0x6F, 0x67, 0x72, 0x61, 0x6D, 0x3E, 0x0A, 0x3C, 0x6C, 0x61, 0x62, 0x65, 0x6C, 0x20, 0x6E, 0x61, 0x6D, 0x65, 0x20, 0x3D, 0x20, 0x22, 0x2E, 0x63, 0x74, 0x6F, 0x72, 0x22, 0x20, 0x61, 0x64, 0x64, 0x72, 0x65, 0x73, 0x73, 0x20, 0x3D, 0x20, 0x22, 0x32, 0x30, 0x32, 0x22, 0x2F, 0x3E, 0x0A, 0x3C, 0x2F, 0x70, 0x72, 0x6F, 0x67, 0x72, 0x61, 0x6D, 0x3E }, "bin/edit");
            File.Chmod("bin/edit", "555");
            Console.WriteLine("Creating Directory /home/" + User + "/");
            fd.makeDir("home/" + User);
            GruntyOS.CurrentUser.Username = User;
            Console.WriteLine("Creating Directory /home/" + User + "/settings/");
            fd.makeDir("home/" + User + "/settings");
            Console.WriteLine("Creating Directory /home/" + User + "/documents/");

            fd.makeDir("home/" + User + "/documents");
            File.Save("home/" + User + "/readme.txt", @"Welcome to Grunty OS! A Cosmos based OS. The primary UI for Grunty OS is command line shell called 'GSHELL' , but
GruntyOS does offer a GUI. You can access this GUI by typing 'GUI'. The command line shell has similar syntax to
BASH but also implents many commands from Windows. For a list of commands look at the file commands.txt. For more
detailed information about specific parts of the operating system setup has created several usefull documents in the
/documents/ folder. ");
            File.Save("home/" + User + "/commands.txt", @"

GruntyOS Commands
-----------------------------------------------
echo           -     Prints a string
cd             -     Changes the current directory
dir            -     Briefly list directory contents
./             -     Executes a program in the current directory
pwd            -     Prints working directory
ls             -     List information about about the directory
mkdir          -     Creates a new directory
passwd         -     Modify a password
su             -     Subsitute user
sudo           -     Execute command with root privilages
mount          -     Mount a Filesystem
disk           -     Disk utility
userdel        -     Delete a user account
logout         -     Log out
useradd        -     Create a new user
mv             -     Move a file
rm             -     Delete a file or directory
unlink         -     See rm
usermod        -     Modify account
chmod          -     Change file permissions

");

            Console.WriteLine("Creating Directory /home/" + User + "/desktop/");


            Console.WriteLine("Creating File /home/" + User + "/readme.txt");
            Console.ReadLine();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            Cosmos.Core.Global.CPU.Reboot();
            Console.WriteLine("Setup complete");
            Console.WriteLine("Please reboot...");
            while (true)
            {
            }

        }

        public static void fill(ConsoleColor c, int x, int width, int y, int height)
        {
            ConsoleColor prevColor = Console.BackgroundColor;
            Console.BackgroundColor = c;
            int left = Console.CursorLeft, top = Console.CursorTop;
            for (int X = 0; X < width; X++)
            {
                for (int Y = 0; Y < height; Y++)
                {
                    Console.CursorLeft = X + x;
                    Console.CursorTop = Y + y;
                    Console.Write(" ");
                }
            }
            Console.CursorLeft = left;
            Console.CursorTop = top;
            Console.BackgroundColor = prevColor;
        }
        public static uint Menu(string[] items, int x, int y)
        {
            int curseleft = x;
            uint selc = 0;
            int cursetop = y;

        redraw:
            Console.CursorTop = y;
            for (int i = 0; i < items.Length; i++)
            {
                Console.CursorLeft = curseleft;
                if (i == selc)
                {
                    ConsoleColor c1 = Console.ForegroundColor;
                    ConsoleColor c2 = Console.BackgroundColor;
                    Console.BackgroundColor = c1;
                    Console.ForegroundColor = c2;
                    Console.WriteLine(items[i]);
                    Console.ForegroundColor = c1;
                    Console.BackgroundColor = c2;
                }
                else
                {

                    Console.CursorLeft = curseleft;
                    Console.WriteLine(items[i]);
                }
            }
            byte c = (byte)Console.Read();
            if (c == 145)
            {
                selc--;
                goto redraw;
            }
            else if (c == (byte)147)
            {
                selc++;
                goto redraw;
            }
            else if (c == (byte)10)
            {

                return selc;
            }
            else
            {
                goto redraw;
            }
            Console.WriteLine(c.ToString());

        }
    }
}
