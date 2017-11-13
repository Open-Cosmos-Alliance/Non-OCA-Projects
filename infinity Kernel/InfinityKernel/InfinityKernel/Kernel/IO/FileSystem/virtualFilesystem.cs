using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GruntyOS.IO.Filesystem
{
    public class RootFS : Filesystem
    {
        public class mountPoint
        {
            public string Path;
            public Filesystem Filesytem;
        }
        public List<mountPoint> getMountPoints()
        {
            return this.mountedFileSystems;
        }
        public static int IndexOf(string str, char c)
        {
            int i = 0;
            foreach (char ch in str)
            {
                if (ch == c)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }
        public static bool Contains(string Str, char c)
        {
            foreach (char ch in Str)
            {
                if (ch == c)
                    return true;
            }
            return false;
        }

        public static string cleanName(string name)
        {
            if (!Contains(name, '/'))
                return name;
            if (name.Substring(0, 1) == "/")
            {
                name = name.Substring(1, name.Length - 1);
            }
            if (!Contains(name, '/'))
                return name;
            if (name.Substring(name.Length - 1, 1) == "/")
            {
                name = name.Substring(0, name.Length - 1);
            }
            return name;
        }

        private List<mountPoint> mountedFileSystems = new List<mountPoint>();
        public Filesystem getFS(string point)
        {
            for (int i = 0; i < mountedFileSystems.Count; i++)
            {
                
                if (mountedFileSystems[i].Path.Length <= point.Length)
                {
                   // Console.WriteLine("/" + cleanName(point.Substring(0, mountedFileSystems[i].Path.Length)) + "/ vs " +  mountedFileSystems[i].Path);
                    if ("/" + cleanName(point.Substring(0,mountedFileSystems[i].Path.Length)) + "/" == mountedFileSystems[i].Path)
                    {
                        return mountedFileSystems[i].Filesytem;
                    }
                }
            }
            throw new Exception("Mount point " + point + " does not exist");
        }
        public string getFSName(string point)
        {
            for (int i = 0; i < mountedFileSystems.Count; i++)
            {
                if (mountedFileSystems[i].Path.Length <= point.Length)
                {
                    if ("/" + cleanName(point.Substring(0,mountedFileSystems[i].Path.Length)) + "/" == mountedFileSystems[i].Path)
                    {
                        return cleanName(point.Substring(mountedFileSystems[i].Path.Length));
                    }
                }
            }
            throw new Exception("Mount point " + point + " does not exist");
        }
        public void Umount(string point,bool force = false)
        {
            List<mountPoint> newMountPoints = new List<mountPoint>();
            for (int i = 0; i < mountedFileSystems.Count; i++)
            {
                if (mountedFileSystems[i].Path != point)
                    newMountPoints.Add(mountedFileSystems[i]);
            }
            this.mountedFileSystems = newMountPoints;
        }
        public void Mount(Filesystem fs, string point)
        {
            Kernel.printf("<6>Mounting new filesystem to %s\n", point);
            mountPoint mp = new mountPoint();
            mp.Path = "/" + cleanName(point) + "/";
            mp.Filesytem = fs;
            mountedFileSystems.Add(mp);
        }
        public override void Chmod(string file, string perms)
        {
            throw new NotImplementedException();
        }
        public override void Chown(string file, string owner)
        {
            throw new NotImplementedException();
        }
        public override List<FileInfo> List(string dir)
        {
            return getFS(dir).List(getFSName(dir));
        }
        public override Stream openFile(string file,int modes)
        {
            return getFS(file).openFile(getFSName(file),modes);
        }
        public override GruntyOS.IO.Networking.Socket mksocket(string file)
        {
            return getFS(file).mksocket(getFSName(file));
        }

        public override GruntyOS.IO.Pipes.PipeServer mkfifo(string file)
        {
            return getFS(file).mkfifo(getFSName(file));
        }
        public override void Unlink(string file)
        {
            throw new NotImplementedException();
        }
    }
}
