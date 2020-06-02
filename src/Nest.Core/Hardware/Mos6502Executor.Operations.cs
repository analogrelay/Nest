using System;
using Nest.Memory;

namespace Nest.Hardware
{
    public static partial class Mos6502Executor
    {
        private static Mos6502.State Adc(int address, Mos6502.State currentState, MemoryUnit memory)
        {
            // Read the value
            var value = memory.ReadByte(address);

            // Add it to the accumulator and return
            var newA = currentState.A + value;
            if ((currentState.P & Mos6502.Flags.Carry) != Mos6502.Flags.None)
            {
                newA += 1;
            }

            var newP = currentState.P;
            if (newA > 0xFF)
            {
                newA = (byte)newA;
                newP |= Mos6502.Flags.Carry;
            }
            else
            {
                newP ^= Mos6502.Flags.Carry;
            }

            if (newA == 0)
            {
                newP |= Mos6502.Flags.Zero;
            }

            if ((newA & 0b1000_0000) != 0)
            {
                newP |= Mos6502.Flags.Negative;
            }

            return currentState.With(a: newA, p: newP);
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