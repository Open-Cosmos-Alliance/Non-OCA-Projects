using System;
using Cosmos.Compiler.Builder;
using System.Collections.Generic;

namespace vfslab
{
    class Program
    {
        #region Cosmos Builder logic
        // Most users wont touch this. This will call the Cosmos Build tool
        [STAThread]
        static void Main(string[] args)
        {
            BuildUI.Run();
        }
        #endregion

        // Main entry point of the kernel
        public static void Init()
        {
            var xBoot = new Cosmos.Sys.Boot();
            xBoot.Execute();

            List<file> fs = new List<file>();

            Console.WriteLine("CosLabs - Virtual File System");
            while (true)
            {
                Console.Write("> ");

                string command = Console.ReadLine();
                if (command == "new file")
                {
                    Console.Write("Name your file: ");
                    string fileName = Console.ReadLine();
                    Console.Write("Put some crap in your file: ");
                    string crap = Console.ReadLine();

                    file newFile = new file();
                    newFile.Content = crap;
                    newFile.Name = fileName;

                    fs.Add(newFile);

                    newFile = null;
                }
                else if (command == "help")
                {
                    Console.WriteLine("new file:  Create new file");
                    Console.WriteLine("list:  List the files");
                    Console.WriteLine("cls:  Clear the screen");
                    Console.WriteLine("help:  Show this");
                    Console.WriteLine("open [filename]:  open a file");
                }
                else if (command == "list")
                {
                    Console.WriteLine("There are: " + fs.Count + " files");

                    for (int i = 0; i < fs.Count; i++)
                    {
                        Console.WriteLine("\t" + fs[i].FileNameAndSize());
                    }
                }
                else if (command.StartsWith("open"))
                {
                    string filename = command.Substring(5);
                    bool fileFound = false;

                    for (int i = 0; i < fs.Count; i++)
                    {
                        if (fs[i].Name == filename)
                        {
                            Console.WriteLine(fs[i].Content);
                            fileFound = true;
                            break;
                        }
                    }

                    if (fileFound == false)
                    {
                        Console.WriteLine("No such file");
                    }
                }
                else if (command == "cls")
                {
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine("No such command");
                    command = null;
                }
            }
        }
    }
}