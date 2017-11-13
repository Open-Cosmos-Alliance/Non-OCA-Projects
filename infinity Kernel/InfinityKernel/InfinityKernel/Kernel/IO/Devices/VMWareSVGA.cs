using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GruntyOS.IO;
using GruntyOS;
using Cosmos.Core;


namespace GruntyOS.IO.Devices
{
    class frameBuffer : Device
    {
        public static int frameBuffers = 1;
        public frameBuffer()
        {
            this.Name = "fb" + frameBuffers.ToString();
            this.Type = deviceType.blockDevice;
            frameBuffers++;
        }
        public override Stream Open(int modes = 4)
        {
            return new svgaStream();
        }
    }
    class svgaStream : Stream
    {
        VMWareSVGA svga = new VMWareSVGA();
        private byte[] pixelData;
        int byteIndex = 0;
        public svgaStream()
        {
            svga = new VMWareSVGA();
            svga.SetMode(800, 600);
        }
        public override int readByte(uint ptr)
        {
            return base.readByte(ptr);
        }
        public override void writeByte(uint ptr, byte dat)
        {
            pixelData[byteIndex] = dat;
            byteIndex++;
            if (byteIndex > 4)
            {
                uint color = System.BitConverter.ToUInt32(pixelData,0);
                svga.Video_Memory[ptr] = color;
                byteIndex = 0;
                svga.Update(0, 0, 800, 600);
            }
            else
                Pointer--;
        }
    }

    class VMWareSVGA
    {

        public enum Register : ushort
        {
            ID = 0,
            Enable = 1,
            Width = 2,
            Height = 3,
            MaxWidth = 4,
            MaxHeight = 5,
            Depth = 6,
            BitsPerPixel = 7,
            PseudoColor = 8,
            RedMask = 9,
            GreenMask = 10,
            BlueMask = 11,
            BytesPerLine = 12,
            FrameBufferStart = 13,
            FrameBufferOffset = 14,
            VRamSize = 15,
            FrameBufferSize = 16,
            Capabilities = 17,
            MemStart = 18,
            MemSize = 19,
            ConfigDone = 20,
            Sync = 21,
            Busy = 22,
            GuestID = 23,
            CursorID = 24,
            CursorX = 25,
            CursorY = 26,
            CursorOn = 27,
            HostBitsPerPixel = 28,
            ScratchSize = 29,
            MemRegs = 30,
            NumDisplays = 31,
            PitchLock = 32,
            FifoNumRegisters = 293
        }

        private enum ID : uint
        {
            Magic = 0x900000,
            V0 = Magic << 8,
            V1 = (Magic << 8) | 1,
            V2 = (Magic << 8) | 2,
            Invalid = 0xFFFFFFFF
        }

        public enum FIFO : uint
        {
            Min = 0,
            Max = 4,
            NextCmd = 8,
            Stop = 12
        }

        private enum FIFOCommand
        {
            Update = 1,
            RECT_FILL = 2,
            RECT_COPY = 3,
            DEFINE_BITMAP = 4,
            DEFINE_BITMAP_SCANLINE = 5,
            DEFINE_PIXMAP = 6,
            DEFINE_PIXMAP_SCANLINE = 7,
            RECT_BITMAP_FILL = 8,
            RECT_PIXMAP_FILL = 9,
            RECT_BITMAP_COPY = 10,
            RECT_PIXMAP_COPY = 11,
            FREE_OBJECT = 12,
            RECT_ROP_FILL = 13,
            RECT_ROP_COPY = 14,
            RECT_ROP_BITMAP_FILL = 15,
            RECT_ROP_PIXMAP_FILL = 16,
            RECT_ROP_BITMAP_COPY = 17,
            RECT_ROP_PIXMAP_COPY = 18,
            DEFINE_CURSOR = 19,
            DISPLAY_CURSOR = 20,
            MOVE_CURSOR = 21,
            DEFINE_ALPHA_CURSOR = 22
        }

        private enum IOPortOffset : byte
        {
            Index = 0,
            Value = 1,
            Bios = 2,
            IRQ = 3
        }

        [Flags]
        private enum Capability
        {
            None = 0,
            RectFill = 1,
            RectCopy = 2,
            RectPatFill = 4,
            LecacyOffscreen = 8,
            RasterOp = 16,
            Cursor = 32,
            CursorByPass = 64,
            CursorByPass2 = 128,
            EigthBitEmulation = 256,
            AlphaCursor = 512,
            Glyph = 1024,
            GlyphClipping = 0x00000800,
            Offscreen1 = 0x00001000,
            AlphaBlend = 0x00002000,
            ThreeD = 0x00004000,
            ExtendedFifo = 0x00008000,
            MultiMon = 0x00010000,
            PitchLock = 0x00020000,
            IrqMask = 0x00040000,
            DisplayTopology = 0x00080000,
            Gmr = 0x00100000,
            Traces = 0x00200000,
            Gmr2 = 0x00400000,
            ScreenObject2 = 0x00800000
        }

