using System.Collections.Generic;
using Nest.Hardware;
using Xunit;

namespace Nest.Hardware.Mos6502
{
    public class Mos6502Tests
    {
        public class Adc
        {
            [Fact]
            public void NoCarry() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x69, 0x42)
                .WithInitialState(a: 0x24)
                .WithResultState(a: 0x66)
                .Run();

            [Fact]
            public void WithCarryIn() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x69, 0x42)
                .WithInitialState(a: 0x24, p: Mos6502Flags.PowerUp | Mos6502Flags.Carry)
                .WithResultState(a: 0x67)
                .Run();

            [Fact]
            public void SetsCarryFlagWhenResultOverflows() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x69, 0x04)
                .WithInitialState(a: 0xFD)
                .WithResultState(a: 0x01, p: Mos6502Flags.PowerUp | Mos6502Flags.Carry)
                .Run();

            [Fact]
            public void SetsZeroFlagWhenResultIsZero() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x69, 0xFF)
                .WithInitialState(a: 0x01)
                .WithResultState(a: 0x00, p: Mos6502Flags.PowerUp | Mos6502Flags.Zero | Mos6502Flags.Carry)
                .Run();

            [Fact]
            public void SetsNegativeBitWhenResultIsNegative() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x69, 0x7F)
                .WithInitialState(a: 0x02)
                .WithResultState(a: 0x81, p: Mos6502Flags.PowerUp | Mos6502Flags.Negative | Mos6502Flags.Overflow)
                .Run();

            [Fact]
            public void SetsOverflowBitIfSignBitChangesDespiteBothValuesHavingSameSign() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x69, 0x7B)
                .WithInitialState(a: 0x2D)
                .WithResultState(a: 0xA8, p: Mos6502Flags.PowerUp | Mos6502Flags.Overflow | Mos6502Flags.Negative)
                .Run();
        }
    }
}