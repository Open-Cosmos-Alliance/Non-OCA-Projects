﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Assembler;
using Cosmos.Assembler.x86;

namespace Cosmos.Assembler.XSharp {
  public class RegisterEBX : Register32 {
    public static readonly RegisterEBX Instance = new RegisterEBX();

    public static RegisterEBX operator ++(RegisterEBX aRegister) {
      new INC { DestinationReg = aRegister.GetId() };
      return aRegister;
    }
    public static RegisterEBX operator --(RegisterEBX aRegister) {
      new Dec { DestinationReg = aRegister.GetId() };
      return aRegister;
    }
    public static RegisterEBX operator <<(RegisterEBX aRegister, int aCount) {
      new ShiftLeft { DestinationReg = aRegister.GetId(), SourceValue = (uint)aCount };
      return aRegister;
    }
    public static RegisterEBX operator >>(RegisterEBX aRegister, int aCount) {
      new ShiftRight { DestinationReg = aRegister.GetId(), SourceValue = (uint)aCount };
      return aRegister;
    }

    public static implicit operator RegisterEBX(Cosmos.Assembler.ElementReference aReference) {
      Instance.Move(aReference);
      return Instance;
    }

    public static implicit operator RegisterEBX(MemoryAction aAction) {
      Instance.Move(aAction);
      return Instance;
    }

    public static implicit operator RegisterEBX(UInt32 aValue) {
      Instance.Move(aValue);
      return Instance;
    }

    public static implicit operator RegisterEBX(RegisterEAX aValue) {
      Instance.Move(aValue.GetId());
      return Instance;
    }

    public static implicit operator RegisterEBX(RegisterECX aValue) {
      Instance.Move(aValue.GetId());
      return Instance;
    }

    public static implicit operator RegisterEBX(RegisterEDX aValue) {
      Instance.Move(aValue.GetId());
      return Instance;
    }

    public void RotateRight(int aCount) {
      new RotateRight { DestinationReg = Registers.EBX, SourceValue = (uint)aCount };
    }

  }
}
