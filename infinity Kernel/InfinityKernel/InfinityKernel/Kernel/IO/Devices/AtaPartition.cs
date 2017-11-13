/*******************************************************
 * Copyright (C) 2012-2013 GruntXProductions <sloan@gruntxproductions.net>
 * 
 * This file is part of Grunty OS Infinity
 * 
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential.
 *******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Hardware.BlockDevice;
using GruntyOS.IO;

using System.Runtime.InteropServices;
namespace GruntyOS.IO.Devices
{
    public class StorageDevice : Device
    {
        static char n = 'a';
        private Cosmos.Hardware.BlockDevice.AtaPio dev;

        public StorageDevice(Cosmos.Hardware.BlockDevice.AtaPio d)
        {
            dev = d;
            this.Name = "sd" + n.ToString();
            n++;
        }
        public override Stream Open(int modes = 4)
        {
            return new ATAStream(dev);
        }
    }
    public class ATAStream : Stream
    {

        public Cosmos.Hardware.BlockDevice.BlockDevice dev;
        private byte[] buff;
        private uint lastSector = 0xFFFFF;
        private bool readStart = false;
        public ATAStream(Cosmos.Hardware.BlockDevice.BlockDevice bd)
        {
            buff = new byte[bd.BlockSize];
            dev = bd;
            Register();
        }
        public override int readByte(uint ptr)
        {
            uint block = ((uint)ptr / 512);
            uint addr = (uint)ptr - ((block) * 512);
            if (lastSector != block)
            {
                if (readStart)
                    dev.WriteBlock(lastSector, 1, buff);
                dev.ReadBlock(block, 1, buff);
               
            }
            lastSector = block;
            readStart = true;
            return (int)buff[addr];
        }
        public override void writeByte(uint ptr, byte data)
        {
            uint block = ((uint)ptr / 512);
            uint addr = (uint)ptr - ((block) * 512);
            if (block != lastSector && readStart)
            {
                dev.WriteBlock(lastSector, 1, buff);
                dev.ReadBlock(block, 1, buff);
            }
            buff[addr] = data; 
            if(block != lastSector)
                dev.WriteBlock(block, 1, buff);
            readStart = true;
            lastSector = block;
        }
    }
}