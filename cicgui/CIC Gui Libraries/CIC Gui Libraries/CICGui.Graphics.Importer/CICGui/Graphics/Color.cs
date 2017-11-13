using Cosmos.Hardware;
using System;

namespace CICGui.Graphics
{
    public class Color
    {
        public static int Black = 0;
        public static int Blue = 0xff;
        public static int Green = 0x3e418;
        public static int Red = 0xf32fdc0;
        public static int White = 0xf36e2d7;

        public static void Init()
        {
            VGAScreen.SetPaletteEntry(Black, 0, 0, 0);
            VGAScreen.SetPaletteEntry(White, 0xff, 0xff, 0xff);
            VGAScreen.SetPaletteEntry(Red, 0xff, 0, 0);
            VGAScreen.SetPaletteEntry(Blue, 0, 0, 0xff);
            VGAScreen.SetPaletteEntry(Green, 0, 0xff, 0);
        }
    }
}

