
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.IL2CPU.Plugs;
using GruntyOS.IO.Filesystem;
using CPUx86 = Cosmos.Assembler.x86;
using GruntyOS.IO;
using GruntyOS.IO.Pipes;
namespace GruntyOS.IO.Devices
{
    public class tty : Device
    {
        static int Count = 1;
        private int thisNumber;

        private static int[] openTTYs = new int[10];
        public tty()
        {
            this.Name = "tty" + Count.ToString();
            thisNumber = Count;
            this.Type = deviceType.charDevice;

            Count++;
        }
        public override Stream Open(int modes = 4)
        {
            ttyStream newStream = new ttyStream();
            openTTYs[thisNumber] = newStream.Descriptor;
            return newStream;
        }
        
    }
    class TTYState
    {
        public bool bright;
        public uint pointer;
        public bool hidden;
        public byte[] text;
        public int X = 0, Y = 0;
        public int foreColor, backColor;
    }
    public unsafe class ttyStream : Stream
    {
        private bool escapeSequence = false;
        ushort* video_memory = (ushort*)0xB8000;
        private string escapeString = "";
        private bool bright = false;
        private bool hidden = false;
        private ushort[] backBuffer;
        public ushort[] frontBuffer;
        private int foreColor = 7;
        private int backColor = 0;
        private int X;
        private int Y;
        private int backbufferPos = 0;
        private int frontBufferPos = 0;
        private TTYState backup;
        private TTYState saveState()
        {
            TTYState vb = new TTYState();

            vb.backColor = this.backColor;
            vb.foreColor = this.foreColor;
            vb.hidden = this.hidden;
            vb.bright = this.bright;
            vb.pointer = this.Pointer;
            byte* VideoRam = (byte*)0xB8000;
            vb.text= new byte[4250];
            for (int i = 0; i < 4250; i++)
            {
                byte b = VideoRam[i];
                vb.text[i] = b;
            }
            vb.X = X;
            vb.Y = Y;
            vb.foreColor = foreColor;
            vb.backColor = backColor;
            return vb;
            
        }
        private void restoreState(TTYState state)
        {
            this.hidden = state.hidden;
            this.bright = state.bright;
            this.X = state.X;
            this.Y = state.Y;
            this.Pointer = state.pointer;
            byte* VideoRam = (byte*)0xB8000;
            for (int i = 0; i < 4250; i++)
            {
                VideoRam[i] = state.text[i];
            }
        }
        private ConsoleColor[] colors = new ConsoleColor[] {ConsoleColor.DarkGray,ConsoleColor.DarkRed,ConsoleColor.DarkGreen,ConsoleColor.DarkYellow,ConsoleColor.DarkBlue,ConsoleColor.DarkMagenta,ConsoleColor.DarkCyan,ConsoleColor.Gray,ConsoleColor.DarkGray,ConsoleColor.Red,ConsoleColor.Green,ConsoleColor.Yellow,ConsoleColor.Blue,ConsoleColor.Magenta,ConsoleColor.Cyan,ConsoleColor.White};
        public ttyStream()
        {
            backBuffer = new ushort[25 * 80];
            frontBuffer = new ushort[25 * 80];
            base.Register();
            ushort val = Kernel.Inw(0x3DA);
            Kernel.Outw(val,0x3C0);
            val = Kernel.Inw(0x3C1);
            val = (ushort)(val & 0xF7);
            Kernel.Outw(val,0x3C0);
          
        }
        private void doBackBufferScroll()
        {
            for (int i = 0; i < 80 * 24; i++)
            {
                backBuffer[i] = backBuffer[i + 80];
            }
            backbufferPos--;
        }
        private void doFrontBufferScroll()
        {
            for (int i = 0; i < 80 * 24; i++)
            {
                frontBuffer[i] = frontBuffer[i + 80];
            }
            frontBufferPos--;
        }
        public void scrollDown()
        {
            if (frontBufferPos == 0)
                return;
            scroll();
            frontBufferPos--;
            for (int i = 24 * 80; i < 25 * 80; i++)
                video_memory[i] = frontBuffer[(80 * frontBufferPos) + ((80 * 24) - i)];

        }
        public void scrollUp()
        {
            if (backbufferPos == 0)
                return;
            int i;
            backbufferPos--;
            for (i = 24 * 80; i < 80 * 25; i++)
            {
                frontBuffer[(80 * frontBufferPos) + ((80 * 24) - i)] = video_memory[i];
            }
            if (frontBufferPos == 25)
                doFrontBufferScroll();
            frontBufferPos++;

            for (i = 80 * 25; i >= 80; i--)
            {
                video_memory[i] = video_memory[i - 80];
                /*
                 * Hello
                 * World
                 * 
                 */
            } 
            for (i = 0; i < 80; i++)
            {
                video_memory[i] = backBuffer[(80 * (backbufferPos)) + i];
            }
        }
        public override int  readByte(uint ptr)
        {
            STIEnabler sti = new STIEnabler();
            sti.Enable();
            uint ic = Cosmos.Hardware.Global.Keyboard.ReadScancode();
            char c;
            Cosmos.Hardware.Global.Keyboard.GetCharValue(ic, out c);
    
            if (ic == 14)
            {
                return 8;
            }
            else
            {
                return (byte)c;
            }
        }
        void scroll()
        {

            uint attributeByte = (((uint)Console.BackgroundColor) << 4) | (((uint)Console.ForegroundColor) & 0x0F);
            ushort blank = (ushort)(0x0 | (attributeByte << 8));
            int i;
            for (i = 0; i < 80; i++)
            {
                backBuffer[(backbufferPos * 80) + i] = video_memory[i];
            }
            for (i = 0 * 80; i < 24 * 80; i++)
            {
                video_memory[i] = video_memory[i + 80];
            }
            if (backbufferPos == 25)
                doBackBufferScroll();
            backbufferPos++;
            for (i = 24 * 80; i < 25 * 80; i++)
            {
                video_memory[i] = blank;
            }
            Y = 24;

        }

