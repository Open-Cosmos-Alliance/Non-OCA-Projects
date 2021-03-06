﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Assembler;
using Cosmos.Assembler.x86;

namespace Cosmos.Assembler.XSharp {
    public class PortNumber {
        public readonly RegistersEnum Register;

        public PortNumber(RegistersEnum aRegister)
        {
            Register = aRegister;
        }

        public override string ToString() {
            return Registers.GetRegisterName(Register);
        }
    }
}