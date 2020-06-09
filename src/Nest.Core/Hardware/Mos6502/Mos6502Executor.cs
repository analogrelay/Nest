using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nest.Memory;

namespace Nest.Hardware.Mos6502
{
    public static partial class Mos6502Executor
    {
        private delegate Mos6502State InstructionExecutor(int address, Mos6502State currentState, MemoryUnit memory);

        private readonly static InstructionExecutor[] _executorsTable = new InstructionExecutor[] {
            Adc, Ahx, Alr, Anc, And, Arr, Asl, Axs,
            BranchIfClear(Mos6502Flags.Carry), // Bcc
            BranchIfSet(Mos6502Flags.Carry), // Bcs
            BranchIfSet(Mos6502Flags.Zero), // Beq
            Bit,
            BranchIfSet(Mos6502Flags.Negative), // Bmi
            BranchIfClear(Mos6502Flags.Zero), // Bne
            BranchIfClear(Mos6502Flags.Negative), // Bpl
            Brk,
            BranchIfClear(Mos6502Flags.Overflow), // Bvc
            BranchIfSet(Mos6502Flags.Overflow), // Bvs
            Clc, Cld, Cli, Clv, Cmp, Cpx,
            Cpy, Dcp, Dec, Dex, Dey, Eor, Inc, Inx,
            Iny, Isc, Jmp, Jsr, Kil, Las, Lax, Lda,
            Ldx, Ldy, Lsr, Nop, Ora, Pha, Php, Pla,
            Plp, Rla, Rol, Ror, Rra, Rti, Rts, Sax,
            Sbc, Sec, Sed, Sei, Shx, Shy, Slo, Sre,
            Sta, Stx, Sty, Tas, Tax, Tay, Tsx, Txa,
            Txs, Tya, Xaa,
        };

        public static Mos6502State Execute(in Mos6502Instruction instruction, in Mos6502State state, MemoryUnit memory, ILogger? logger = null)
        {
            logger ??= NullLogger.Instance;

            logger.LogTrace("Executing {Instruction} (Current CPU State: {State})", instruction, state);

            // Resolve the address used to store the operand
            var (address, crossesPageBoundary) = ResolveAddress(instruction.AddressingMode, state, memory);

            // Look up the executor in the table
            Debug.Assert((int)instruction.Operation >= 0 && (int)instruction.Operation < _executorsTable.Length, "Operation has no entry in the executors table!");
            var executor = _executorsTable[(int)instruction.Operation];

            // Advance the PC to the end of the instruction
            var newState = state.With(pc: state.PC + instruction.InstructionSize);

            // Execute!
            newState = executor(address, newState, memory);

            logger.LogDebug("Executed {Instruction} (New CPU State: {State})", instruction, newState);

            return newState;
        }

        internal static (int address, bool crossesPageBoundary) ResolveAddress(Mos6502AddressingMode addressingMode, in Mos6502State state, MemoryUnit memory)
        {
            static (int address, bool crossesPageBoundary) ComputeOffset(int baseAddress, int offset)
            {
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
                    if (offset < 0x80)
                    {
                        return (state.PC + 2 + offset, false);
                    }
                    else
                    {
                        return (state.PC + 2 + offset - 0x100, false);
                    }
                case Mos6502AddressingMode.Absolute: return ((int)memory.ReadUInt16LittleEndian(state.PC + 1), false);
                case Mos6502AddressingMode.AbsoluteX: return ComputeOffset(memory.ReadUInt16LittleEndian(state.PC + 1), state.X);
                case Mos6502AddressingMode.AbsoluteY: return ComputeOffset(memory.ReadUInt16LittleEndian(state.PC + 1), state.Y);
                case Mos6502AddressingMode.Indirect: return (memory.ReadUInt16LittleEndian(memory.ReadUInt16LittleEndian(state.PC + 1)), false);
                case Mos6502AddressingMode.IndexedIndirect:
                    return (memory.ReadUInt16LittleEndian((int)memory.ReadByte(state.PC + 1) + state.X), false);
                case Mos6502AddressingMode.IndirectIndexed:
                    return ComputeOffset(memory.ReadUInt16LittleEndian((int)memory.ReadByte(state.PC + 1)), state.Y);
                default:
                    throw new NotSupportedException($"Unknown Addressing Mode: {addressingMode}.");
            }
        }
    }
}