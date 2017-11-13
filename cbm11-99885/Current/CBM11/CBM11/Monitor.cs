using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emu6502
{
    public partial class CPU
    {
        ushort nextLocation = 0;
        bool asmMode = false;

        private void GoMonitor()
        {
            bool BREAK = true;

            io.PrintLine("BREAK");

            DisplayRegisters();

            while (BREAK == true)
            {
                string cmd = "";

                if (asmMode == false)
                {
                    io.Print(">");
                    cmd = io.ReadLine(true);
                }
                else
                {
                    io.Print(">A " + WordToHex(nextLocation) + " ");
                    cmd = io.ReadLine(true);
                    cmd = "A " + WordToHex(nextLocation) + " " + cmd;
                }

                cmd = UCase(cmd);

                if (cmd != "")
                {
                    string[] parms = cmd.Split(' ');

                    switch (parms[0])
                    {
                        case "M":
                            if (parms.Length == 1) MemCmd(ref registers, "");
                            if (parms.Length == 2) MemCmd(ref registers, parms[1]);
                            if (parms.Length == 3) PokCmd(ref registers, parms[1], parms[2]);
                            asmMode = false;
                            break;
                        case "F":
                            if (parms.Length == 4) FilCmd(ref registers, parms[1], parms[2], parms[3]);
                            asmMode = false;
                            break;
                        case "A":
                            if (parms.Length == 3) AsmCmd(ref registers, parms[1], parms[2], "");
                            if (parms.Length == 4) AsmCmd(ref registers, parms[1], parms[2], parms[3]);
                            break;
                        case "G":
                            IRQ_MONITOR = false;
                            BREAK = false;
                            if (parms.Length > 1)
                                GoCmd(ref registers, parms[1]);
                            else
                                GoCmd(ref registers, "");
                            asmMode = false;
                            break;
                        case "S":
                            IRQ_MONITOR = true;
                            BREAK = false;
                            if (parms.Length > 1)
                                GoCmd(ref registers, parms[1]);
                            else
                                GoCmd(ref registers, "");
                            asmMode = false;
                            break;
                        case "R":
                            DisplayRegisters();
                            asmMode = false;
                            break;
                        /*default:
                            InputErr();
                            break;*/
                    }
                }
            }


        }

        private void GoCmd(ref Emu6502.CPU.registerStruct registers, string parm)
        {
            ushort location = 0;

            location = (parm == "" ? registers.pc : HexToInt(parm));

            registers.pc = location;
        }

        private void MemCmd(ref Emu6502.CPU.registerStruct registers, string parm)
        {
            ushort location = 0;

            location = (parm == "" ? registers.pc : HexToInt(parm));

            for (int x = 0; x < 8; x++)
            {
                io.Print(">" + WordToHex(location) + " ");

                for (int y = 0; y < 16; y++)
                {
                    byte val = memory.read(location++);
                    string hex = ByteToHex(val);
                    io.Print(hex + " ");
                }

                io.Print(":");

                for (int y = 0; y < 16; location--, y++) ;

                for (int y = 0; y < 16; y++)
                {
                    byte val = memory.read(location++);
                    char c = '\0';

                    if (val > 31 && val < 127)
                        c = (char)val;
                    else
                        c = '.';

                    io.Print(c);
                }

                io.PrintLine("");
            }
        }

        private void PokCmd(ref Emu6502.CPU.registerStruct registers, string parm1, string parm2)
        {
            ushort location = 0;
            ushort bval = 0;

            location = HexToInt(parm1);
            bval = HexToInt(parm2);

            memory.write(location, (byte)bval);

        }

        private void FilCmd(ref Emu6502.CPU.registerStruct registers, string startAddr, string endAddr, string byteval)
        {
            ushort sloc = HexToInt(startAddr);
            ushort eloc = HexToInt(endAddr);
            ushort bval = HexToInt(byteval);

            if (sloc > eloc || bval > 255)
            {
                io.Print('?');
                return;
            }

            for (; sloc <= eloc; sloc++)
                memory.write(sloc, (byte)bval);

        }

        private void AsmCmd(ref Emu6502.CPU.registerStruct registers, string addr, string opcode, string parm)
        {
            ushort location = HexToInt(addr);
            string p = "";

            // implied or accumulator
            if (parm == "")
            {
                int op = validOp(opcode + "1");

                if (op > -1)
                {
                    memory.write(location, (byte)op);

                    io.CursorUp();
                    io.CursorResetLine();
                    io.PrintLine(">A " + addr + " " + ByteToHex(op) + " " + opcode);
                    nextLocation = ++location;
                    asmMode = true;
                }
                else
                    InputErr();

                return;
            }

            //Remove any $ (not needed - all hex)
            for (int i = 0; i < parm.Length; i++) if (parm[i] != '$') p = p + parm[i];

            // immediate
            if (p.Length == 3 && p[0] == '#')
            {
                int op = validOp(opcode + "7");

                if (op > -1)
                {
                    memory.write(location, (byte)op);
                    location++;

                    byte b = (byte)HexToInt(p);
                    memory.write(location, b);

                    io.CursorUp();
                    io.CursorResetLine();
                    io.PrintLine(">A " + addr + " " + ByteToHex(op) + " " + ByteToHex(b) + " " + opcode + " " + p);
                    nextLocation = ++location;
                    asmMode = true;
                }
                else
                    InputErr();

                return;
            }

            // zero page
            if (p.Length == 2)
            {
                int op = validOp(opcode + "B");

                if (op > -1)
                {
                    memory.write(location, (byte)op);

                    byte b = (byte)HexToInt(p);
                    memory.write(++location, b);

                    io.CursorUp();
                    io.CursorResetLine();
                    io.PrintLine(">A " + addr + " " + ByteToHex(op) + " " + ByteToHex(b) + " " + opcode + " " + p);
                    nextLocation = ++location;
                    asmMode = true;
                }
                else
                    InputErr();

                return;
            }

            // zero page, x
            if (p.Length == 4 && p[2] == ',' && p[3] == 'X')
            {
                int op = validOp(opcode + "C");

                if (op > -1)
                {
                    memory.write(location, (byte)op);

                    string sb = p[0].ToString() + p[1].ToString();
                    byte b = (byte)HexToInt(sb);
                    memory.write(++location, b);

                    io.CursorUp();
                    io.CursorResetLine();
                    io.PrintLine(">A " + addr + " " + ByteToHex(op) + " " + ByteToHex(b) + " " + opcode + " " + p);
                    nextLocation = ++location;
                    asmMode = true;
                }
                else
                    InputErr();

                return;
            }

            // zero page, y
            if (p.Length == 4 && p[2] == ',' && p[3] == 'Y')
            {
                int op = validOp(opcode + "D");

                if (op > -1)
                {
                    memory.write(location, (byte)op);

                    string sb = p[0].ToString() + p[1].ToString();
                    byte b = (byte)HexToInt(sb);
                    memory.write(++location, b);

                    io.CursorUp();
                    io.CursorResetLine();
                    io.PrintLine(">A " + addr + " " + ByteToHex(op) + " " + ByteToHex(b) + " " + opcode + " " + p);
                    nextLocation = ++location;
                    asmMode = true;
                }
                else
                    InputErr();

                return;
            }

            //relative
            if (opcode == "BCC" || opcode == "BCS" || opcode == "BEQ" || opcode == "BMI"
                || opcode == "BNE" || opcode == "BPL" || opcode == "BVC" || opcode == "BVS")
            {
                int op = validOp(opcode + "5");

                if (op > -1)
                {
                    memory.write(location, (byte)op);

                    int b = HexToInt(p) - (location + 2);
                    byte b2 = (byte)b;

                    memory.write(++location, b2);

                    io.CursorUp();
                    io.CursorResetLine();
                    io.PrintLine(">A " + addr + " " + ByteToHex(op) + " " + ByteToHex(b2) + " " + opcode + " " + p);
                    nextLocation = ++location;
                    asmMode = true;
                }
                else
                    InputErr();

                return;
            }

            //absolute
            if (p.Length == 4 && p[0] != '(' && p[3] != ')')
            {
                int op = validOp(opcode + "2");

                if (op > -1)
                {
                    memory.write(location, (byte)op);

                    byte hi = (byte)HexToInt(p[0].ToString() + p[1].ToString());
                    byte lo = (byte)HexToInt(p[2].ToString() + p[3].ToString());
                    memory.write(++location, lo);
                    memory.write(++location, hi);

                    io.CursorUp();
                    io.CursorResetLine();
                    io.PrintLine(">A " + addr + " " + ByteToHex(op) + " " + ByteToHex(lo) + " " + ByteToHex(hi) + " " + opcode + " " + p);
                    nextLocation = ++location;
                    asmMode = true;
                }
                else
                    InputErr();

                return;
            }

            // abs, x
            if (p.Length == 6 && p[4] == ',' && p[5] == 'X')
            {
                int op = validOp(opcode + "3");

                if (op > -1)
                {
                    memory.write(location, (byte)op);

                    byte hi = (byte)HexToInt(p[0].ToString() + p[1].ToString());
                    byte lo = (byte)HexToInt(p[2].ToString() + p[3].ToString());
                    memory.write(++location, lo);
                    memory.write(++location, hi);

                    io.CursorUp();
                    io.CursorResetLine();
                    io.PrintLine(">A " + addr + " " + ByteToHex(op) + " " + ByteToHex(lo) + " " + ByteToHex(hi) + " " + opcode + " " + p);
                    nextLocation = ++location;
                    asmMode = true;
                }
                else
                    InputErr();

                return;
            }

            // abs, y
            if (p.Length == 6 && p[0] != '(' && p[3] != ')' && p[4] == ',' && p[5] == 'Y')
            {
                int op = validOp(opcode + "4");

                if (op > -1)
                {
                    memory.write(location, (byte)op);

                    byte hi = (byte)HexToInt(p[0].ToString() + p[1].ToString());
                    byte lo = (byte)HexToInt(p[2].ToString() + p[3].ToString());
                    memory.write(++location, lo);
                    memory.write(++location, hi);

                    io.CursorUp();
                    io.CursorResetLine();
                    io.PrintLine(">A " + addr + " " + ByteToHex(op) + " " + ByteToHex(lo) + " " + ByteToHex(hi) + " " + opcode + " " + p);
                    nextLocation = ++location;
                    asmMode = true;
                }
                else
                    InputErr();

                return;
            }


            // indexed indirect
            if (p.Length == 6 && p[0] == '(' && p[3] == ',' && p[4] == 'X' && p[5] == ')')
            {
                int op = validOp(opcode + "9");

                if (op > -1)
                {
                    memory.write(location, (byte)op);
                    location++;

                    byte b = (byte)HexToInt(p[1].ToString() + p[2].ToString());
                    memory.write(location, b);

                    io.CursorUp();
                    io.CursorResetLine();
                    io.PrintLine(">A " + addr + " " + ByteToHex(op) + " " + ByteToHex(b) + " " + opcode + " " + p);
                    nextLocation = ++location;
                    asmMode = true;
                }
                else
                    InputErr();

                return;
            }

            // indirect indexed
            if (p.Length == 6 && p[0] == '(' && p[3] == ')' && p[4] == ',' && p[5] == 'Y')
            {
                int op = validOp(opcode + "A");

                if (op > -1)
                {
                    memory.write(location, (byte)op);
                    location++;

                    byte b = (byte)HexToInt(p[1].ToString() + p[2].ToString());
                    memory.write(location, b);

                    io.CursorUp();
                    io.CursorResetLine();
                    io.PrintLine(">A " + addr + " " + ByteToHex(op) + " " + ByteToHex(b) + " " + opcode + " " + p);
                    nextLocation = ++location;
                    asmMode = true;
                }
                else
                    InputErr();

                return;
            }

            // indirect
            if (p.Length == 6 && p[0] == '(' && p[5] == ')')
            {
                int op = validOp(opcode + "8");

                if (op > -1)
                {
                    memory.write(location, (byte)op);

                    byte hi = (byte)HexToInt(p[1].ToString() + p[2].ToString());
                    byte lo = (byte)HexToInt(p[3].ToString() + p[4].ToString());
                    memory.write(++location, lo);
                    memory.write(++location, hi);

                    io.CursorUp();
                    io.CursorResetLine();
                    io.PrintLine(">A " + addr + " " + ByteToHex(op) + " " + ByteToHex(lo) + " " + ByteToHex(hi) + " " + opcode + " " + p);
                    nextLocation = ++location;
                    asmMode = true;
                }
                else
                    InputErr();

                return;
            }

            InputErr();
            return;
        }

        private void InputErr()
        {
            io.CursorUp();
            io.CursorToEOL();
            io.PrintLine("?");
            asmMode = false;
        }

        private void DisplayRegisters()
        {
            io.PrintLine("     PC  SR AC XR YR SP  NV-BDIZC");
            io.Print(";   " + WordToHex(registers.pc) + " ");
            io.Print(ByteToHex(registers.sr) + " " + ByteToHex(registers.a) + " " + ByteToHex(registers.x) +
                " " + ByteToHex(registers.y) + " " + ByteToHex(registers.sp) + "  ");

            string flags = Bit(registers.sr, srflags.n).ToString();
            flags += Bit(registers.sr, srflags.v).ToString();
            flags += Bit(registers.sr, srflags.x).ToString();
            flags += Bit(registers.sr, srflags.b).ToString();
            flags += Bit(registers.sr, srflags.d).ToString();
            flags += Bit(registers.sr, srflags.i).ToString();
            flags += Bit(registers.sr, srflags.z).ToString();
            flags += Bit(registers.sr, srflags.c).ToString();

            io.PrintLine(flags);
        }

        #region Conversion Methods

        private string ByteToHex(int n)
        {
            string x = "00" + b2h(n);
            string ans = x.Substring(x.Length - 2, 2);
            return ans;
        }

        private string WordToHex(int n)
        {
            string x = "0000" + b2h(n);
            string ans = x.Substring(x.Length - 4, 4);
            return ans;
        }

        private string b2h(int n)
        {
            string hexval = "";

            if (n == 0) return hexval;
            else
            {
                int r = n % 16;
                n = n / 16;
                hexval = hexval + b2h(n);

                switch (r)
                {
                    case 10: return hexval + "A";
                    case 11: return hexval + "B";
                    case 12: return hexval + "C";
                    case 13: return hexval + "D";
                    case 14: return hexval + "E";
                    case 15: return hexval + "F";
                    default: return hexval + r.ToString();
                }

            }
        }

        private ushort HexToInt(string hex)
        {
            int v = 0;
            int digit = 0;
            int pwr = 0;
            int answer = 0;

            for (int i = hex.Length - 1; i > -1; i--)
            {
                char c = hex[i];

                switch (c)
                {
                    case '0': v = 0; break;
                    case '1': v = 1; break;
                    case '2': v = 2; break;
                    case '3': v = 3; break;
                    case '4': v = 4; break;
                    case '5': v = 5; break;
                    case '6': v = 6; break;
                    case '7': v = 7; break;
                    case '8': v = 8; break;
                    case '9': v = 9; break;
                    case 'A': v = 10; break;
                    case 'B': v = 11; break;
                    case 'C': v = 12; break;
                    case 'D': v = 13; break;
                    case 'E': v = 14; break;
                    case 'F': v = 15; break;
                }

                pwr = 1;

                for (int p = 0; p < digit; p++)
                    pwr = pwr * 16;

                answer = answer + (v * pwr);
                digit++;

            }
            return (ushort)answer;
        }

        private string UCase(string s)
        {
            string result = "";

            for (int x = 0; x < s.Length; x++)
            {
                int l = (int)s[x];
                if (l > 96 && l < 123) l = l - 32;
                result = result + (char)l;
            }

            return result;
        }

        private int validOp(string op)
        {
            string[] xops = {
                    "BRK1", "ORA9", ""    , "" , ""    , "ORAB", "ASLB", "", "PHP1", "ORA7", "ASL1", "" , ""    , "ORA2", "ASL2", "",
                    "BPL5", "ORAA", ""    , "" , ""    , "ORAC", "ASLC", "", "CLC1", "ORA4", ""    , "" , ""    , "ORA3", "ASL3", "",
                    "JSR2", "AND9", ""    , "" , "BITB", "ANDB", "ROLB", "", "PLP1", "AND7", "ROL1", "" , "BIT2", "AND2", "ROL2", "",
                    "BMI5", "ANDA", ""    , "" , ""    , "ANDC", "ROLC", "", "SEC1", "AND4", ""    , "" , ""    , "AND3", "ROL3", "",
                    "RTI1", "EOR9", ""    , "" , ""    , "EORB", "LSRB", "", "PHA1", "EOR7", "LSR1", "" , "JMP2", "EOR2", "LSR2", "",
                    "BVC5", "EORA", ""    , "" , ""    , "EORC", "LSRC", "", "CLI1", "EOR4", ""    , "" , ""    , "EOR3", "LSR3", "",
                    "RTS1", "ADC9", ""    , "" , ""    , "ADCB", "RORB", "", "PLA1", "ADC7", "ROR1", "" , "JMP8", "ADC2", "ROR2", "",
                    "BVS5", "ADCA", ""    , "" , ""    , "ADCC", "RORC", "", "SEI1", "ADC4", ""    , "" , ""    , "ADC3", "ROR3", "",
                    ""    , "STA9", ""    , "" , "STYB", "STAB", "STXB", "", "DEY1", ""    , "TXA1", "" , "STY2", "STA2", "STX2", "",
                    "BCC5", "STAA", ""    , "" , "STYC", "STAC", "STXD", "", "TYA1", "STA4", "TXS1", "" , ""    , "STA3", ""    , "",
                    "LDY7", "LDA9", "LDX7", "" , "LDYB", "LDAB", "LDXB", "", "TAY1", "LDA7", "TAX1", "" , "LDY2", "LDA2", "LDX2", "",
                    "BCS5", "LDAA", ""    , "" , "LDYC", "LDAC", "LDXD", "", "CLV1", "LDA4", "TSX1", "" , "LDY3", "LDA3", "LDX4" ,"",
                    "CPY7", "CMP9", ""    , "" , "CPYB", "CMPB", "DECB", "", "INY1", "CMP7", "DEX1", "" , "CPY2", "CMP2", "DEC2", "",
                    "BNE5", "CMPA", ""    , "" , ""    , "CMPC", "DECC", "", "CLD1", "CMP4", ""    , "" , ""    , "CMP3", "DEC3", "",
                    "CPX7", "SBC9", ""    , "" , "CPXB", "SBCB", "INCB", "", "INX1", "SBC7", "NOP1", "" , "CPX2", "SBC2", "INC2", "",
                    "BEQ5", "SBCA", ""    , "" , ""    , "SBCC", "INCC", "", "SED1", "SBC4", ""    , "" , ""    , "SBC3", "INC3", ""};

            for (byte i = 0; i <= 254; i++)
                if (xops[i] == op) return i;

            return -1;

        }

        #endregion

    }
}
