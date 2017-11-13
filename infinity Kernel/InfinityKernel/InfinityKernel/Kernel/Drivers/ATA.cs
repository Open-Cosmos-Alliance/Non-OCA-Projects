
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Hardware.BlockDevice;
using GruntyOS.IO;

using System.Runtime.InteropServices;
using GruntyOS.Core;
namespace GruntyOS.HAL
{
    public class PartitionDev : GruntyOS.IO.Device
    {
        private Partition part;
        public PartitionDev(Partition p,string n)
        {
            part = p;
            this.Name = n;
        }
        public override Stream Open(int modes = 4)
        {
            return new GruntyOS.IO.Devices.ATAStream(part);
        }
    }
    public class ATA :  Service
    {
        private class MBREntry
        {
            public byte Boot;
            public byte StartingHead;
            public byte StartingSector;
            public ushort StartingCylider;
            public byte SystemID;
            public byte EndingHead;
            public byte EndingSector;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        unsafe struct GPTHeader
        {
            [FieldOffset(0)]
            public fixed byte Magic[8];
            [FieldOffset(8)]
            public uint Revision;
            [FieldOffset(12)]
            public uint HeaderSize;
            [FieldOffset(16)]
            public uint CRC32;
            [FieldOffset(20)]
            public uint Reserved;
            [FieldOffset(24)]
            public ulong CurrentLBA;
            [FieldOffset(32)]
            public ulong BackupLBA;
            [FieldOffset(40)]
            public ulong FirstUsableLBA;
            [FieldOffset(48)]
            public ulong LastUsableLBA;
            [FieldOffset(56)]
            public fixed byte DiskGUI[16];
            [FieldOffset(72)]
            public ulong StartingLBA;
            [FieldOffset(80)]
            public uint PartitionCount;

        }
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        unsafe struct GPTEntry
        {
            [FieldOffset(0)]
            public fixed byte PartitionTypeGUID[16];
            [FieldOffset(16)]
            public fixed byte UniquePartitionGUID[16];
            [FieldOffset(32)]
            public ulong FirstLBA;
            [FieldOffset(40)]
            public ulong LastLBA;
            [FieldOffset(48)]
            public fixed byte PartitionName[72];
        }
        private static char HDlabel = 'a';
        public static void ReadMBR(string File)
        {
            Console.WriteLine(File);
            BinaryReader br = new BinaryReader(stdio.fopen(File));
            br.BaseStream.Pointer = 0;
            // null fill!!!!
            for (int i = 0; i < 4; )
            {
                br.BaseStream.Pointer = (uint)(446 + (i * 16) + 8);
                uint relativeSector = br.ReadUInt32();
                uint sectorCount = br.ReadUInt32();

                if (sectorCount != 0u)
                {
                    Kernel.printf("<6>Creating device node for ATA partition (%s%d)\n",File,i + 1);
                    string devname = File.Substring(File.Length - 3);
                    GruntyOS.IO.Devices.ATAStream ataStrm = new IO.Devices.ATAStream(new Partition(((IO.Devices.ATAStream)br.BaseStream).dev, relativeSector, sectorCount));
                    Kernel.devFS.Devices.Add(new PartitionDev(new Partition(((IO.Devices.ATAStream)br.BaseStream).dev, relativeSector, sectorCount), devname + i.ToString()));
                }
                i++;
            }
        }
        public unsafe static void ReadGPT(AtaPio dev, uint count)
        {
            /*
            BinaryReader br = new BinaryReader(MBR);
            byte counter = 0;

            fixed (byte* ptr = tmp)
            {
                for (uint i = 0; i < 32; i++)
                {
                    dev.ReadBlock(2 + i, 1, tmp);
                    for (int b = 0; b < 4; b++)
                    {
                        GPTEntry* partition = (GPTEntry*)(uint)(ptr + ((uint)b * 128u));

                        if ((int)(partition->FirstLBA) != 0)
                        {
                            Devices.device d = new Devices.device();
                            Partition p = new Partition(dev, partition->FirstLBA, (partition->LastLBA - partition->FirstLBA));
                            d.dev = p;
                            d.name = "/dev/sd" + HDlabel.ToString();
                            int device = ((int)counter + 1);
                            d.name += device.ToString();
                            Devices.dev.Add(d);
                        }
                        counter++;
                    }

                }
            }
             */
        }
        public ATA()
        {
            this.Name = "ATA Manager";
        }
        public override bool Init()
        {
            Kernel.printf("<5>Searching for disks\n");
            for (int i = 0; i < Cosmos.Hardware.BlockDevice.BlockDevice.Devices.Count; i++)
            {
                if (Cosmos.Hardware.BlockDevice.AtaPio.Devices[i] is AtaPio)
                {
                    GruntyOS.IO.Devices.StorageDevice sd = new IO.Devices.StorageDevice((AtaPio)Cosmos.Hardware.BlockDevice.BlockDevice.Devices[i]);
                    Kernel.devFS.Devices.Add(sd);
                    printk("<5>IDE ATA Device Found! Scanning for partitions!\n");
                    ReadMBR("/dev/" + sd.Name);
                }
            }
                return true;
        }
    }
}
