using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS
{
    public class Gshell : Applet
    {
        private string PS1 = @"\[0;34mdebug \[0;37m\$ \[0;m";
        private string CD = "/";
        private string getRealPS1
        {
            get
            {
                string ret = "";
                for (int i = 0; i < PS1.Length; i++)
                {
                    if (PS1[i] == '\\' && PS1[i + 1] == '$')
                    {
                        if (Kernel.getUID() == 0)
                            ret += "#";
                        else
                            ret += "$";
                        ++i;
                    }
                    else if (PS1[i] == '\\' && PS1[i + 1] == 'u')
                    {
                        if (Kernel.getUID() == 0)
                            ret += "root";
                        else
                            ret += "notYetImplemented";
                        ++i;
                    }
                    else if (PS1[i] == '\\' && PS1[i + 1] == 'w')
                    {
                        ret += "/";
                        ++i;
                    }
                    else
                        ret += PS1[i];
                }
                return ret;
            }
        }
        protected override void Run(string[] args)
        {
            printf("Welcome the Infinity debug shell. !\n");
           
            while (true)
            {
                printf(getRealPS1);
                string cmd = stdio.readln();
            }
        }
    }
}
