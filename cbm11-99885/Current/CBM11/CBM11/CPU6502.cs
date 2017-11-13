using System;

namespace Emu6502
{

    /// <summary>
    /// CPU 6502 Emulator
    /// Scott Hutter
    /// </summary>

	public partial class CPU
	{
		#region Structures

		public struct registerStruct
		{
			public byte a;
			public byte x;
			public byte y;
			public ushort pc; // program counter
			public byte sp; // stack pointer
			public byte sr; // status register (flags)
			public byte pcl
			{
				get {return (byte)(pc & 0xff);}
			}
			public byte pch
			{
				get {return (byte)(pc >> 8);}
			}
		};
		
		public struct srflags
		{
			public const byte c = 0;
			public const byte z = 1;
			public const byte i = 2;
			public const byte d = 3;
			public const byte b = 4;
			public const byte x = 5;
			public const byte v = 6;
			public const byte n = 7;
		}

		public enum AddressModes
		{
			RELATIVE,
            IMMEDIATE,
			ZEROPAGE,
			ZEROPAGE_X,
			ZEROPAGE_Y,
			ABSOLUTE,
			ABSOLUTE_INDEXED_X,
			ABSOLUTE_INDEXED_Y,
			INDEXED_INDIRECT,
			INDIRECT_INDEXED,
			INDIRECT_ABSOLUTE
		}

		#endregion

		#region Private Variables

		int clock = 0;
		public registerStruct registers = new registerStruct();
        public byte flags = 0;

		private Memory memory = null;
        private ConsoleIO io = null;
        private bool IRQ_MONITOR = false;

		#endregion

		#region Constants

		private const byte stackPage = 0x01;
		private const byte stackTop = 0xFF;

		#endregion

		public CPU(ref Emu6502.Memory mem)
		{
            memory = mem;
            io = memory.consoleIO;
		}

		#region Bit Manipulation Static Methods

		private static bool GetBit(byte b, byte bit)		{ return (b & (1 << bit)) != 0; } 
		private static void SetBit(ref byte b, byte bit)	{ b |= (byte)(1 << bit); } 
		private static void ResetBit(ref byte b, byte bit)	{ b &= (byte) ~(1 << bit); } 
		private static void FlipBit(ref byte b, byte bit)	{ b ^= (byte)(1 << bit); }
        private static int  Bit(int b, int bit)             { return (b >> bit) & ((1 << 1) - 1);}


        #endregion

		#region Stack Methods

		private byte pop()
		{
			registers.sp++;
			int temp = registers.sp + (stackPage * 256);
			return memory.read((ushort)temp);
		}

		private void push(byte val)
		{
			int temp = registers.sp + (stackPage * 256);
			memory.write((ushort)temp, val);
			registers.sp--;
		}

		private void push_lohi(ushort val)
		{
			push(MSB(val));
			push(LSB(val));
		}

		private ushort pop_lohi()
		{
			byte lo = pop();
			byte hi = pop();
			return (ushort)((hi*256)+lo);
		}

		#endregion

		#region Conversion Methods

		private ushort LoHiToVal(ushort memLocation)
		{
            //Converts little endian bytes to 16 bit address
            int lo = memory.read(memLocation);
            int hi = memory.read(++memLocation) * 256;

            int v = hi + lo;
            return (ushort)v;

		}

		private ushort WORD(byte lsb, byte msb)
		{
			return (ushort)(lsb | msb << 8);
		}

		private byte LSB(ushort val)
		{
			return (byte)(val&0xff);
		}

		private byte MSB(ushort val)
		{
			return (byte)(val>>8);
		}

		#endregion

		#region Addressing modes

		/// <summary>
		/// EA - Returns the Effective Address, given an address mode
		/// This method is always relative to the current program counter pointer
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="ticks"></param>
		/// <returns></returns>
		ushort EA(AddressModes mode, int ticks)
		{
			switch(mode)
			{
				case AddressModes.RELATIVE:
				{
					sbyte bo = (sbyte)memory.read(registers.pc);
					registers.pc++;
					return (ushort)(registers.pc + bo);
				}
                case AddressModes.IMMEDIATE:
                {
                    return WORD((byte)registers.pc++, 0x00);
                }
				case AddressModes.ZEROPAGE:
				{
					return WORD(memory.read(registers.pc++), 0x00);
				}
				case AddressModes.ZEROPAGE_X:
				{
					return WORD((byte)(memory.read(registers.pc++) + registers.x), 0x00);
				}
				case AddressModes.ZEROPAGE_Y:
				{
					return WORD((byte)(memory.read(registers.pc++) + registers.y), 0x00);
				}
				case AddressModes.ABSOLUTE:
				{			
					byte lsb = memory.read(registers.pc++);
					byte msb = memory.read(registers.pc++);
					return WORD(lsb, msb);
				}
				case AddressModes.ABSOLUTE_INDEXED_X:
				{			
					ushort ea = WORD(memory.read(registers.pc++), memory.read(registers.pc++));

					if (LSB(ea) + registers.x > 0xff) 
						clock += ticks;
					return (ushort)(ea + registers.x);
				}

				case AddressModes.ABSOLUTE_INDEXED_Y:
				{
					ushort ea = WORD(memory.read(registers.pc++), memory.read(registers.pc++));

					if (LSB(ea) + registers.y > 0xff) 
						clock += ticks;

					return (ushort)(ea + registers.y);
				}
				case AddressModes.INDEXED_INDIRECT:
				{
					byte zpa = (byte)(memory.read(registers.pc++) + registers.x);
					byte lsb = memory.read(zpa++);
					byte msb = memory.read(zpa);
					return WORD(lsb, msb);
				}
				case AddressModes.INDIRECT_INDEXED:
				{
					byte zpa = memory.read(registers.pc++);
					byte lsb = memory.read(zpa++);
					byte msb = memory.read(zpa);

					if (lsb + registers.y > 0xff) 
						clock += ticks;

					return (ushort)(WORD(lsb, msb) + registers.y);
				}
				case AddressModes.INDIRECT_ABSOLUTE:
				{
					ushort ea = WORD(memory.read(registers.pc++), memory.read(registers.pc++));
					byte lsb = memory.read(ea);
					ea = WORD((byte)(LSB(ea) + 1), MSB(ea));  // emulate bug
					byte msb = memory.read(ea);
					return WORD(lsb, msb);
				}

					
			}
			
			return 0;
		}


		#endregion

        private void SetNZ(byte register)
        {
            if (GetBit(register, 7) == true) SetBit(ref registers.sr, srflags.n);
            else ResetBit(ref registers.sr, srflags.n);

            if (register == 0) SetBit(ref registers.sr, srflags.z);
            else ResetBit(ref registers.sr, srflags.z);
        }