        private Cosmos.Core.IOPort IndexPort;
        private Cosmos.Core.IOPort ValuePort;
        private Cosmos.Core.IOPort BiosPort;
        private Cosmos.Core.IOPort IRQPort;

        public Cosmos.Core.MemoryBlock Video_Memory;
        private Cosmos.Core.MemoryBlock FIFO_Memory;

        private PCIDeviceNormal device;
        private ushort height;
        private ushort width;
        private uint depth;
        private uint capabilities;

        public VMWareSVGA()
        {
            device = (PCIDeviceNormal)Cosmos.Core.PCI.GetDevice(0x15AD, 0x0405);
            device.EnableMemory(true);
            uint basePort = device.BaseAddresses[0].BaseAddress();
            IndexPort = new IOPort((ushort)(basePort + (uint)IOPortOffset.Index));
            ValuePort = new IOPort((ushort)(basePort + (uint)IOPortOffset.Value));
            BiosPort = new IOPort((ushort)(basePort + (uint)IOPortOffset.Bios));
            IRQPort = new IOPort((ushort)(basePort + (uint)IOPortOffset.IRQ));

            WriteRegister(Register.ID, (uint)ID.V2);
            if (ReadRegister(Register.ID) != (uint)ID.V2)
                return;

            Video_Memory = new MemoryBlock(ReadRegister(Register.FrameBufferStart), ReadRegister(Register.VRamSize));

            capabilities = ReadRegister(Register.Capabilities);
            InitializeFIFO();
        }

        public void InitializeFIFO()
        {
            FIFO_Memory = new MemoryBlock(ReadRegister(Register.MemStart), ReadRegister(Register.MemSize));
            FIFO_Memory[(uint)FIFO.Min] = (uint)Register.FifoNumRegisters * sizeof(uint);
            FIFO_Memory[(uint)FIFO.Max] = FIFO_Memory.Size;
            FIFO_Memory[(uint)FIFO.NextCmd] = FIFO_Memory[(uint)FIFO.Min];
            FIFO_Memory[(uint)FIFO.Stop] = FIFO_Memory[(uint)FIFO.Min];
            WriteRegister(Register.ConfigDone, 1);
        }

        public void SetMode(ushort width, ushort height, ushort depth = 32)
        {
            // Depth is color depth in bytes.
            this.depth = (uint)(depth / 8);
            this.width = width;
            this.height = height;
            WriteRegister(Register.Width, width);
            WriteRegister(Register.Height, height);
            WriteRegister(Register.BitsPerPixel, depth);
            WriteRegister(Register.Enable, 1);
            InitializeFIFO();
        }

        public void WriteRegister(Register register, uint value)
        {
            IndexPort.DWord = (uint)register;
            ValuePort.DWord = value;
        }

        public uint ReadRegister(Register register)
        {
            IndexPort.DWord = (uint)register;
            return ValuePort.DWord;
        }

        public uint GetFIFO(FIFO cmd)
        {
            return FIFO_Memory[(uint)cmd];
        }

        public uint SetFIFO(FIFO cmd, uint value)
        {
            return FIFO_Memory[(uint)cmd] = value;
        }

        public void WaitForFifo()
        {
            WriteRegister(Register.Sync, 1);
            while (ReadRegister(Register.Busy) != 0) { }
        }

        public void WriteToFifo(uint value)
        {
            if (((GetFIFO(FIFO.NextCmd) == GetFIFO(FIFO.Max) - 4) && GetFIFO(FIFO.Stop) == GetFIFO(FIFO.Min)) ||
                (GetFIFO(FIFO.NextCmd) + 4 == GetFIFO(FIFO.Stop)))
                WaitForFifo();

            SetFIFO((FIFO)GetFIFO(FIFO.NextCmd), value);
            SetFIFO(FIFO.NextCmd, GetFIFO(FIFO.NextCmd) + 4);

            if (GetFIFO(FIFO.NextCmd) == GetFIFO(FIFO.Max))
                SetFIFO(FIFO.NextCmd, GetFIFO(FIFO.Min));
        }

        public void Update(ushort x, ushort y, ushort width, ushort height)
        {
            WriteToFifo((uint)FIFOCommand.Update);
            WriteToFifo(x);
            WriteToFifo(y);
            WriteToFifo(width);
            WriteToFifo(height);
            WaitForFifo();
        }

        public void SetPixel(ushort x, ushort y, uint color)
        {
            Video_Memory[(uint)((y * width + x) * depth)] = color;
        }

        public uint GetPixel(ushort x, ushort y)
        {
            return Video_Memory[(uint)((y * width + x) * depth)];
        }

        public void Clear(uint color)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    SetPixel((ushort)x, (ushort)y, color);
                }
            }
        }




    }
}
