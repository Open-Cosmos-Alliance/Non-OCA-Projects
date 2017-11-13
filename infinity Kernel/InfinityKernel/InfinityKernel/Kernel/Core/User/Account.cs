using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS
{
    public unsafe partial class Kernel 
    {
        private static uint UID, GID;
        public static void setGID(uint gid)
        {
            GID = gid;
        }
        public static uint getGID()
        {
            return GID;
        }
        public static void setUID(uint uid)
        {
            UID = uid;
        }
        public static uint getUID()
        {
            return UID;
        }
    }
}
