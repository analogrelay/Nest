using System;
using Nest.Memory;

namespace Nest.Hardware.Mos6502
{
    public static partial class Mos6502Executor
    {
        private static Mos6502State Adc(int address, Mos6502State currentState, MemoryUnit memory)
        {
            // ADC: A = A + M + P[Carry]

            var m = memory.ReadByte(address);
            var r = currentState.A + m + (int)(currentState.P & Mos6502Flags.Carry);

            var flags = currentState.P;

            // Carry out if r overflows
            flags = flags.SetIf(r > 0xFF, Mos6502Flags.Carry);
            r = (byte)r;

            flags = flags.SetIf(r == 0, Mos6502Flags.Zero);
            flags = flags.SetIf((r & 0b1000_0000) != 0, Mos6502Flags.Negative);

            // Check if the overflow bit should be set.
            // The bit should be set when both A and M have the same sign bit, and R has a different sign bit.
            // This indicates that signed addition overflowed and the sign bit is inaccurate.
            // We set based on this truth table
            // | # |      A      |      M      |      R      |    A ^ M    |    A ^ R    | Value |
            // | 1 | 0b0xxx_xxxx | 0b0xxx_xxxx | 0b0xxx_xxxx | 0b0xxx_xxxx | 0b0xxx_xxxx |   F   |
            // | 2 | 0b0xxx_xxxx | 0b0xxx_xxxx | 0b1xxx_xxxx | 0b0xxx_xxxx | 0b1xxx_xxxx |   T   |
            // | 3 | 0b0xxx_xxxx | 0b1xxx_xxxx | 0b0xxx_xxxx | 0b1xxx_xxxx | 0b0xxx_xxxx |   F   |
            // | 4 | 0b0xxx_xxxx | 0b1xxx_xxxx | 0b1xxx_xxxx | 0b1xxx_xxxx | 0b1xxx_xxxx |   F   |
            // | 5 | 0b1xxx_xxxx | 0b0xxx_xxxx | 0b0xxx_xxxx | 0b1xxx_xxxx | 0b1xxx_xxxx |   F   |
            // | 6 | 0b1xxx_xxxx | 0b0xxx_xxxx | 0b1xxx_xxxx | 0b1xxx_xxxx | 0b0xxx_xxxx |   F   |
            // | 7 | 0b1xxx_xxxx | 0b1xxx_xxxx | 0b0xxx_xxxx | 0b0xxx_xxxx | 0b1xxx_xxxx |   T   |
            // | 8 | 0b1xxx_xxxx | 0b1xxx_xxxx | 0b1xxx_xxxx | 0b0xxx_xxxx | 0b0xxx_xxxx |   F   |
            // From the above, we can see that overflow is set in rows 2 and 7, when:
            // * A^M has the sign bit clear
            // * AND, A^R has the sign bit set.
            flags = ((currentState.A ^ m) & 0x80) == 0 && ((currentState.A ^ r) & 0x80) == 0x80 ?
                flags | Mos6502Flags.Overflow :
                flags & ~Mos6502Flags.Overflow;

            return currentState.With(a: r, p: flags);
        }

        private static Mos6502State Ahx(int address, Mos6502State currentState, MemoryUnit memory)
        {
            // AHX is an unofficial opcode with odd behavior
            // Based this test on https://github.com/anurse/remy/blob/4f01c0e16ac9b8db1553d97588c9d8927884cef1/src/hw/mos6502/exec/store.rs#L158
            // I don't remember where I figured that behavior out then :(.
            var highByte = (address & 0xFF00) >> 8;
            var val = currentState.A & currentState.X & highByte;

            memory.WriteByte(address, (byte)val);
            return currentState;
        }

        private static Mos6502State Alr(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Anc(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State And(int address, Mos6502State currentState, MemoryUnit memory)
        {
            var value = memory.ReadByte(address);
            var r = (byte)(value & currentState.A);

            var flags = currentState.P;
            flags = flags.SetIf(r == 0, Mos6502Flags.Zero);
            flags = flags.SetIf((r & 0b1000_0000) != 0, Mos6502Flags.Negative);
            return currentState.With(a: r, p: flags);
        }

        private static Mos6502State Arr(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Asl(int address, Mos6502State currentState, MemoryUnit memory)
        {
            var operand = address == 0 ? currentState.A : memory.ReadByte(address);
            var r = operand << 1;

            var flags = currentState.P;

            flags = flags.SetIf(r > 0xFF, Mos6502Flags.Carry);
            r = (byte)r;

            // Set Zero flag **only** if the accumulator changes to zero.
            if (address == 0)
            {
                flags = flags.SetIf(r == 0, Mos6502Flags.Zero);
            }
            flags = flags.SetIf((r & 0b1000_0000) != 0, Mos6502Flags.Negative);

            if (address == 0)
            {
                return currentState.With(a: (byte)r, p: flags);
            }
            else
            {
                memory.WriteByte(address, (byte)r);
                return currentState.With(p: flags);
            }
        }

        private static Mos6502State Axs(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static InstructionExecutor BranchIf(bool set, Mos6502Flags flag)
        {
            return (address, currentState, _) =>
            {
                if (set == currentState.P.IsSet(flag))
                {
                    // The base cycle count has already been added to the clock
                    // +1 if the condition was true (it was, if we're here)
                    // +1 if the new address crosses a page boundary
                    var cycles = (address & 0xFF00) != (currentState.PC & 0xFF00) ? 2 : 1;

                    return currentState.With(pc: address, clock: currentState.Clock + cycles);
                }
                else
                {
                    return currentState;
                }
            };
        }

        private static Mos6502State Bit(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Brk(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Clc(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Cld(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Cli(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Clv(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Cmp(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Cpx(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Cpy(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Dcp(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Dec(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Dex(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Dey(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Eor(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Inc(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Inx(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Iny(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Isc(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Jmp(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Jsr(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Kil(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Las(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Lax(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Lda(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Ldx(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Ldy(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Lsr(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Nop(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Ora(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Pha(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Php(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Pla(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Plp(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Rla(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Rol(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Ror(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Rra(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Rti(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Rts(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Sax(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Sbc(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Sec(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Sed(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Sei(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Shx(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502State Shy(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502State Slo(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502State Sre(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502State Sta(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502State Stx(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502State Sty(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502State Tas(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502State Tax(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502State Tay(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502State Tsx(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502State Txa(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502State Txs(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502State Tya(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502State Xaa(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
    }
}