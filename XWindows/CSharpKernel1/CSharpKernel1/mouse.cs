using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpKernel1
{
    class mouse
    {      
        public static void Initialize(Cosmos.Hardware.VGAScreen scr, window ParentWnd)
        {
            Cosmos.Hardware.Mouse Mouse2 = new Cosmos.Hardware.Mouse();

            Mouse2.Initialize();

            uint x = (uint)Mouse2.X;
            uint y = (uint)Mouse2.Y;

            uint oc = 0;

            uint k = 0;

            window.DrawKeyboard(ParentWnd);

            //window lst = window.Last(ParentWnd);

            window nd = new window(10, 20, 50, 20, 5, window.LastWnd, false);

            window end = new window(ParentWnd.Width / 2, ParentWnd.Width / 2, 50, 30, 5, nd, false);
            end.StringTo("DIALOG WND");

            window wnd = null; window act = null;

            window.ReDraw(ParentWnd);

            while (true)
            {
                #region Mouse Data
                uint mx = (uint)Mouse2.X;
                uint my = (uint)Mouse2.Y;

                if (mx != x || my != y)
                {
                    scr.SetPixel320x200x8(x, y, oc);
                    x = mx;
                    y = my;
                    oc = scr.GetPixel320x200x8(x, y);
                }

                switch (k) 
                {
                    case 1:
                        act.Sync(mx, my, ParentWnd);
                        break;
                    case 2:
                        window.Destroy(act, ParentWnd);
                        k = 0;
                        break;
                    case 3:
                        act.SynSz(mx, my, ParentWnd);
                        break;
                    case 4:
                        window l = window.LastWnd;
                        if (l.ch == 666) window.Destroy(l, ParentWnd);
                        k = 0;
                        break;
                }

                scr.SetPixel320x200x8(mx, my, 6);
               
                if (Mouse2.Buttons == Cosmos.Hardware.Mouse.MouseState.Right)
                {
                    window last = window.LastWnd;

                    if ((last.modal == true) && (last.ch == 666))
                    {
                        window.Destroy(last, ParentWnd);
                    }

                    wnd = new window(0, 0, 51, 51, oc, window.LastWnd, true);
       
                    wnd.color = 3;
                    wnd.ch = 666;
                    wnd.StringTo("CREATE WND");
                    wnd.Sync(mx, my, ParentWnd);
                }
            
                if (Mouse2.Buttons == Cosmos.Hardware.Mouse.MouseState.Left)
                {
                    act = window.Active(mx, my, window.LastWnd);

                    if (act.modal == true)
                    {
                        if (act.ch == 666)
                        {
                            window newWnd = new window(ParentWnd.Width / 3, ParentWnd.Width / 3, 50, 30, 5, act.parent, false);
                        }
                        else if (act.ch == 0)
                        {
                            k = 4;
                        }
                        else 
                        {
                            nd.CharTo(act.ch - 1);
                        }
                    }
                    else
                    {
                        k = act.Status(mx, my);
                    }

                    window.ReDraw(ParentWnd);
                }

                if (Mouse2.Buttons == Cosmos.Hardware.Mouse.MouseState.Middle)
                {
                    k = 0;
                }
                #endregion
            }
        }
    }
}