        public void start(ushort startAddress)
		{
			SetBit(ref registers.sr, srflags.x); // this bit is always set
			registers.pc = startAddress;
			if(registers.sp == 0) registers.sp = stackTop;

			this.start();
		}

        // Main CPU Entry Point
		public void start()
		{
			byte op = 0;

			//Main loop
			while(true)
			{
                op = memory.read(registers.pc);
				registers.pc++;
				
				switch(op)
                {
                    #region ADC

                    case 0x0069:
                    {
                        //Immediate  TODO: DECIMAL MODE
                        int tmp = registers.a + memory.read(registers.pc) + Bit(registers.sr, srflags.c);

                        if (Bit(registers.sr, srflags.d) == 0)
                        {

                            if (tmp > 0xff)
                                SetBit(ref registers.sr, srflags.c);
                            else
                                ResetBit(ref registers.sr, srflags.c);

                            if (Bit(registers.a, 7) != Bit(tmp, 7))
                                SetBit(ref registers.sr, srflags.v);
                            else
                                ResetBit(ref registers.sr, srflags.v);

                            registers.a = (byte)tmp;
                            SetNZ(registers.a);

                            clock += 2;
                        }
                        break;

                    }

                    case 0x0065:
                    {
                        //Zero page
                        int tmp = registers.a + memory.read(EA(AddressModes.ZEROPAGE, 0)) + Bit(registers.sr, srflags.c);

                        if (Bit(registers.sr, srflags.d) == 0)
                        {

                            if (tmp > 0xff)
                                SetBit(ref registers.sr, srflags.c);
                            else
                                ResetBit(ref registers.sr, srflags.c);

                            if (Bit(registers.a, 7) != Bit(tmp, 7))
                                SetBit(ref registers.sr, srflags.v);
                            else
                                ResetBit(ref registers.sr, srflags.v);

                            registers.a = (byte)tmp;
                            SetNZ(registers.a);

                            clock += 3;
                        }
                        break;

                    }

                    case 0x0075:
                    {
                        //Zero page, x
                        int tmp = registers.a + memory.read(EA(AddressModes.ZEROPAGE_X, 0)) + Bit(registers.sr, srflags.c);

                        if (Bit(registers.sr, srflags.d) == 0)
                        {

                            if (tmp > 0xff)
                                SetBit(ref registers.sr, srflags.c);
                            else
                                ResetBit(ref registers.sr, srflags.c);

                            if (Bit(registers.a, 7) != Bit(tmp, 7))
                                SetBit(ref registers.sr, srflags.v);
                            else
                                ResetBit(ref registers.sr, srflags.v);

                            registers.a = (byte)tmp;
                            SetNZ(registers.a);

                            clock += 3;
                        }
                        break;
                    }
                    case 0x006D:
                    {
                        //Absolute
                        int tmp = registers.a + memory.read(EA(AddressModes.ABSOLUTE, 0)) + Bit(registers.sr, srflags.c);

                        if (Bit(registers.sr, srflags.d) == 0)
                        {

                            if (tmp > 0xff)
                                SetBit(ref registers.sr, srflags.c);
                            else
                                ResetBit(ref registers.sr, srflags.c);

                            if (Bit(registers.a, 7) != Bit(tmp, 7))
                                SetBit(ref registers.sr, srflags.v);
                            else
                                ResetBit(ref registers.sr, srflags.v);

                            registers.a = (byte)tmp;
                            SetNZ(registers.a);

                            clock += 3;
                        }
                        break;
                    }
                    case 0x007D:
                    {
                        //Absolute, x
                        int tmp = registers.a + memory.read(EA(AddressModes.ABSOLUTE_INDEXED_X, 0)) + Bit(registers.sr, srflags.c);

                        if (Bit(registers.sr, srflags.d) == 0)
                        {

                            if (tmp > 0xff)
                                SetBit(ref registers.sr, srflags.c);
                            else
                                ResetBit(ref registers.sr, srflags.c);

                            if (Bit(registers.a, 7) != Bit(tmp, 7))
                                SetBit(ref registers.sr, srflags.v);
                            else
                                ResetBit(ref registers.sr, srflags.v);

                            registers.a = (byte)tmp;
                            SetNZ(registers.a);

                            clock += 3;
                        }
                        break;
                    }
                    case 0x0079:
                    {
                        //Absolute, y
                        int tmp = registers.a + memory.read(EA(AddressModes.ABSOLUTE_INDEXED_Y, 0)) + Bit(registers.sr, srflags.c);

                        if (Bit(registers.sr, srflags.d) == 0)
                        {

                            if (tmp > 0xff)
                                SetBit(ref registers.sr, srflags.c);
                            else
                                ResetBit(ref registers.sr, srflags.c);

                            if (Bit(registers.a, 7) != Bit(tmp, 7))
                                SetBit(ref registers.sr, srflags.v);
                            else
                                ResetBit(ref registers.sr, srflags.v);

                            registers.a = (byte)tmp;
                            SetNZ(registers.a);

                            clock += 3;
                        }
                        break;
                    }
                    case 0x0061:
                    {
                        //indexed indirect AND ($ob,x)
                        int tmp = registers.a + memory.read(EA(AddressModes.INDEXED_INDIRECT, 0)) + Bit(registers.sr, srflags.c);

                        if (Bit(registers.sr, srflags.d) == 0)
                        {

                            if (tmp > 0xff)
                                SetBit(ref registers.sr, srflags.c);
                            else
                                ResetBit(ref registers.sr, srflags.c);

                            if (Bit(registers.a, 7) != Bit(tmp, 7))
                                SetBit(ref registers.sr, srflags.v);
                            else
                                ResetBit(ref registers.sr, srflags.v);

                            registers.a = (byte)tmp;
                            SetNZ(registers.a);

                            clock += 3;
                        }
                        break;
                    }
                    case 0x0071:
                    {
                        //indirect indexed  AND ($ob),y
                        int tmp = registers.a + memory.read(EA(AddressModes.INDIRECT_INDEXED, 0)) + Bit(registers.sr, srflags.c);

                        if (Bit(registers.sr, srflags.d) == 0)
                        {

                            if (tmp > 0xff)
                                SetBit(ref registers.sr, srflags.c);
                            else
                                ResetBit(ref registers.sr, srflags.c);

                            if (Bit(registers.a, 7) != Bit(tmp, 7))
                                SetBit(ref registers.sr, srflags.v);
                            else
                                ResetBit(ref registers.sr, srflags.v);

                            registers.a = (byte)tmp;
                            SetNZ(registers.a);

                            clock += 3;
                        }
                        break;
                    }

                    #endregion

                    #region AND
                    case 0x0029:
					{
                        registers.a &= memory.read(EA(AddressModes.IMMEDIATE, 0));
						SetNZ(registers.a);
						clock += 2;
						break;
					}
					case 0x0025:
					{
						//Zero page
						registers.a &= memory.read(EA(AddressModes.ZEROPAGE, 0));
						SetNZ(registers.a);
						clock += 3;
						break;
					}
					case 0x0035:
					{
						//Zero page, x
						registers.a &= memory.read(EA(AddressModes.ZEROPAGE_X, 0));
						SetNZ(registers.a);
						clock += 4;
						break;
					}
					case 0x002D:
					{
						//Absolute
						registers.a &= memory.read(EA(AddressModes.ABSOLUTE, 0));
						SetNZ(registers.a);
						clock += 4;
						break;
					}
					case 0x003D:
					{
						//Absolute, x
						registers.a &= memory.read(EA(AddressModes.ABSOLUTE_INDEXED_X, 0));
						SetNZ(registers.a);
						clock += 4;
						break;
					}
					case 0x0039:
					{
						//Absolute, y
						registers.a &= memory.read(EA(AddressModes.ABSOLUTE_INDEXED_Y, 0));
						SetNZ(registers.a);
						clock += 4;
						break;
					}
					case 0x0021:
					{
						//indexed indirect AND ($ob,x)
						registers.a &= memory.read(EA(AddressModes.INDEXED_INDIRECT, 0));
						SetNZ(registers.a);
						clock += 6;
						break;
					}
					case 0x0031:
					{
						//indirect indexed  AND ($ob),y
						registers.a &= memory.read(EA(AddressModes.INDIRECT_INDEXED, 0));
						SetNZ(registers.a);
						clock += 6;
						break;
					}
					#endregion

                    #region ASL

                    case 0x000A:
                    {
                        //Accumulator
                        byte tmp = registers.a;

                        if (Bit(tmp, 7) == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        tmp = (byte)((tmp << 1) & 0x00FE);
                        registers.a = tmp;
                        SetNZ(tmp);
                        clock += 2;
                        break;
                    }

                    case 0x0006:
                    {
                        //zero page
                        ushort location = EA(AddressModes.ZEROPAGE, 0);
                        byte tmp = memory.read(location);

                        if (Bit(tmp, 7) == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        tmp = (byte)((tmp << 1) & 0x00FE);
                        memory.write(location, tmp);
                        SetNZ(tmp);
                        clock += 5;
                        break;
                    }

                    case 0x0016:
                    {
                        //zero page, x
                        ushort location = EA(AddressModes.ZEROPAGE_X, 0);
                        byte tmp = memory.read(location);

                        if (Bit(tmp, 7) == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        tmp = (byte)((tmp << 1) & 0x00FE);
                        memory.write(location, tmp);
                        SetNZ(tmp);
                        clock += 6;
                        break;
                    }

                    case 0x000E:
                    {
                        //absolute
                        ushort location = EA(AddressModes.ABSOLUTE, 0);
                        byte tmp = memory.read(location);

                        if (Bit(tmp, 7) == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        tmp = (byte)((tmp << 1) & 0x00FE);
                        memory.write(location, tmp);
                        SetNZ(tmp);
                        clock += 6;
                        break;
                    }

                    case 0x001E:
                    {
                        //absolute, x
                        ushort location = EA(AddressModes.ABSOLUTE_INDEXED_X, 0);
                        byte tmp = memory.read(location);

                        if (Bit(tmp, 7) == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        tmp = (byte)((tmp << 1) & 0x00FE);
                        memory.write(location, tmp);
                        SetNZ(tmp);
                        clock += 7;
                        break;
                    }

                    #endregion

                    #region BCC
                    case 0x0090:
					{
						if(GetBit(registers.sr, srflags.c) == false)
						{
							registers.pc = EA(AddressModes.RELATIVE, 0);
						}
						else registers.pc++;
						//TODO: clock + 1 if branch succeeds, +2 if to a new page
						clock += 2;
						break;
					}
					#endregion
					
					#region BCS
					case 0x00B0:
					{
						if(GetBit(registers.sr, srflags.c) == true)
						{
							registers.pc = EA(AddressModes.RELATIVE, 0);
						}
						else registers.pc++;
						//TODO: clock + 1 if branch succeeds, +2 if to a new page
						clock += 2;
						break;
					}
					#endregion

					#region BEQ
					case 0x00F0:
					{
						if(GetBit(registers.sr, srflags.z) == true)
						{
							registers.pc = EA(AddressModes.RELATIVE, 0);
						}
						else registers.pc++;
						//TODO: clock + 1 if branch succeeds, +2 if to a new page
						clock += 2;
						break;
					}
					#endregion
					
					#region BMI
					case 0x0030:
					{
						if(GetBit(registers.sr, srflags.n) == true)
						{
							registers.pc = EA(AddressModes.RELATIVE, 0);
						}
						else registers.pc++;
						//TODO: clock + 1 if branch succeeds, +2 if to a new page
						clock += 2;
						break;
					}
					#endregion

                    #region BIT

                    case 0x0024:
                    {
                        //Zero Page
                        byte tmp = memory.read(EA(AddressModes.ZEROPAGE, 0));

                        if (Bit(tmp, 7) == 1)
                            SetBit(ref registers.sr, srflags.n);
                        else
                            ResetBit(ref registers.sr, srflags.n);
                       
                        if (Bit(tmp, 6) == 1)
                            SetBit(ref registers.sr, srflags.v);
                        else
                            ResetBit(ref registers.sr, srflags.v);
                          
                        if((registers.a & tmp) == 0)
                            SetBit(ref registers.sr, srflags.z);
                        else
                            ResetBit(ref registers.sr, srflags.z);

                        clock += 3;

                        break;
                    }

                    case 0x002C:
                    {
                        //Absolute
                        byte tmp = memory.read(EA(AddressModes.ABSOLUTE, 0));

                        if (Bit(tmp, 7) == 1)
                            SetBit(ref registers.sr, srflags.n);
                        else
                            ResetBit(ref registers.sr, srflags.n);

                        if (Bit(tmp, 6) == 1)
                            SetBit(ref registers.sr, srflags.v);
                        else
                            ResetBit(ref registers.sr, srflags.v);

                        if ((registers.a & tmp) == 0)
                            SetBit(ref registers.sr, srflags.z);
                        else
                            ResetBit(ref registers.sr, srflags.z);

                        clock += 4;

                        break;
                    }

                    #endregion

                    #region BNE
                    case 0x00D0:
					{
						if(GetBit(registers.sr, srflags.z) == false)
						{
							registers.pc = EA(AddressModes.RELATIVE, 0);
						}
						else registers.pc++;
						//TODO: clock + 1 if branch succeeds, +2 if to a new page
						clock += 2;
						break;
					}
					#endregion

					#region BPL
					case 0x0010:
					{
						if(GetBit(registers.sr, srflags.n) == false)
						{
							registers.pc = EA(AddressModes.RELATIVE, 0);
						}
						else registers.pc++;
						//TODO: clock + 1 if branch succeeds, +2 if to a new page
						clock += 2;
						break;
					}
					#endregion

					#region BRK
					case 0x0000:
					{
                        //push_lohi(registers.pc);
						SetBit(ref registers.sr, srflags.b);
						//push(registers.sr);
                        //registers.pc = LoHiToVal(0xFFFE);
                        //SetBit(ref registers.sr, srflags.i);

                        GoMonitor();

                        break;
					}
					#endregion	
						
					#region BVC
					case 0x0050:
					{
						if(GetBit(registers.sr, srflags.v) == false)
						{
							registers.pc = EA(AddressModes.RELATIVE, 0);
						}
						else registers.pc++;
						//TODO: clock + 1 if branch succeeds, +2 if to a new page
						clock += 2;
						break;
					}
					#endregion
					
					#region BVS
					case 0x0070:
					{
						if(GetBit(registers.sr, srflags.v) == true)
						{
							registers.pc = EA(AddressModes.RELATIVE, 0);
						}
						else registers.pc++;
						//TODO: clock + 1 if branch succeeds, +2 if to a new page
						clock += 2;
						break;
					}
					#endregion

					#region CLC
					case 0x0018:
					{
						ResetBit(ref registers.sr, srflags.c);
						clock += 2;
						break;
					}
					#endregion

					#region CLD
					case 0x00D8:
					{
						ResetBit(ref registers.sr, srflags.d);
						clock += 2;
						break;
					}
					#endregion

					#region CLI
					case 0x0058:
					{
						ResetBit(ref registers.sr, srflags.i);
						clock += 2;
						break;
					}
					#endregion

					#region CLV
					case 0x00B8:
					{
						ResetBit(ref registers.sr, srflags.v);
						clock += 2;
						break;
					}
					#endregion

					#region CMP
					case 0x00C9:
					{
						//Immediate
                        byte temp = memory.read(EA(AddressModes.IMMEDIATE, 0));
						if(registers.a >= temp) SetBit(ref registers.sr, srflags.c);
						if(registers.a == temp) SetBit(ref registers.sr, srflags.z);
						if(GetBit(registers.a, 7) == true) SetBit(ref registers.sr, srflags.n);
						clock += 2;
						break;
					}
					case 0x00C5:
					{
						//Zero Page
						byte temp = memory.read(EA(AddressModes.ZEROPAGE, 0));
						if(registers.a >= temp) SetBit(ref registers.sr, srflags.c);
						if(registers.a == temp) SetBit(ref registers.sr, srflags.z);
						if(GetBit(registers.a, 7) == true) SetBit(ref registers.sr, srflags.n);
						clock += 3;
						break;
					}
					case 0x00D5:
					{
						//Zero Page, x
						byte temp = memory.read(EA(AddressModes.ZEROPAGE_X, 0));
						if(registers.a >= temp) SetBit(ref registers.sr, srflags.c);
						if(registers.a == temp) SetBit(ref registers.sr, srflags.z);
						if(GetBit(registers.a, 7) == true) SetBit(ref registers.sr, srflags.n);
						clock += 4;
						break;
					}
					case 0x00CD:
					{
						//Absolute
						byte temp = memory.read(EA(AddressModes.ABSOLUTE, 0));
						if(registers.a >= temp) SetBit(ref registers.sr, srflags.c);
						if(registers.a == temp) SetBit(ref registers.sr, srflags.z);
						if(GetBit(registers.a, 7) == true) SetBit(ref registers.sr, srflags.n);
						clock += 4;
						break;
					}
					case 0x00DD:
					{
						//Absolute,x 
						byte temp = memory.read(EA(AddressModes.ABSOLUTE_INDEXED_X, 0));
						if(registers.a >= temp) SetBit(ref registers.sr, srflags.c);
						if(registers.a == temp) SetBit(ref registers.sr, srflags.z);
						if(GetBit(registers.a, 7) == true) SetBit(ref registers.sr, srflags.n);
						clock += 4;
						break;
					}
					case 0x00D9:
					{
						//Absolute, y
						byte temp = memory.read(EA(AddressModes.ABSOLUTE_INDEXED_Y, 0));
						if(registers.a >= temp) SetBit(ref registers.sr, srflags.c);
						if(registers.a == temp) SetBit(ref registers.sr, srflags.z);
						if(GetBit(registers.a, 7) == true) SetBit(ref registers.sr, srflags.n);
						clock += 4;
						break;
					}
					case 0x00C1:
					{
						//(Indirect, x)
						byte temp = memory.read(EA(AddressModes.INDEXED_INDIRECT, 0));
						if(registers.a >= temp) SetBit(ref registers.sr, srflags.c);
						if(registers.a == temp) SetBit(ref registers.sr, srflags.z);
						if(GetBit(registers.a, 7) == true) SetBit(ref registers.sr, srflags.n);
						clock += 6;
						break;
					}
					case 0x00D1:
					{
						//(Indirect), y
						byte temp = memory.read(EA(AddressModes.INDIRECT_INDEXED, 0));
						if(registers.a >= temp) SetBit(ref registers.sr, srflags.c);
						if(registers.a == temp) SetBit(ref registers.sr, srflags.z);
						if(GetBit(registers.a, 7) == true) SetBit(ref registers.sr, srflags.n);
						clock += 5;
						break;
					}
					#endregion

					#region CPX
					case 0x00E0:
					{
						//Immediate
                        byte temp = memory.read(EA(AddressModes.IMMEDIATE, 0));
						if(registers.x >= temp) SetBit(ref registers.sr, srflags.c);
						if(registers.x == temp) SetBit(ref registers.sr, srflags.z);
						if(GetBit(registers.x, 7) == true) SetBit(ref registers.sr, srflags.n);
						clock += 2;
						break;
					}
					case 0x00E4:
					{
						//Zero Page
						byte temp = memory.read(EA(AddressModes.ZEROPAGE, 0));
						if(registers.x >= temp) SetBit(ref registers.sr, srflags.c);
						if(registers.x == temp) SetBit(ref registers.sr, srflags.z);
						if(GetBit(registers.x, 7) == true) SetBit(ref registers.sr, srflags.n);
						clock += 3;
						break;
					}
					case 0x00EC:
					{
						//Absolute
						byte temp = memory.read(EA(AddressModes.ABSOLUTE, 0));
						if(registers.x >= temp) SetBit(ref registers.sr, srflags.c);
						if(registers.x == temp) SetBit(ref registers.sr, srflags.z);
						if(GetBit(registers.x, 7) == true) SetBit(ref registers.sr, srflags.n);
						clock += 4;
						break;
					}
					#endregion

					#region CPY

					case 0x00C0:
					{
						//Immediate
						byte temp = memory.read(EA(AddressModes.IMMEDIATE, 0));
						if(registers.y >= temp) SetBit(ref registers.sr, srflags.c);
						if(registers.y == temp) SetBit(ref registers.sr, srflags.z);
						if(GetBit(registers.y, 7) == true) SetBit(ref registers.sr, srflags.n);
						clock += 2;
						break;
					}
					case 0x00C4:
					{
						//Zero Page
						byte temp = memory.read(EA(AddressModes.ZEROPAGE, 0));
						if(registers.y >= temp) SetBit(ref registers.sr, srflags.c);
						if(registers.y == temp) SetBit(ref registers.sr, srflags.z);
						if(GetBit(registers.y, 7) == true) SetBit(ref registers.sr, srflags.n);
						clock += 3;
						break;
					}
					case 0x00CC:
					{
						//Absolute
						byte temp = memory.read(EA(AddressModes.ABSOLUTE, 0));
						if(registers.y >= temp) SetBit(ref registers.sr, srflags.c);
						if(registers.y == temp) SetBit(ref registers.sr, srflags.z);
						if(GetBit(registers.y, 7) == true) SetBit(ref registers.sr, srflags.n);
						clock += 4;
						break;
					}
					#endregion

					#region DEC
					case 0x00C6:
					{
						//zero page
						ushort location = memory.read(registers.pc);
                        byte temp = (byte)(memory.read(EA(AddressModes.ZEROPAGE, 0)) - 1);
						memory.write(location, temp);
						SetNZ(temp);
						clock+=5;
						break;
					}
					case 0x00D6:
					{
						//zero page, x
                        ushort location = (ushort)(memory.read(registers.pc) + registers.x);
                        byte temp = (byte)(memory.read(EA(AddressModes.ZEROPAGE_X, 0)) - 1);
						memory.write(location, temp);
						SetNZ(temp);
						clock+=6;
						break;
					}
                    case 0x00CE:
                    {
                        //absolute 
                        ushort location = LoHiToVal(registers.pc);
                        byte temp = (byte)(memory.read(EA(AddressModes.ABSOLUTE, 0)) - 1);
                        memory.write(location, temp);
                        SetNZ(temp);
                        clock += 5;
                        break;
                    }
					case 0x00DE:
					{
                        //absolute,x
                        ushort location = (ushort)(LoHiToVal(registers.pc) + registers.x);
                        byte temp = (byte)(memory.read(EA(AddressModes.ABSOLUTE_INDEXED_X, 0)) - 1);
                        memory.write(location, temp);
                        SetNZ(temp);
                        clock += 5;
                        break;
					}
					#endregion

					#region DEX
					case 0x00CA:
					{
						registers.x--;
						SetNZ(registers.x);
						clock += 2;
						break;
					}
					#endregion	

					#region DEY
					case 0x0088:
					{
						registers.y--;
						SetNZ(registers.y);
						clock += 2;
						break;
					}
					#endregion	

                    #region EOR

                    case 0x0049:
                    {
                        //Immediate
                        registers.a ^= memory.read(registers.pc);
                        SetNZ(registers.a);
                        registers.pc += 2;
                        clock += 2;
                        break;
                    }
                    case 0x0045:
                    {
                        //Zero page
                        registers.a ^= memory.read(EA(AddressModes.ZEROPAGE, 0));
                        SetNZ(registers.a);
                        clock += 3;
                        break;
                    }
                    case 0x0055:
                    {
                        //Zero page, x
                        registers.a ^= memory.read(EA(AddressModes.ZEROPAGE_X, 0));
                        SetNZ(registers.a);
                        clock += 4;
                        break;
                    }
                    case 0x004D:
                    {
                        //Absolute
                        registers.a ^= memory.read(EA(AddressModes.ABSOLUTE, 0));
                        SetNZ(registers.a);
                        clock += 4;
                        break;
                    }
                    case 0x005D:
                    {
                        //Absolute, x
                        registers.a ^= memory.read(EA(AddressModes.ABSOLUTE_INDEXED_X, 0));
                        SetNZ(registers.a);
                        clock += 4;
                        break;
                    }
                    case 0x0059:
                    {
                        //Absolute, y
                        registers.a ^= memory.read(EA(AddressModes.ABSOLUTE_INDEXED_Y, 0));
                        SetNZ(registers.a);
                        clock += 4;
                        break;
                    }
                    case 0x0041:
                    {
                        //indexed indirect AND ($ob,x)
                        registers.a ^= memory.read(EA(AddressModes.INDEXED_INDIRECT, 0));
                        SetNZ(registers.a);
                        clock += 6;
                        break;
                    }
                    case 0x0051:
                    {
                        //indirect indexed  AND ($ob),y
                        registers.a ^= memory.read(EA(AddressModes.INDIRECT_INDEXED, 0));
                        SetNZ(registers.a);
                        clock += 6;
                        break;
                    }

                    #endregion

                    #region INC
                    case 0x00E6:
					{
						//zero page
                        ushort location = EA(AddressModes.ZEROPAGE, 0);
						byte temp = (byte)(memory.read(location) + 1);
						memory.write(location, temp);
						SetNZ(temp);
						registers.pc+=2;
						clock+=5;
						break;
					}
					case 0x00F6:
					{
						//zero page, x
                        ushort location = EA(AddressModes.ZEROPAGE_X, 0); 
						byte temp = (byte)(memory.read(location) + 1);
						memory.write(location, temp);
						SetNZ(temp);
						clock+=6;
						break;
					}
                    case 0x00EE:
                    {
                        //absolute
                        ushort location = EA(AddressModes.ABSOLUTE, 0);
                        byte temp = (byte)(memory.read(location) + 1);
                        memory.write(location, temp);
                        SetNZ(temp);
                        clock += 6;
                        break;
                    }
					case 0x00FE:
					{
                        //absolute, x
                        ushort location = EA(AddressModes.ABSOLUTE_INDEXED_X, 0);
                        byte temp = (byte)(memory.read(location) + 1);
                        memory.write(location, temp);
                        SetNZ(temp);
                        clock += 7;
						break;
					}
					#endregion

					#region INX
					case 0x00E8:
					{
						registers.x++;
						SetNZ(registers.x);
						clock += 2;
						break;
					}
					#endregion

					#region INY
					case 0x00C8:
					{
						registers.y++;
						SetNZ(registers.y);
						clock += 2;
						break;
					}
					#endregion

					#region JMP
					case 0x004C:
					{
						//absolute
                        registers.pc = EA(AddressModes.ABSOLUTE, 0);
						clock += 3;
						break;
					}
                    case 0x006C:
                    {
                        // Indirect
                        registers.pc = EA(AddressModes.INDIRECT_ABSOLUTE, 0);
                        clock += 5;
                        break;
                    }
					#endregion

					#region JSR
					case 0x0020:
					{
                        ushort location = EA(AddressModes.ABSOLUTE, 0);
						push_lohi(--registers.pc); //push address of next op - 1 (see rts)
						registers.pc = location;
						clock += 6;
						break;
					}
					#endregion

					#region LDA
					case 0x00A9:
					{
						//Immediate
                        registers.a = memory.read(EA(AddressModes.IMMEDIATE, 0));
						SetNZ(registers.a);
						clock += 2;
						break;
					}
					case 0x00A5:
					{
						//Zero page
						registers.a = memory.read(EA(AddressModes.ZEROPAGE, 0));
						SetNZ(registers.a);
						clock += 3;
						break;
					}
					case 0x00B5:
					{
						//Zero page, x
						registers.a = memory.read(EA(AddressModes.ZEROPAGE_X, 0));
						SetNZ(registers.a);
						clock += 4;
						break;
					}
					case 0x00AD:
					{
						//Absolute
						registers.a = memory.read(EA(AddressModes.ABSOLUTE, 0));
						SetNZ(registers.a);
						clock += 4;
						break;
					}
					case 0x00BD:
					{
						//Absolute, x
						registers.a = memory.read(EA(AddressModes.ABSOLUTE_INDEXED_X, 0));
						SetNZ(registers.a);
						clock += 4;
						break;
					}
					case 0x00B9:
					{
						//Absolute, y
						registers.a = memory.read(EA(AddressModes.ABSOLUTE_INDEXED_Y, 0));
						SetNZ(registers.a);
						clock += 4;
						break;
					}
					case 0x00A1:
					{
						//indexed indirect AND ($ob,x)
						registers.a = memory.read(EA(AddressModes.INDEXED_INDIRECT, 0));
						SetNZ(registers.a);
						clock += 6;
						break;
					}
					case 0x00B1:
					{
						//indirect indexed  AND ($ob),y
						registers.a = memory.read(EA(AddressModes.INDIRECT_INDEXED, 0));
						SetNZ(registers.a);
						clock += 5;
						break;
					}

					#endregion

					#region LDX

					case 0x00A2:
					{
						//Immediate
						registers.x = memory.read(registers.pc);
						SetNZ(registers.x);
						registers.pc++;
						clock += 2;
						break;
					}
					case 0x00A6:
					{
						//Zero page
						registers.x = memory.read(EA(AddressModes.ZEROPAGE, 0));
						SetNZ(registers.x);
						clock += 3;
						break;
					}
					case 0x00B6:
					{
						//Zero page, y
						registers.x = memory.read(EA(AddressModes.ZEROPAGE_Y, 0));
						SetNZ(registers.x);
						clock += 4;
						break;
					}
					case 0x00AE:
					{
						//Absolute
						registers.x = memory.read(EA(AddressModes.ABSOLUTE, 0));
						SetNZ(registers.x);
						clock += 4;
						break;
					}
					case 0x00BE:
					{
						//Absolute, y
						registers.x = memory.read(EA(AddressModes.ABSOLUTE_INDEXED_Y, 0));
						SetNZ(registers.x);
						clock += 4;
						break;
					}
					

					#endregion

					#region LDY

					case 0x00A0:
					{
						//Immediate
						registers.y = memory.read(registers.pc);
						SetNZ(registers.y);
						registers.pc++;
						clock += 2;
						break;
					}
					case 0x00A4:
					{
						//Zero page
						registers.y = memory.read(EA(AddressModes.ZEROPAGE, 0));
						SetNZ(registers.y);
						clock += 3;
						break;
					}
					case 0x00B4:
					{
						//Zero page, x
						registers.y = memory.read(EA(AddressModes.ZEROPAGE_X, 0));
						SetNZ(registers.y);
						clock += 4;
						break;
					}
					case 0x00AC:
					{
						//Absolute
						registers.y = memory.read(EA(AddressModes.ABSOLUTE, 0));
						SetNZ(registers.y);
						clock += 4;
						break;
					}
					case 0x00BC:
					{
						//Absolute, x
						registers.y = memory.read(EA(AddressModes.ABSOLUTE_INDEXED_X, 0));
						SetNZ(registers.y);
						clock += 4;
						break;
					}

					#endregion

                    #region LSR

                    case 0x004A:
                    {
                        //Accumulator
                        byte tmp = registers.a;

                        ResetBit(ref registers.sr, srflags.n);

                        if (Bit(tmp, 0) == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        tmp = (byte)((tmp >> 1) & 0x007F);
                        registers.a = tmp;
                        SetNZ(tmp);
                        clock += 2;
                        break;
                    }

                    case 0x0046:
                    {
                        //zero page
                        ushort location = EA(AddressModes.ZEROPAGE, 0);
                        byte tmp = memory.read(location);

                        ResetBit(ref registers.sr, srflags.n);

                        if (Bit(tmp, 0) == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        tmp = (byte)((tmp >> 1) & 0x007F);
                        memory.write(location, tmp);
                        SetNZ(tmp);
                        clock += 5;
                        break;
                    }

                    case 0x0056:
                    {
                        //zero page, x
                        ushort location = EA(AddressModes.ZEROPAGE_X, 0);
                        byte tmp = memory.read(location);

                        ResetBit(ref registers.sr, srflags.n);

                        if (Bit(tmp, 0) == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        tmp = (byte)((tmp >> 1) & 0x007F);
                        memory.write(location, tmp);
                        SetNZ(tmp);
                        clock += 6;
                        break;
                    }

                    case 0x004E:
                    {
                        //absolute
                        ushort location = EA(AddressModes.ABSOLUTE, 0);
                        byte tmp = memory.read(location);

                        ResetBit(ref registers.sr, srflags.n);

                        if (Bit(tmp, 0) == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        tmp = (byte)((tmp >> 1) & 0x007F);
                        memory.write(location, tmp);
                        SetNZ(tmp);
                        clock += 6;
                        break;
                    }

                    case 0x005E:
                    {
                        //absolute, x
                        ushort location = EA(AddressModes.ABSOLUTE_INDEXED_X, 0);
                        byte tmp = memory.read(location);

                        ResetBit(ref registers.sr, srflags.n);

                        if (Bit(tmp, 0) == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        tmp = (byte)((tmp >> 1) & 0x007F);
                        memory.write(location, tmp);
                        SetNZ(tmp);
                        clock += 7;
                        break;
                    }

                    #endregion

                    #region NOP
                    case 0x00EA:
					{
						clock += 2;
						break;
					}
					#endregion

                    #region ORA

                    case 0x0009:
                    {
                        //Immediate
                        registers.a |= memory.read(EA(AddressModes.IMMEDIATE, 0));
                        SetNZ(registers.a);
                        clock += 2;
                        break;
                    }
                    case 0x0005:
                    {
                        //Zero page
                        registers.a |= memory.read(EA(AddressModes.ZEROPAGE, 0));
                        SetNZ(registers.a);
                        clock += 3;
                        break;
                    }
                    case 0x0015:
                    {
                        //Zero page, x
                        registers.a |= memory.read(EA(AddressModes.ZEROPAGE_X, 0));
                        SetNZ(registers.a);
                        clock += 4;
                        break;
                    }
                    case 0x000D:
                    {
                        //Absolute
                        registers.a |= memory.read(EA(AddressModes.ABSOLUTE, 0));
                        SetNZ(registers.a);
                        clock += 4;
                        break;
                    }
                    case 0x001D:
                    {
                        //Absolute, x
                        registers.a |= memory.read(EA(AddressModes.ABSOLUTE_INDEXED_X, 0));
                        SetNZ(registers.a);
                        clock += 4;
                        break;
                    }
                    case 0x0019:
                    {
                        //Absolute, y
                        registers.a |= memory.read(EA(AddressModes.ABSOLUTE_INDEXED_Y, 0));
                        SetNZ(registers.a);
                        clock += 4;
                        break;
                    }
                    case 0x0001:
                    {
                        //indexed indirect  ($ob,x)
                        registers.a |= memory.read(EA(AddressModes.INDEXED_INDIRECT, 0));
                        SetNZ(registers.a);
                        clock += 6;
                        break;
                    }
                    case 0x0011:
                    {
                        //indirect indexed   ($ob),y
                        registers.a |= memory.read(EA(AddressModes.INDIRECT_INDEXED, 0));
                        SetNZ(registers.a);
                        clock += 5;
                        break;
                    }

                    #endregion

                    #region PHA
                    case 0x0048:
					{
						push(registers.a);
						clock += 3;
						break;
					}
					#endregion

					#region PHP
					case 0x0008:
					{
						push(registers.sr);
						clock += 3;
						break;
					}
					#endregion

					#region PLA
					case 0x0068:
					{
						registers.a = pop();
						SetNZ(registers.a);
						clock += 4;
						break;
					}
					#endregion

					#region PLP
					case 0x0028:
					{
						registers.sr = pop();
						clock += 4;
						break;
					}
					#endregion

                    #region ROL

                    case 0x002A:
                    {
                        //Accumulator
                        int tmp = registers.a;
                        int tmp2 = Bit(tmp, 7);

                        tmp = (byte)((tmp << 1) & 0x00FE);
                        tmp = tmp | ((GetBit(registers.sr, srflags.c) == true) ? 1:0);

                        if(tmp2 == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);
                        
                        registers.a = (byte)tmp;
                        SetNZ(registers.a);
                        clock += 2;
                        break;
                    }

                    case 0x0026:
                    {
                        //zero page
                        ushort location = EA(AddressModes.ZEROPAGE, 0);
                        int tmp = memory.read(location);
                        int tmp2 = Bit(tmp, 7);

                        tmp = (byte)((tmp << 1) & 0x00FE);
                        tmp = tmp | ((GetBit(registers.sr, srflags.c) == true) ? 1 : 0);

                        if (tmp2 == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        memory.write(location, (byte)tmp);
                        SetNZ((byte)tmp);
                        clock += 5;
                        break;
                    }

                    case 0x0036:
                    {
                        //zero page, x
                        ushort location = EA(AddressModes.ZEROPAGE_X, 0);
                        int tmp = memory.read(location);
                        int tmp2 = Bit(tmp, 7);

                        tmp = (byte)((tmp << 1) & 0x00FE);
                        tmp = tmp | ((GetBit(registers.sr, srflags.c) == true) ? 1 : 0);

                        if (tmp2 == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        memory.write(location, (byte)tmp);
                        SetNZ((byte)tmp);
                        clock += 6;
                        break;
                    }

                    case 0x002E:
                    {
                        //absolute
                        ushort location = EA(AddressModes.ABSOLUTE, 0);
                        int tmp = memory.read(location);
                        int tmp2 = Bit(tmp, 7);

                        tmp = (byte)((tmp << 1) & 0x00FE);
                        tmp = tmp | ((GetBit(registers.sr, srflags.c) == true) ? 1 : 0);

                        if (tmp2 == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        memory.write(location, (byte)tmp);
                        SetNZ((byte)tmp);
                        clock += 6;
                        break;
                    }

                    case 0x003E:
                    {
                        //absolute, x
                        ushort location = EA(AddressModes.ABSOLUTE_INDEXED_X, 0);
                        int tmp = memory.read(location);
                        int tmp2 = Bit(tmp, 7);

                        tmp = (byte)((tmp << 1) & 0x00FE);
                        tmp = tmp | ((GetBit(registers.sr, srflags.c) == true) ? 1 : 0);

                        if (tmp2 == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        memory.write(location, (byte)tmp);
                        SetNZ((byte)tmp);
                        clock += 7;
                        break;
                    }

                    #endregion

                    #region ROR

                    case 0x006A:
                    {
                        //Accumulator
                        int tmp = registers.a;
                        int tmp2 = Bit(tmp, 0);

                        tmp = (byte)((tmp >> 1) & 0x007F);
                        tmp = tmp | ((GetBit(registers.sr, srflags.c) == true) ? 128 : 0);

                        if (tmp2 == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        registers.a = (byte)tmp;
                        SetNZ(registers.a);
                        clock += 2;
                        break;
                    }

                    case 0x0066:
                    {
                        //zero page
                        ushort location = EA(AddressModes.ZEROPAGE, 0);
                        int tmp = memory.read(location);
                        int tmp2 = Bit(tmp, 0);

                        tmp = (byte)((tmp >> 1) & 0x007F);
                        tmp = tmp | ((GetBit(registers.sr, srflags.c) == true) ? 128 : 0);

                        if (tmp2 == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        memory.write(location, (byte)tmp);
                        SetNZ((byte)tmp);
                        clock += 5;
                        break;
                    }

                    case 0x0076:
                    {
                        //zero page, x
                        ushort location = EA(AddressModes.ZEROPAGE_X, 0);
                        int tmp = memory.read(location);
                        int tmp2 = Bit(tmp, 0);

                        tmp = (byte)((tmp >> 1) & 0x007F);
                        tmp = tmp | ((GetBit(registers.sr, srflags.c) == true) ? 128 : 0);

                        if (tmp2 == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        memory.write(location, (byte)tmp);
                        SetNZ((byte)tmp);
                        clock += 6;
                        break;
                    }

                    case 0x006E:
                    {
                        //absolute
                        ushort location = EA(AddressModes.ABSOLUTE, 0);
                        int tmp = memory.read(location);
                        int tmp2 = Bit(tmp, 0);

                        tmp = (byte)((tmp >> 1) & 0x007F);
                        tmp = tmp | ((GetBit(registers.sr, srflags.c) == true) ? 128 : 0);

                        if (tmp2 == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        memory.write(location, (byte)tmp);
                        SetNZ((byte)tmp);
                        clock += 6;
                        break;
                    }

                    case 0x007E:
                    {
                        //absolute, x
                        ushort location = EA(AddressModes.ABSOLUTE_INDEXED_X, 0);
                        int tmp = memory.read(location);
                        int tmp2 = Bit(tmp, 0);

                        tmp = (byte)((tmp >> 1) & 0x007F);
                        tmp = tmp | ((GetBit(registers.sr, srflags.c) == true) ? 128 : 0);

                        if (tmp2 == 1)
                            SetBit(ref registers.sr, srflags.c);
                        else
                            ResetBit(ref registers.sr, srflags.c);

                        memory.write(location, (byte)tmp);
                        SetNZ((byte)tmp);
                        clock += 7;
                        break;
                    }

                    #endregion

                    #region RTI
                    case 0x0040:
					{
						registers.sr = pop();
						registers.pc = pop_lohi();
						clock += 6;
						break;
					}
					#endregion

					#region RTS
					case 0x0060:
					{
						registers.pc = (ushort)(pop_lohi() + 1);
						clock += 6;
						break;
					}
					#endregion

                    #region SBC

                    //todo

                    #endregion

                    #region SEC
                    case 0x0038:
					{
						SetBit(ref registers.sr, srflags.c);
						clock += 2;
						break;
					}
					#endregion

					#region SED
					case 0x00F8:
					{
						SetBit(ref registers.sr, srflags.d);
						clock += 2;
						break;
					}
					#endregion

					#region SEI
					case 0x0078:
					{
						SetBit(ref registers.sr, srflags.i);
						clock += 2;
						break;
					}
					#endregion

					#region STA
					case 0x0085:
					{
                        //Zero page
                        ushort location = EA(AddressModes.ZEROPAGE, 0);
                        memory.write(location, registers.a);
                        clock += 3;
                        break;
					}
					case 0x0095:
					{
						//Zero page, x
                        ushort location = EA(AddressModes.ZEROPAGE_X, 0);
						memory.write(location, registers.a);
						clock += 4;
						break;
					}
					case 0x008D:
					{
						//Absolute
                        ushort location = EA(AddressModes.ABSOLUTE, 0);
						memory.write(location, registers.a);
						clock += 4;
						break;
					}
                    case 0x009D:
                    {
                        //Absolute, x
                        ushort location = EA(AddressModes.ABSOLUTE_INDEXED_X, 0);
                        memory.write(location, registers.a);
                        SetNZ(registers.a);
                        clock += 5;
                        break;
                    }
                    case 0x0099:
                    {
                        //Absolute, y
                        ushort location = EA(AddressModes.ABSOLUTE_INDEXED_Y, 0);
                        memory.write(location, registers.a);
                        SetNZ(registers.a);
                        clock += 5;
                        break;
                    }
                    case 0x0081:
                    {
                        //indexed indirect ($ob,x)
                        ushort location = EA(AddressModes.INDEXED_INDIRECT, 0);
                        memory.write(location, registers.a);
                        SetNZ(registers.a);
                        clock += 6;
                        break;
                    }
                    case 0x0091:
                    {
                        //indirect indexed  ($ob),y
                        ushort location = EA(AddressModes.INDIRECT_INDEXED, 0);
                        memory.write(location, registers.a);
                        SetNZ(registers.a);
                        clock += 6;
                        break;
                    }
					#endregion

					#region STX
					case 0x0086:
					{
						//Zero page
                        ushort location = EA(AddressModes.ZEROPAGE, 0);
						memory.write(location, registers.x);
						registers.pc += 2;
						clock += 3;
						break;
					}
					case 0x0096:
					{
						//Zero page, y
                        ushort location = EA(AddressModes.ZEROPAGE_Y, 0);
						memory.write(location, registers.x);
						registers.pc += 2;
						clock += 4;
						break;
					}
					case 0x008E:
					{
						//Absolute
                        ushort location = EA(AddressModes.ABSOLUTE, 0);
						memory.write(location, registers.x);
						registers.pc += 2;
						clock += 4;
						break;
					}
					#endregion

					#region STY
					case 0x0084:
					{
						//Zero page
                        ushort location = EA(AddressModes.ZEROPAGE, 0);
						memory.write(location, registers.y);
						clock += 3;
						break;
					}
					case 0x0094:
					{
						//Zero page, x
                        ushort location = EA(AddressModes.ZEROPAGE_X, 0);
						memory.write(location, registers.y);
						clock += 4;
						break;
					}
					case 0x008C:
					{
						//Absolute
                        ushort location = EA(AddressModes.ABSOLUTE, 0);
						memory.write(location, registers.y);
						clock += 4;
						break;
					}
					#endregion

					#region TAX
					case 0x00AA:
					{
						registers.x = registers.a;
						SetNZ(registers.x);
						clock += 2;
						break;
					}
					#endregion

					#region TAY
					case 0x00A8:
					{
						registers.y = registers.a;
						SetNZ(registers.y);
						clock += 2;
						break;
					}
					#endregion

					#region TSX
					case 0x00BA:
					{
						registers.x = registers.sp;
						SetNZ(registers.x);
						clock += 2;
						break;
					}
					#endregion

					#region TXA
					case 0x008A:
					{
						registers.a = registers.x;
						SetNZ(registers.a);
						clock += 2;
						break;
					}
					#endregion

					#region TXS
					case 0x009A:
					{
						registers.sp = registers.x;
						clock += 2;
						break;
					}
					#endregion

					#region TYA
					case 0x0098:
					{
						registers.a = registers.y;
						SetNZ(registers.a);
						clock += 2;
						break;
					}
					#endregion

                    #region JAM
                    //For the sake of unit testing, we have to 
                    //use a HALT /JAM opcode to stop the simulation
                    case 0x002:
                    {
                        return;
                    }

                    #endregion

                }

                if (IRQ_MONITOR)
                    GoMonitor();
			}

		}

    }
}
