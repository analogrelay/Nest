using System;
using Nest.Memory;

namespace Nest.Hardware
{
    public static partial class Mos6502Executor
    {
        private static Mos6502.State Adc(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            // ADC: A = A + M + P[Carry]

            var m = memory.ReadByte(address);
            var r = currentState.A + m + (int)(currentState.P & Mos6502.Flags.Carry);

            var newP = currentState.P;

            // Carry out if r overflows
            newP = r > 0xFF ? newP | Mos6502.Flags.Carry : newP & ~Mos6502.Flags.Carry;
            newP = r == 0 ? newP | Mos6502.Flags.Zero : newP & ~Mos6502.Flags.Zero;
            newP = (r & 0b1000_0000) != 0 ? newP | Mos6502.Flags.Negative : newP & ~Mos6502.Flags.Negative;

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
            newP = ((currentState.A ^ m) & 0x80) == 0 && ((currentState.A ^ r) & 0x80) == 0x80 ?
                newP | Mos6502.Flags.Overflow :
                newP & ~Mos6502.Flags.Overflow;

            return currentState.With(a: (byte)r, p: newP);
        }

        private static Mos6502.State Ahx(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Alr(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Anc(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State And(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Arr(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Asl(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Axs(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Bcc(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Bcs(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Beq(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Bit(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Bmi(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Bne(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Bpl(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Brk(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Bvc(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Bvs(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Clc(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Cld(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Cli(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Clv(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Cmp(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Cpx(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Cpy(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Dcp(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Dec(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Dex(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Dey(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Eor(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Inc(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Inx(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Iny(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Isc(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Jmp(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Jsr(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Kil(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Las(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Lax(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Lda(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Ldx(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Ldy(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Lsr(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Nop(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Ora(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Pha(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Php(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Pla(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Plp(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Rla(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Rol(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Ror(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Rra(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Rti(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Rts(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Sax(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Sbc(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Sec(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Sed(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Sei(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502.State Shx(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502.State Shy(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502.State Slo(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502.State Sre(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502.State Sta(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502.State Stx(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502.State Sty(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502.State Tas(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502.State Tax(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502.State Tay(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502.State Tsx(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502.State Txa(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502.State Txs(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502.State Tya(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
        private static Mos6502.State Xaa(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }
    }
}