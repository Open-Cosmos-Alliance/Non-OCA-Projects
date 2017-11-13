using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS.IO.Filesystem
{
    public enum fileType
    {
        Directory = 1,
        Link = 2,
        Socket = 3,
        Pipe = 4,
        Executable = 5,
        BlockDevice = 6,
        CharDevice = 7

    }
    public class FileInfo
    {
        public string Name;
        public fileType Type;
        public string Permissions;
        public string Owner;
    }
    public abstract class Filesystem
    {
        public abstract Stream openFile(string file,int modes);
        public abstract void Unlink(string file);
        public abstract void Chmod(string file, string perms);
        public abstract void Chown(string file, string owner);
        public abstract GruntyOS.IO.Pipes.PipeServer mkfifo(string fifo);
        public abstract GruntyOS.IO.Networking.Socket mksocket(string name);
        public abstract List<FileInfo> List(string dir);
    }
}
