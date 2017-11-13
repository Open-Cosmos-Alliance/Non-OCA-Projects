using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpKernel1
{
    class keyboard
    {
        static uint clk = 0;
        static uint cr = 0;
        static uint color = 0;
        public static void Initialize(window ParentWnd)
        {
            Cosmos.Hardware.VGAScreen scr = new Cosmos.Hardware.VGAScreen();
            Cosmos.Hardware.Keyboard kb = new Cosmos.Hardware.Keyboard();

            window end = new window(ParentWnd.Width / 2, ParentWnd.Width / 2, 50, 30, 5, ParentWnd,false);

            while (true)
            {
                ConsoleKey key = kb.ReadKey();

                if (key == ConsoleKey.A)
                {

                    clk += 5;
                    end.DrawFont(1,30 + clk,30 + cr, color);
                }
                if (key == ConsoleKey.B)
                {

                    clk += 5;
                    end.DrawFont(2, 30 + clk, 30 + cr, color);
                }
                if (key == ConsoleKey.C)
                {

                    clk += 5;
                    end.DrawFont(3, 30 + clk, 30 + cr, color);
                } 
                if (key == ConsoleKey.D)
                {

                    clk += 5;
                    end.DrawFont(4, 30 + clk, 30 + cr, color);
                }
                if (key == ConsoleKey.E)
                {

                    clk += 5;
                    end.DrawFont(5, 30 + clk, 30 + cr, color);
                }
                if (key == ConsoleKey.F)
                {

                    clk += 5;
                    end.DrawFont(6, 30 + clk, 30 + cr, color);
                }
                if (key == ConsoleKey.G)
                {

                    clk += 5;
                    end.DrawFont(7, 30 + clk, 30 + cr, color);
                }
                if (key == ConsoleKey.H)
                {

                    clk += 5;
                    end.DrawFont(8, 30 + clk, 30 + cr, color);
                }
                if (key == ConsoleKey.I)
                {

                    clk += 5;
                    end.DrawFont(9, 30 + clk, 30 + cr, color);
                }
                if (key == ConsoleKey.J)
                {

                    clk += 5;
                    end.DrawFont(10, 30 + clk, 30 + cr, color);
                }
                if (key == ConsoleKey.K)
                {

                    clk += 5;
                    end.DrawFont(11, 30 + clk, 30 + cr, color);
                }
                if (key == ConsoleKey.L)
                {

                    clk += 5;
                    end.DrawFont(12, 30 + clk, 30 + cr, color);
                }
                if (key == ConsoleKey.M)
                {

                    clk += 5;
                    end.DrawFont(13, 30 + clk, 30 + cr, color);
                    clk += 2;
                }
                if (key == ConsoleKey.N)
                {

                    clk += 5;
                    end.DrawFont(14, 30 + clk, 30 + cr, color);
                    clk += 1;
                }
                if (key == ConsoleKey.O)
                {

                    clk += 5;
                    end.DrawFont(15, 30 + clk, 30 + cr, color);
                    clk += 1;
                }
                if (key == ConsoleKey.P)
                {

                    clk += 5;
                    end.DrawFont(16, 30 + clk, 30 + cr, color);
                }
                if (key == ConsoleKey.Q)
                {

                    clk += 5;
                    end.DrawFont(17, 30 + clk, 30 + cr, color);
                    clk += 1;
                }
                if (key == ConsoleKey.R)
                {

                    clk += 5;
                    end.DrawFont(18, 30 + clk, 30 + cr, color);
                }
                if (key == ConsoleKey.S)
                {

                    clk += 5;
                    end.DrawFont(19, 30 + clk, 30 + cr, color);
                }
                if (key == ConsoleKey.T)
                {

                    clk += 5;
                    end.DrawFont(20, 30 + clk, 30 + cr, color);
                }
                if (key == ConsoleKey.U)
                {

                    clk += 5;
                    end.DrawFont(21, 30 + clk, 30 + cr, color);
                    clk += 1;
                }
                if (key == ConsoleKey.V)
                {

                    clk += 5;
                    end.DrawFont(22, 30 + clk, 30 + cr, color);
                }
                if (key == ConsoleKey.W)
                {

                    clk += 5;
                    end.DrawFont(23, 30 + clk, 30 + cr, color);
                    clk += 2;
                }
                if (key == ConsoleKey.X)
                {

                    clk += 5;
                    end.DrawFont(24, 30 + clk, 30 + cr, color);
                    clk += 2;
                }
                if (key == ConsoleKey.Y)
                {

                    clk += 5;
                    end.DrawFont(25, 30 + clk, 30 + cr, color);
                }
                if (key == ConsoleKey.Z)
                {

                    clk += 5;
                    end.DrawFont(26, 30 + clk, 30 + cr, color);
                }
                if (key == ConsoleKey.Spacebar)
                {

                    clk += 5;
                }
                if (key == ConsoleKey.Enter)
                {

                    cr += 10;
                    clk = 0;
                }
                if (key == ConsoleKey.UpArrow)
                {

                    if (color < 255) color++;
                }
                if (key == ConsoleKey.DownArrow)
                {

                    if (color > 0) color--;
                }
            }
        }
    }
}
