using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS.IO
{
    public static class File
    {
        public static Stream Open(string file, int modes)
        {
            return Kernel.Root.openFile(file,modes);
        }
    }
}
