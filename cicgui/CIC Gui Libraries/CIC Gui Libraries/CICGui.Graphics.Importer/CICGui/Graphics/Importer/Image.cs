using Cosmos.Hardware;
using CICGui.Math;
using System;

namespace CICGui.Graphics.Importer
{
    public class Image
    {
        public int Height;
        public string[] strData;
        public int Width;

        private Image()
        {
            this.strData = null;
            this.Width = 1;
            this.Height = 1;
            this.strData = new string[(this.Height + 1) * (this.Width + 1)];
        }

        public Image(int width, int height)
        {
            this.strData = null;
            this.Width = width;
            this.Height = height;
            this.strData = new string[(this.Height + 1) * (this.Width + 1)];
        }

        public void Dispose()
        {
            this.strData = null;
        }

        public void Draw(uint x, uint y)
        {
            uint num = 0;
            uint num2 = 0;
            uint num3 = 0;
            string str2 = "";
            for (uint i = 0; i < this.strData.Length; i++)
            {
                if ((this.strData[i] != "666") && (this.strData[i] != "666666666"))
                {
                    if (str2 != this.strData[i])
                    {
                        uint num4 = CICGui.Math.Math.ToUint(this.strData[i].Remove(3));
                        uint num5 = CICGui.Math.Math.ToUint(this.strData[i].Remove(0, 3).Remove(3, 3));
                        uint num6 = CICGui.Math.Math.ToUint(this.strData[i].Remove(0, 6));
                        VGAScreen.SetPaletteEntry((int) CICGui.Math.Math.ToUint(this.strData[i]), (byte) num4, (byte) num5, (byte) num6);
                        str2 = this.strData[i];
                    }
                    VGAScreen.SetPixel320x200x8(x + num2, y + num, CICGui.Math.Math.ToUint(this.strData[i]));
                }
                num3++;
                num2++;
                if (num3 >= this.Width)
                {
                    num3 = 0;
                    num++;
                    num2 = 0;
                }
            }
        }

        public string GetPixel(uint x, uint y)
        {
            return this.strData[(int) ((IntPtr) ((x + 1) * (y + 1)))];
        }

        public void ImportData(string[] dData)
        {
            this.strData = dData;
        }

        public void SetPixel(uint x, uint y, string color)
        {
            this.strData[(int) ((IntPtr) ((x + 1) * (y + 1)))] = color;
        }
    }
}

