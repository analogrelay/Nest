using System;
using Nest.Memory;

namespace Nest.Hardware.Mos6502
{
    public static partial class Mos6502Decoder
    {
        public static Mos6502Instruction Decode(MemoryUnit memory, int offset)
        {
            var opcode = memory.ReadByte(offset);

            return new Mos6502Instruction(
                (int)opcode,
                _operationTable[opcode],
                _addressingModeTable[opcode],
                _cycleCountTable[opcode],
                _instructionSizeTable[opcode]);
        }
    }
}