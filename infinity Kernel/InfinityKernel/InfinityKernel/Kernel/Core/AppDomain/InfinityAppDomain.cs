using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GruntyOS.IO;
namespace GruntyOS
{
    public unsafe static partial class Kernel
    {
        private static Stack<InfinityAppDomain> domains = new Stack<InfinityAppDomain>();
        public static InfinityAppDomain currentDomain
        {
            get
            {
                return domains.Peek();
            }
            set
            {
                domains.Push(value);
            }
        }
        public static void SetAppDomain(InfinityAppDomain domain)
        {
            currentDomain = domain;
        }
    }
    public class InfinityAppDomain
    {
        public uint UID, GID;
        public string Name;
        public List<Stream> openedStreams = new List<Stream>();
        public InfinityAppDomain()
        {
            
        }
        public static InfinityAppDomain CreateDomain(string name)
        {
            InfinityAppDomain ret = new InfinityAppDomain();
            ret.Name = name;
            ret.UID = Kernel.getUID();
            ret.GID = Kernel.getGID();
            return ret;
        }
    }
}