        public static void update_cursor(ushort row, ushort col)
        {
            ushort position = (ushort)((row * 80) + col + 1);
            Kernel.Outb(0x3D4, (byte)0x0F);
            Kernel.Outb(0x3D5, (byte)(position & 0xFF));
            Kernel.Outb(0x3D4, 0x0E);
            Kernel.Outb(0x3D5, (byte)((position >> 8) & 0xFF));

        }
        public static int Set(int Value, byte Bit, bool On)
        { return On ? Value | (1 << Bit) : Clear(Value, Bit); }
        public static int Clear(int Value, byte Bit)
        { return Value & ~(1 << Bit); }
        private void printc(uint ptr,byte data)
        {

            Y = (int)(((int)ptr) / 80);
            X = (int)(ptr - ((80 * Y)));
            update_cursor((ushort)(Y ), (ushort)X);
            if (data == (byte)('\n'))
            {


                uint i;
                for (i = ptr + 1; (i) % 80 != 0; i++) ;
                Pointer = i - 1;
                if (Pointer + 1 >= (uint)(80 * 25))
                {
                    scroll();
                    Pointer = (80 * 24) - 1;
                   
                }
            }
            else if (data == (byte)'\r')
            {
                Pointer--;
                return;
            }
            else
            {
                byte* vram = (byte*)0xB8000; byte attributeByte;
                attributeByte = (byte)((((uint)this.colors[backColor] << 4) | (((uint)this.colors[foreColor]) & 0x0F)));
                if (bright)
                {
                    attributeByte = (byte)Set(attributeByte, 7, true);
                    attributeByte = (byte)Set(attributeByte, 3, true);
                }
                vram[(ptr * 2)] = data;
                vram[(ptr * 2) + 1] = attributeByte;
            }
        }
        public override void writeByte(uint ptr, byte data)
        {
            if (data == '\\')
            {
                escapeString = "";
                escapeSequence = true;
                this.Pointer--;
                return;
            }
            else if (!escapeSequence)
            {
                if(!hidden)
                    printc(ptr, data);
            }
            else
            {
                this.Pointer--;
                if ((char)data == 'm')
                {
                    escapeSequence = false;
                    string attr1 = escapeString[0].ToString();
                    int extra = 0;
                    string args = "";
                    if (attr1 == "0")
                    {
                        bright = false;
                        hidden = false;
                        foreColor = 7;
                        backColor = 0;
                            
                    }
                    else if (attr1 == "1")
                        bright = true;
                    else if (attr1 == "2")
                        bright = false;
                    if (escapeString.Length >= 2)
                        args = escapeString.Substring(2);
                    else
                        return;
                    foreach (string str in args.Split(';'))
                    {
                        int d = Conversions.StringToInt(str);
                        if (d >= 30 && d < 40)
                        {

                            foreColor = d - 30;
                        }
                        else if (d >= 40)
                        {
                            backColor = d - 40;
                        }
                        else
                        {
                        }
                    }
                    return;
                }
                else if (data == (byte)'[')
                {
                    return;
                }
              
                escapeString += ((char)data).ToString();
                if (escapeString == "8")
                {
                    restoreState(backup);
                    escapeSequence = false;
                }
                else if (escapeString == "e")
                {
                    escapeSequence = false;
                    printc(ptr, (byte)'\\');
                    this.Pointer++;
                    return;
                }
                else if (escapeString == "d")
                {
                    escapeSequence = false;
                    scrollDown();
                }
                else if (escapeString == "u")
                {
                    escapeSequence = false;
                    scrollUp();
                }
                else if (escapeString == "c")
                {
                    update_cursor(0, 0);
                    for (int i = 0; i < 4250; i++)
                    {
                        video_memory[i] = 0;
                    }
                    Pointer = 0;
                    X = 0;
                    Y = 0;
                    escapeSequence = false;
                    return;
                }
                else if (escapeString == "7")
                {
                    backup = saveState();
                    escapeSequence = false;
                }
            }
        }
    }
}

class STIEnabler
{

    [PlugMethod(Assembler = typeof(Enable))]
    public void Enable()
    {
    }
}
[Plug(Target = typeof(STIEnabler))]
class Enable : AssemblerMethod
{
    public override void AssembleNew(object aAssembler, object aMethodInfo)
    {
        new CPUx86.Sti();

    }
}