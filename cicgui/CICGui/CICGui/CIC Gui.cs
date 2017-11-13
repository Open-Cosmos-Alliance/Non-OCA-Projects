using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace CICGui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label3.Text = "Status: Waiting to proceed.";
        }

        static Bitmap file;

        public static string formatRGB(string xxx, int xx, int yy)
        {
            if (xxx.Length == 2) xxx = "0" + xxx;
            if (xxx.Length == 1) xxx = "00" + xxx;
            return xxx;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool stop = false;
            if (!File.Exists(textBox1.Text)) { label3.Text = "Error: File doesn't exist!"; stop = true; } //restarts if bad file url
            if (!stop)
            {
                file = new Bitmap(textBox1.Text);
                label3.Text = "Status: File loaded.";
                string output;
                output = "using System;" + System.Environment.NewLine +
                    "using System.Collections.Generic;" + System.Environment.NewLine +
                    "using Cosmos.Hardware;" + System.Environment.NewLine +
                    "using Cosmos.Kernel;" + System.Environment.NewLine +
                    "using CICGui.Graphics.Importer;" + System.Environment.NewLine + System.Environment.NewLine +
                    "namespace CICGui.Graphics" + System.Environment.NewLine +
                    "{" + System.Environment.NewLine +
                    "   class IMG_Name" + System.Environment.NewLine +
                    "   {" + System.Environment.NewLine +
                    "       public static Image IMG = new Image(" + file.Width + ", " + file.Height + ");" + System.Environment.NewLine +
                    "       public static string[] imgData = {" + System.Environment.NewLine +
                    "           ";

                for (int yy = 0; yy < file.Height; yy++)
                {
                    for (int xx = 0; xx < file.Width; xx++)
                    {
                        if (file.GetPixel(xx, yy).A < 200)
                            output += '"' + "666666666" + '"' + ", ";
                        else
                            output += '"' + formatRGB("" + file.GetPixel(xx, yy).R, xx, yy) + "" + formatRGB("" + file.GetPixel(xx, yy).G, xx, yy) + "" + formatRGB("" + file.GetPixel(xx, yy).B, xx, yy) + '"' + ", ";
                    }
                    output += System.Environment.NewLine + "           ";
                }
                output += '"' + "666" + '"' + System.Environment.NewLine +
                    "       };" + System.Environment.NewLine +
                    "       public static void Draw(uint x, uint y)" + System.Environment.NewLine +
                    "       {" + System.Environment.NewLine +
                    "           IMG.ImportData(imgData);" + System.Environment.NewLine +
                    "           IMG.Draw(x, y);" + System.Environment.NewLine +
                    "       }" + System.Environment.NewLine +
                    "       " + System.Environment.NewLine +
                    "       public static void Dispose()" + System.Environment.NewLine +
                    "       {" + System.Environment.NewLine +
                    "           IMG.Dispose();" + System.Environment.NewLine +
                    "       }" + System.Environment.NewLine +
                    "    }" + System.Environment.NewLine +
                    "}";
                label3.Text = "Status: File being created...";
                string b = textBox2.Text;
                output = output.Replace("IMG_Name", b);
                string newPath = System.IO.Path.Combine(Application.UserAppDataPath, "CIC Gui Output");
                System.IO.Directory.CreateDirectory(newPath);


                using (StreamWriter file2 = new System.IO.StreamWriter(Application.UserAppDataPath + "/CIC Gui Output/" + b + ".cs", false))
                {
                    file2.Write(output);
                }
                label3.Text = "Status: Complete! See AppData Output Folder";
            }
        }
    }
}
