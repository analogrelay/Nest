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

            var newP = currentState.P;

            // Carry out if r overflows
            newP = r > 0xFF ? newP | Mos6502Flags.Carry : newP & ~Mos6502Flags.Carry;
            r = (byte)r;

            newP = r == 0 ? newP | Mos6502Flags.Zero : newP & ~Mos6502Flags.Zero;
            newP = (r & 0b1000_0000) != 0 ? newP | Mos6502Flags.Negative : newP & ~Mos6502Flags.Negative;

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
                newP | Mos6502Flags.Overflow :
                newP & ~Mos6502Flags.Overflow;

            return currentState.With(a: r, p: newP);
        }

        private static Mos6502State Ahx(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        private static Mos6502State Arr(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Asl(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Axs(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Bcc(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Bcs(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Beq(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Bit(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Bmi(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Bne(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Bpl(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Brk(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Bvc(int address, Mos6502State currentState, MemoryUnit memory)
        {
            throw new NotImplementedException();
        }

        private static Mos6502State Bvs(int address, Mos6502State currentState, MemoryUnit memory)
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