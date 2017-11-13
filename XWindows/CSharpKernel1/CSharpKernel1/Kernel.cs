using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;

namespace CSharpKernel1
{
    
    public class Kernel : Sys.Kernel
    {
        Cosmos.Hardware.VGAScreen scr = new Cosmos.Hardware.VGAScreen();
     
        protected override void BeforeRun()
        {
            Console.WriteLine("THE OS booted successfully.");
            Console.WriteLine("Copyright THE OS. ");
            Console.WriteLine("===========================================================================");

            scr.SetMode320x200x8(); //Set the mode to 320x200

            scr.SetPaletteEntry(0, 0, 63, 0);

            scr.Clear(0); //Clear the screen

            window MainWnd = new window(0, 0, 320, 200, 4, null, true);

            MainWnd.Draw();

            //onscreenkb.DrawKeyboard(MainWnd);

            //keyboard.Initialize(MainWnd);

            mouse.Initialize(scr, MainWnd);
    }

        protected override void Run(){}
    }
}
