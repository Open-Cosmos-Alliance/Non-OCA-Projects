﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos.Compiler.XSharp {
  public enum TokenType { 
    // Line based
    Comment, LiteralAsm
    //
    , Register, Keyword, AlphaNum
    // Values
    , ValueInt
    // Symbols
    , Assignment, BracketLeft, BracketRight, Plus, Minus, Colon, Dollar, CurlyLeft, CurlyRight, Dot
    // 
    , WhiteSpace
    // For now used during scanning while user is typing, but in future could be user methods we have to find etc
    , Unknown
    }

  public class Token {
    public TokenType Type;
    public string Value;
    public int SrcPosStart;
    public int SrcPosEnd;
  }
}
