using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nest.Memory;

namespace Nest.Hardware
{
    public static class Mos6502Executor
    {
        private readonly static Func<Mos6502Instruction, Mos6502.State, VirtualMemory, Mos6502.State>[] _executorsTable;

        public static Mos6502.State Execute(in Mos6502Instruction instruction, in Mos6502.State state, VirtualMemory memory, ILogger? logger = null)
        {
            static int ComputeRelativeAddress(int start, int offset) =>
                offset < 0x80 ? start + offset : (start + offset - 0x100);

            logger ??= NullLogger.Instance;

            logger.LogTrace("Executing {Instruction} (Current CPU State: {State})", instruction, state);

            // Resolve the address used to store the operand
            var (address, crossesPageBoundary) = ResolveAddress(instruction.AddressingMode, state, memory, logger);

            logger.LogTrace("Executed {Instruction} (New CPU State: {State})", instruction, newState);

            return newState;
        }

        private static (int address, bool crossesPageBoundary) ResolveAddress(Mos6502AddressingMode addressingMode, in Mos6502.State state, VirtualMemory memory, ILogger logger)
        {
            static (int address, bool crossesPageBoundary) ComputeOffset(int baseAddress, int offset) {
                var addr = baseAddress + offset;
                return (addr, (baseAddress & 0xFF00) != (addr & 0xFF00));
            }

            switch (addressingMode)
            {
                case Mos6502AddressingMode.Implicit:
                case Mos6502AddressingMode.Accumulator: return (0, false);

                case Mos6502AddressingMode.Immediate: return (state.PC + 1, false);
                case Mos6502AddressingMode.ZeroPage: return (memory.ReadByte(state.PC + 1), false);
                case Mos6502AddressingMode.ZeroPageY: return ((memory.ReadByte(state.PC + 1) + state.Y) & 0xFF, false);
                case Mos6502AddressingMode.ZeroPageX: return ((memory.ReadByte(state.PC + 1) + state.X) & 0xFF, false);
                case Mos6502AddressingMode.Relative:
                    var offset = memory.ReadByte(state.PC + 1);
                    if (offset < 0x80) {
                        return (state.PC + 2 + offset, false);
                    } else {
                        return (state.PC + 2 + offset - 0x100, false);
                    }
                case Mos6502AddressingMode.Absolute: return ((int)memory.ReadUInt16LittleEndian(state.PC + 1), false);
                case Mos6502AddressingMode.AbsoluteX: return ComputeOffset(memory.ReadUInt16LittleEndian(state.PC + 1), state.X);
                case Mos6502AddressingMode.AbsoluteY: return ComputeOffset(memory.ReadUInt16LittleEndian(state.PC + 1), state.Y);
                case Mos6502AddressingMode.Indirect: return (memory.ReadUInt16LittleEndian(memory.ReadUInt16LittleEndian(state.PC + 1)), false);
                case Mos6502AddressingMode.IndexedIndirect: return (memory.ReadUInt16LittleEndian((int)memory.ReadByte(state.PC + 1) + state.X), false)
                case Mos6502AddressingMode.IndirectIndexed: return ComputeOffset(memory.ReadUInt16LittleEndian((int)memory.ReadByte(state.PC + 1)), state.Y),
            }
        }
    }
}