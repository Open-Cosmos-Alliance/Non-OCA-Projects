using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emu6502
{
    public class ConsoleIO
    {
        Cosmos.Hardware.TextScreen screen = new Cosmos.Hardware.TextScreen();
        Cosmos.Hardware.Keyboard kb = new Cosmos.Hardware.Keyboard();        

        const int MAXROWS = 24;
        const int MAXCOLS = 79;

        private int current_row = 0;
        private int current_col = 0;

        public ConsoleIO()
        {
            this.Clear();
            current_col = 0;
            current_row = 0;

            this.PrintLine("CBM11 8-BIT OPERATING SYSTEM");
            this.PrintLine("6502 EMULATION - SCOTT HUTTER");
            this.PrintLine("Cosmos BIOS");
            this.PrintLine("65535 BYTES FREE");
            this.PrintLine("");
        }

        public void Clear()
        {
            for (int x = 0; x <= MAXROWS; x++)
            {
                for (int y = 0; y <= MAXCOLS; y++)
                    screen[y, x] = ' ';
            }

            current_col = 0;
            current_row = 0;

            screen.SetCursorPos(current_col, current_row);
        }

        public void Print(int row, int col, string s)
        {
            if (row > MAXROWS || col > MAXCOLS || row < 0 || col < 0)
                return;

            current_row = row;
            current_col = col;

            this.Print(s);
        }

        public void Print(string s)
        {
            for (int x = 0; x < s.Length; x++)
                this.Print(s[x]);
        }

        public void PrintLine(string s)
        {
            this.Print(s);

            this.Print((char)13);
        }

        public void Print(char c)
        {
            switch(c)
            {
                case (char)8:
                {
                    if (current_col > 0)
                    {
                        current_col--;
                    }
                    break;
                }
                case (char)13:
                {
                    current_col = 0;
                    current_row++;
                    break;
                }
                default:
                {
                    screen[current_col, current_row] = c;
                    current_col++;
                    break;
                }

            }

            if (current_col > MAXCOLS)
            {
                current_row++;
                current_col = 0;
            }

            if (current_row > MAXROWS)
            {
                current_row = MAXROWS;
                scrollDown();
                
            }

            screen.SetCursorPos(current_col, current_row);
        }

        public void Poke(ushort offset, byte b)
        {
            int row = offset / MAXCOLS;
            int col = offset % MAXCOLS;

            screen[col, row] = (char)b;
        }

        public void CursorUp()
        {
            if (current_row > 0) current_row--;
        }

        public void CursorResetLine()
        {
            current_col = 0;

            for (int y = 0; y <= MAXCOLS; y++)
                screen[y, MAXROWS] = ' ';
        }

        public void CursorToEOL()
        {
            for (current_col = MAXCOLS; current_col >= 0; current_col--)
            {
                if (screen[current_col, current_row] != ' ')
                { current_col++; break; }
            }


        }

        public char ReadKey()
        {
            char c = kb.ReadChar();

            // Bug in keyboard handler?  (should be just 8, not 2408)
            if (c == 2408) c = (char)8;
            if (c == 10) c = (char)13;

            return c;
        }

        public string ReadLine()
        {
            return ReadLine(false);
        }

        public string ReadLine(bool UpperCase)
        {

            char c = (char)0;
            string s = "";
            
            while (c != 13) {

                c = this.ReadKey();

                if (UpperCase) c = ucase(c);

                this.Print(c);

                if(c!=13)
                    s = s + c;
            };

            return s;

        }

        private void scrollDown()
        {
            for (int i = 1; i < MAXROWS+1 ; i++)
                moveRowUp(i);
            
            for (int y = 0; y <= MAXCOLS; y++)
                screen[y, MAXROWS] = ' ';
        }

        private void moveRowUp(int rowNumber)
        {
            if (rowNumber > 0 && rowNumber < MAXROWS + 1)
            {
                for (int c = 0; c < MAXCOLS + 1; c++)
                    screen[c, rowNumber - 1] = screen[c, rowNumber];
            }
        }

        private char ucase(char c)
        {
           if (c > 0x60 && c < 0x7b) c = (char)(c - 0x20);
           return c;
        }

    }
}
