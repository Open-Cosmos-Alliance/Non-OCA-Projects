using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS
{
    public unsafe partial class Kernel
    {
        /// <summary>
        /// Begins a mutual exlusion lock, note this will
        /// stop ALL threading, so be carefull.
        /// </summary>
        public static void mutexLock()
        {
        }
        public static void mutexUnlock()
        {
        }
    }
}
