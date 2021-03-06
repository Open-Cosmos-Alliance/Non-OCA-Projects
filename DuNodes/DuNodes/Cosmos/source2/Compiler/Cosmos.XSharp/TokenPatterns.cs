﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Cosmos.Compiler.XSharp {
  public class TokenPatterns {
    public delegate void CodeFunc(TokenList aTokens, ref List<string> rCode);
    protected Dictionary<TokenPattern, CodeFunc> mPatterns = new Dictionary<TokenPattern, CodeFunc>();
    protected Dictionary<string, CodeFunc> mKeywords = new Dictionary<string, CodeFunc>();
    protected string mGroup;
    protected string mProcedureName;
    protected bool mInIntHandler;

    public TokenPatterns() {
      AddPatterns();
      AddKeywords();
    }

    protected string Quoted(string aString) {
      return "\"" + aString + "\"";
    }

    protected void AddKeywords() {
      AddKeyword("Call", delegate(TokenList aTokens, ref List<string> rCode) {
        string xLabel = aTokens[1].Value;
        var xPattern = new TokenPattern(aTokens);
        if (xPattern == "Call ABC") {
          rCode.Add("new Call {{ DestinationLabel = " + Quoted(mGroup + "_" + xLabel) + " }};");
        } else {
          rCode = null;
        }
      });

      AddKeyword("Exit", delegate(TokenList aTokens, ref List<string> rCode) {
        rCode.Add("new Jump {{ DestinationLabel = " + Quoted(mGroup + "_" + mProcedureName + "_Exit") + " }};");
      });

      AddKeyword("Group", delegate(TokenList aTokens, ref List<string> rCode) {
        var xPattern = new TokenPattern(aTokens);
        if (xPattern == "Group ABC") {
          mGroup = aTokens[1].Value;
        } else {
          rCode = null;
        }
      });

      AddKeyword("InterruptHandler", delegate(TokenList aTokens, ref List<string> rCode) {
        mInIntHandler = true;
        var xPattern = new TokenPattern(aTokens);
        if (xPattern == "InterruptHandler ABC {") {
          mProcedureName = aTokens[1].Value;
          rCode.Add("new Label(\"" + mGroup + "_{1}\");");
        } else {
          rCode = null;
        }
      });

      AddKeyword("Jump", delegate(TokenList aTokens, ref List<string> rCode) {
        var xPattern = new TokenPattern(aTokens);
        if (xPattern == "Jump ABC") {
          rCode.Add("new Jump {{ DestinationLabel = \"" + mGroup + "_{1}\" }};");
        } else {
          rCode = null;
        }
      });

      AddKeyword("Return", "new Return();");
      AddKeyword("ReturnInterrupt", "new IRET();");
      AddKeyword("PopAll", "new Popad();");
      AddKeyword("PushAll", "new Pushad();");

      AddKeyword("Procedure", delegate(TokenList aTokens, ref List<string> rCode) {
        mInIntHandler = false;
        var xPattern = new TokenPattern(aTokens);
        if (xPattern == "Procedure ABC {") {
          mProcedureName = aTokens[1].Value;
          rCode.Add("new Label(\"" + mGroup + "_{1}\");");
        } else {
          rCode = null;
        }
      });
    }

    protected void AddKeyword(string aKeyword, CodeFunc aCode) {
      mKeywords.Add(aKeyword.ToUpper(), aCode);
    }

    protected void AddKeyword(string aKeyword, string aCode) {
      AddKeyword(aKeyword, delegate(TokenList aTokens, ref List<string> rCode) {
        rCode.Add(aCode);
      });
    }

    protected int IntValue(Token aToken) {
      if (aToken.Value.StartsWith("0x")) {
        return int.Parse(aToken.Value.Substring(2), NumberStyles.AllowHexSpecifier);
      } else {
        return int.Parse(aToken.Value);
      }
    }

    protected void AddPatterns() {
      AddPattern("! Move EAX, 0",
        "new LiteralAssemblerCode(\"{0}\");"
      );
      AddPattern("# Comment",
        "new Comment(\"{0}\");"
      );
      AddPattern("Label:" ,
        "new Label(\"{0}\");"
      );

      AddPattern("EAX = 123",
        "new Mov{{ DestinationReg = RegistersEnum.{0}, SourceValue = {2} }};"
      );
      AddPattern("EAX = EAX",
        "new Mov{{ DestinationReg = RegistersEnum.{0}, SourceReg = RegistersEnum.{2} }};"
      );
      AddPattern("EAX = EAX[1]",
        "new Mov {{"
          + " DestinationReg = RegistersEnum.{0}"
          + ", SourceReg = RegistersEnum.{2}, SourceIsIndirect = true, SourceDisplacement = {4}"
          + "}};"
      );
      AddPattern("EAX = EAX[-1]",
        "new Mov {{"
          + " DestinationReg = RegistersEnum.{0}"
          + ", SourceReg = RegistersEnum.{2}, SourceIsIndirect = true, SourceDisplacement = -{4}"
          + "}};"
      );

      AddPattern("Port[DX] = AX", 
        // TODO: DX only for index
        // TODO: Src reg can only be EAX, AX, AL
        "new Out {{ DestinationReg = RegistersEnum.{5}}};"
      );

      AddPattern("+EAX",
        "new Push {{"
          + " DestinationReg = RegistersEnum.{1}"
          + "}};"
      );
      AddPattern("-EAX",
        "new Pop {{"
          + " DestinationReg = RegistersEnum.{1}"
          + "}};"
      );

      AddPattern("Variable = EAX", delegate(TokenList aTokens, ref List<string> rCode) {
        rCode.Add("new Mov {{"
         + " DestinationRef = Cosmos.Assembler.ElementReference.New(\"" + mGroup + "_{0}\"), DestinationIsIndirect = true"
         + " , SourceReg = RegistersEnum.{2}"
         + " }};");
      });

      // TODO: Allow asm to optimize these to Inc/Dec
      AddPattern("EAX + 1", delegate(TokenList aTokens, ref List<string> rCode) {
        if (IntValue(aTokens[2]) == 1) {
          rCode.Add("new INC {{ DestinationReg = RegistersEnum.{0} }};");
        } else {
          rCode.Add("new Add {{ DestinationReg = RegistersEnum.{0}, SourceValue = {2} }};");
        }
      });

      AddPattern("EAX - 1", delegate(TokenList aTokens, ref List<string> rCode) {
        if (IntValue(aTokens[2]) == 1) {
          rCode.Add("new Dec {{ DestinationReg = RegistersEnum.{0} }};");
        } else {
          rCode.Add("new Sub {{ DestinationReg = RegistersEnum.{0}, SourceValue = {2} }};");
        }
      });

      AddPattern("}", delegate(TokenList aTokens, ref List<string> rCode) {
        rCode.Add("new Label(\"" + mGroup + "_" + mProcedureName + "_Exit\");");
        if (mInIntHandler) {
          rCode.Add("new IRET();");
        } else {
          rCode.Add("new Return();");
        }
      });
    }

    public List<string> GetCode(TokenList aTokens) {
      CodeFunc xAction = null;
      List<string> xResult = new List<string>();
      if (aTokens[0].Type == TokenType.Keyword) {
        if (mKeywords.TryGetValue(aTokens[0].Value.ToUpper(), out xAction)) {
          xAction(aTokens, ref xResult);
          if (xResult == null) {
            throw new Exception("Unrecognized syntax for keyword: " + aTokens[0].Value);
          }
        }
      }
      if (xAction == null) {
        var xPattern = new TokenPattern(aTokens);
        if (!mPatterns.TryGetValue(xPattern, out xAction)) {
          throw new Exception("Token pattern not found.");
        }
        xAction(aTokens, ref xResult);
      }

      for(int i = 0; i < xResult.Count; i++) {
        xResult[i] = string.Format(xResult[i], aTokens.Select(c => c.Value).ToArray());
      }
      return xResult;
    }

    protected void AddPattern(string aPattern, CodeFunc aCode) {
      mPatterns.Add(new TokenPattern(aPattern), aCode);
    }

    protected void AddPattern(string aPattern, string aCode) {
      AddPattern(aPattern, delegate(TokenList aTokens, ref List<string> rCode) {
        rCode.Add(aCode);
      });
    }

  }
}
