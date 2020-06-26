using Nest.Memory;
using Xunit;

namespace Nest.Hardware.Mos6502
{
    public partial class Mos6502Tests
    {
        public class Adc
        {
            [Fact]
            public void NoCarry() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x69, 0x42)
                .WithInitialState(a: 0x24)
                .WithResultState(a: 0x66, pc: 0x02, clock: 2)
                .Run();

            [Fact]
            public void WithCarryIn() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x69, 0x42)
                .WithInitialState(a: 0x24, p: Mos6502Flags.PowerUp | Mos6502Flags.Carry)
                .WithResultState(a: 0x67, pc: 0x02, clock: 2)
                .Run();

            [Fact]
            public void SetsCarryFlagWhenResultOverflows() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x69, 0x04)
                .WithInitialState(a: 0xFD)
                .WithResultState(a: 0x01, pc: 0x02, p: Mos6502Flags.PowerUp | Mos6502Flags.Carry, clock: 2)
                .Run();

            [Fact]
            public void SetsZeroFlagWhenResultIsZero() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x69, 0xFF)
                .WithInitialState(a: 0x01)
                .WithResultState(a: 0x00, pc: 0x02, p: Mos6502Flags.PowerUp | Mos6502Flags.Zero | Mos6502Flags.Carry, clock: 2)
                .Run();

            [Fact]
            public void SetsNegativeBitWhenResultIsNegative() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x69, 0x7F)
                .WithInitialState(a: 0x02)
                .WithResultState(a: 0x81, pc: 0x02, p: Mos6502Flags.PowerUp | Mos6502Flags.Negative | Mos6502Flags.Overflow, clock: 2)
                .Run();

            [Fact]
            public void SetsOverflowBitIfSignBitChangesDespiteBothValuesHavingSameSign() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x69, 0x7B)
                .WithInitialState(a: 0x2D)
                .WithResultState(a: 0xA8, pc: 0x02, p: Mos6502Flags.PowerUp | Mos6502Flags.Overflow | Mos6502Flags.Negative, clock: 2)
                .Run();
        }

        public class And
        {
            [Fact]
            public void PerformsBitwiseAnd() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x29, 0b0000_1111)
                .WithInitialState(a: 0b1111_0101)
                .WithResultState(a: 0b0000_0101, pc: 0x02, clock: 2)
                .Run();

            [Fact]
            public void SetsZeroFlagIfResultIsZero() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x29, 0b0000_1111)
                .WithInitialState(a: 0b1111_0000)
                .WithResultState(a: 0x00, pc: 0x02, p: Mos6502Flags.PowerUp | Mos6502Flags.Zero, clock: 2)
                .Run();

            [Fact]
            public void SetsNegativeFlagIfBit7IsSet() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x29, 0b1010_1010)
                .WithInitialState(a: 0b1111_0000)
                .WithResultState(a: 0b1010_0000, pc: 0x02, p: Mos6502Flags.PowerUp | Mos6502Flags.Negative, clock: 2)
                .Run();
        }

        public class Asl
        {
            [Fact]
            public void ShiftsAccumulatorLeft() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x0A)
                .WithInitialState(a: 0b0000_1111)
                .WithResultState(a: 0b0001_1110, pc: 0x01, clock: 2)
                .Run();
                
            [Fact]
            public void ShiftsMemoryLeft() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x0E, 0x02, 0xD0)
                .WithMemory(0xD000, 0xAB, 0xCD, 0b0000_1111)
                .WithInitialState()
                .WithResultState(pc: 0x03, clock: 6)
                .ExpectWrite(0xD002, 0b0001_1110)
                .Run();

            [Fact]
            public void CarryOut() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x0A)
                .WithInitialState(a: 0b1011_0000)
                .WithResultState(a: 0b0110_0000, pc: 0x01, p: Mos6502Flags.PowerUp | Mos6502Flags.Carry, clock: 2)
                .Run();

            [Fact]
            public void SetsZeroFlagIfAccumulatorBecomesZero() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x0A)
                .WithInitialState(a: 0b1000_0000)
                .WithResultState(a: 0x00, pc: 0x01, p: Mos6502Flags.PowerUp | Mos6502Flags.Zero | Mos6502Flags.Carry, clock: 2)
                .Run();

            [Fact]
            public void DoesNotSetZeroFlagIfResultIsZeroButTargetIsNotAccumulator() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x0E, 0x02, 0xD0)
                .WithMemory(0xD000, 0xAB, 0xCD, 0b1000_0000)
                .WithInitialState()
                .WithResultState(pc: 0x03, p: Mos6502Flags.PowerUp | Mos6502Flags.Carry, clock: 6)
                .ExpectWrite(0xD002, 0x00)
                .Run();

            [Fact]
            public void SetsNegativeFlagIfResultIsNegative() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x0E, 0x02, 0xD0)
                .WithMemory(0xD000, 0xAB, 0xCD, 0b0100_0000)
                .WithInitialState()
                .WithResultState(pc: 0x03, p: Mos6502Flags.PowerUp | Mos6502Flags.Negative, clock: 6)
                .ExpectWrite(0xD002, 0b1000_0000)
                .Run();
        }

        public class Bit
        {
            [Fact]
            public void SetsZeroIfNoBitsInArgumentMatchA() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x24, 0x10)
                .WithMemory(0x0010, 0b0101_0101)
                .WithInitialState(a: 0b1010_1010)
                .WithResultState(a: 0b1010_1010, pc: 0x02, p: Mos6502Flags.PowerUp | Mos6502Flags.Zero, clock: 3)
                .Run();

            [Fact]
            public void OverflowIsSetToBit6OfResult() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x24, 0x10)
                .WithMemory(0x0010, 0b0100_0000)
                .WithInitialState(a: 0b0100_0000)
                .WithResultState(a: 0b0100_0000, pc: 0x02, p: Mos6502Flags.PowerUp | Mos6502Flags.Overflow, clock: 3)
                .Run();

            [Fact]
            public void NegativeIsSetToBit7OfResult() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x24, 0x10)
                .WithMemory(0x0010, 0b1000_0000)
                .WithInitialState(a: 0b1000_0000)
                .WithResultState(a: 0b1000_0000, pc: 0x02, p: Mos6502Flags.PowerUp | Mos6502Flags.Negative, clock: 3)
                .Run();
        }
    }
}