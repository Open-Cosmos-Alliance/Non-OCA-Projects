using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct size_t
{
    public uint value;
    public size_t(uint val)
    {
        value = val;
    }
    public static implicit operator size_t(uint val)
    {
        return new size_t(val);
    }
    public static implicit operator size_t(int val)
    {
        return new size_t((uint)val);
    }
    public static implicit operator size_t(byte val)
    {
        return new size_t((uint)val);
    }
    public static size_t operator +(size_t first, size_t second)
    {
        return new size_t(first.value + second.value);
    }

    public static size_t operator -(size_t first, size_t second)
    {
        return new size_t(first.value - second.value);
    }
}

