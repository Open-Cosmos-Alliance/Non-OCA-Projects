using System;
namespace GruntyOS.IO.Devices
{
    public class keyboardDev : Device
    {
        public keyboardDev()
        {
            this.Name = "keyboard";
            this.Type = deviceType.other;
        }
        public override Stream Open(int modes = 4)
        {
            return new keyboardStream();
        }
    }
    public class keyboardStream : Stream
    {
        byte[] scanCode = new byte[4];
        int pos = 8;
        uint scancode ;
        byte[] tmp;
        public keyboardStream()
        {
            base.Register();
            
        }
        private byte toByte(bool v)
        {
            if (v)
                return 1;
            else
                return 0;
        }
        public override void writeByte(uint ptr, byte data)
        {

        }
        private unsafe void getBytes(uint u,byte[] dest)
        {
            byte* ptr = (byte*)&u;
            for (int i = 0; i < 4; i++)
                dest[i] = ptr[i];
        }
        public override int readByte(uint ptr)
        {
            if (pos > 4)
            {
                pos = 1;
                Cosmos.Hardware.Global.Keyboard.GetScancode(out scancode);
                getBytes(scancode, scanCode);
                return scanCode[0];
            }
            else
            {
                pos++;
                return scanCode[pos -1];
            }
            return 0;
        }
    }
}

