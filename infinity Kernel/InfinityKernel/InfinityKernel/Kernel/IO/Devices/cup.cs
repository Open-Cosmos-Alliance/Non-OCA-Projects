using System;
namespace GruntyOS.IO.Devices
{
    public class cupDev : Device
    {
        public cupDev()
        {
            this.Name = "cup";
            this.Type = deviceType.charDevice;
        }
        public override Stream Open(int modes = 4)
        {
            return new cupStream();
        }
    }
    public class cupStream : Stream
    {
        private string anscii_art = @"                              \[1;40m                              \[0;0m                     
                \[0;41m            \[1;40m  \[1;47m                              \[1;40m  \[0;m
\[0;41m                          \[0;40m  \[1;47m      \[0;45m                      \[1;47m      \[1;40m  \[0;m
\[0;41m                          \[0;40m  \[1;47m    \[0;45m          \[1;45m  \[0;45m    \[1;45m  \[0;45m        \[1;47m    \[1;40m  \[0;m
\[0;43m                          \[0;40m  \[1;47m  \[0;45m    \[1;45m  \[0;45m              \[1;40m    \[1;45m  \[0;45m    \[1;47m  \[1;40m  \[0;m    \[1;40m    \[0;m
\[0;43m                          \[0;40m  \[1;47m  \[0;45m                  \[1;40m  \[1;47m    \[1;40m  \[0;45m    \[1;47m  \[1;40m  \[0;m  \[1;40m  \[1;47m    \[1;40m  \[0;m
\[0;43m                \[0;43m          \[1;40m  \[1;47m  \[0;45m          \[1;45m  \[0;45m      \[1;40m  \[1;47m      \[0;45m    \[1;47m  \[1;40m    \[1;47m      \[1;40m  \[0;m
\[1;43m                  \[0;40m  \[0;43m      \[1;40m  \[1;47m  \[0;45m                  \[1;40m  \[1;47m      \[1;40m        \[1;47m        \[1;40m  \[0;m
\[1;43m                \[0;40m  \[1;47m  \[1;40m  \[0;43m    \[1;40m  \[1;47m  \[0;45m                  \[1;40m  \[1;47m                      \[1;40m  \[0;m
\[1;43m                \[0;40m  \[1;47m  \[1;40m        \[1;47m  \[0;45m            \[1;45m  \[0;45m  \[1;40m  \[1;47m                          \[1;40m  \[0;m
\[1;42m                  \[1;47m        \[1;40m  \[1;47m  \[0;45m  \[1;45m  \[0;45m            \[1;40m  \[1;47m      \[1;47m  \[1;40m  \[1;47m        \[1;47m  \[1;40m  \[1;47m    \[1;40m  \[0;m
\[1;42m                  \[0;40m    \[1;47m    \[1;40m  \[1;47m  \[0;45m                \[1;40m  \[1;47m      \[1;40m    \[1;47m    \[1;40m  \[1;47m  \[1;40m    \[1;47m    \[1;40m  \[0;m
\[1;42m                \[1;44m      \[1;40m      \[1;47m    \[0;45m      \[1;45m  \[0;45m      \[1;40m  \[1;47m  \[0;45m    \[1;47m                \[0;45m    \[1;40m  \[0;m
\[1;44m                          \[0;40m  \[1;47m      \[0;45m              \[1;40m  \[1;47m      \[1;40m            \[1;47m    \[1;40m  \[0;m
\[1;44m                \[0;44m          \[1;40m    \[1;47m                    \[1;40m  \[1;47m                  \[1;40m  \[0;m
\[0;44m                        \[0;40m  \[1;47m    \[1;40m                                        \[0;m          
\[0;45m                        \[0;40m  \[1;47m    \[1;40m  \[0;m  \[1;40m  \[1;47m  \[1;40m  \[0;m            \[1;47m    \[1;40m  \[0;m  \[1;40m  \[1;47m    \[1;40m  \[0;m 
\[0;45m                \[0;m        \[0;40m      \[0;m      \[0;40m    \[0;m            \[0;40m      \[0;m    \[0;40m      \[0;m           
       ";
        public cupStream()
        {
            base.Register();
        }
        public override void writeByte(uint ptr, byte data)
        {

        }
        public override int readByte(uint ptr)
        {
            if (ptr >= anscii_art.Length)
                return -1;
            else
                return (byte)anscii_art[(int)ptr];
        }
    }
}

