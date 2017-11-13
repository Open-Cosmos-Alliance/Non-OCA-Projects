using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GruntyOS.IO;
namespace GruntyOS.Lang
{
    public class Scanner
    {
        private class StringLiteral
        {
            public string Value;
        }
        bool isWhiteSpace(char c)
        {
            if (c == ' ')
            {
                return true;
            }
            else if (c == '\r')
            {
                return true;
            }
            else if (c == '\n')
            {
                return true;
            }
            return false;
        }
        bool isLetter(char c)
        {
            byte code = (byte)c;
            if (code >= 65 && code <= 90)
            {
                return true;
            }
            if (code >= 97 && code <= 122)
            {
                return true;
            }
            return false;
        }
        bool isLetterOrDigit(char c)
        {
            if (isLetter(c))
            {
                return true;
            }
            byte code = (byte)c;
            if (code >= 48 && code <= 58)
            {
                return true;
            }
            return false;
        }
        bool isDigit(char c)
        {
            
            byte code = (byte)c;
            if (code >= 48 && code <= 58)
            {
                return true;
            }
            return false;
        }
        object Dot = new object();
        
        public List<object> getTokens(string s)
        {
            List<Object> fin = new List<object>();
            TextReader tr = new TextReader(s);
            while (tr.Length > tr.pos)
            {
                while (isWhiteSpace(tr.Peek()))
                {
                    tr.Read();
                }
                char ch = tr.Peek();

                if (ch == '"')
                {
                    tr.Read();
                    string accum = "";
                    while (tr.Peek() != '"')
                    {
                        accum += tr.Read().ToString();
                    }
                    tr.Read();

                    fin.Add(accum);
                }
                else if (ch == '\'')
                {
                    tr.Read();
                    string accum = "";
                    while (tr.Peek() != '\'')
                    {
                        accum += tr.Read().ToString();
                    }
                    tr.Read();

                    StringLiteral sl = new StringLiteral();
                    sl.Value = accum;
                    fin.Add(sl);
                }
                
                else if (isDigit(ch))
                {
                    string accum = "";
                    while (isDigit(tr.Peek()))
                    {
                        accum += tr.Read().ToString();
                    }
                    fin.Add(accum);
                }
                else if (isLetter(ch) || ch == '/' || ch == '-')
                {
                    string accum = "";
                    while (isLetterOrDigit(tr.Peek()) || tr.Peek() == '_' || tr.Peek() == '.' || tr.Peek() == '/' || tr.Peek() == '-')
                    {
                        accum += tr.Read().ToString();
                    }
                    fin.Add(accum);
                }

            }
            return fin;
        }
    }
}
namespace GruntyOS.String
{
    public class Util
    {
        public static bool Contains(string Str, char c)
        {
            foreach (char ch in Str)
            {
                if (ch == c)
                    return true;
            }
            return false;
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
        public static string cleanName(string name)
        {
            if (name.Substring(0, 1) == "/")
            {
                name = name.Substring(1, name.Length - 1);
            }
            if (name.Substring(name.Length - 1, 1) == "/")
            {
                name = name.Substring(0, name.Length - 1);
            }
            return name;
        }
        public static int LastIndexOf(string This, char ch)
        {
            int ret = -1;
            int i = 0;
            foreach (char c in This)
            {
                if (c == ch)
                {
                    ret = i;
                }

                i++;
            }
            return ret;
        }
    }
}
