using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS
{
    public static partial class Kernel
    {
        private static List<GruntyOS.Core.Service> services = new List<Core.Service>();
        public static void InitServices()
        {
            printf("Initializing core services\n\n");
            for (int i = 0; i < services.Count; i++)
            {

                if (services[i].Init())
                {

                }
                else
                {
                    printf("<3>Could not start %s\n", services[i].Name);
                }
            }
        }
        public static void registerService(GruntyOS.Core.Service srv)
        {
            services.Add(srv);
        }
    }
}
namespace GruntyOS.Core
{
    public enum Priority
    {
        HIGH = 1,
        MEDIUM = 2,
        LOW = 3,
    }
    public abstract class Service
    {
        public string Name;
        public Priority priority = Priority.MEDIUM;
        public abstract bool Init();
        public void printk(string format,params object[] args)
        {
            Kernel.printf(format, args);
        }
        public virtual void onSync()
        {
        }
    }
}
