using System;
using Nest.Hardware;
using Nest.Memory;
using Xunit;

namespace Nest.Tests.Hardware
{
    public class Mos6502Tests
    {
        public class Adc
        {
            public class ImmediateMode
            {
                [Fact]
                public void NoCarry()
                {
                    var mem = CreateMemory((0x0000, new byte[] { 0x69, 0x42 }));
                    var state = Mos6502.State.PowerUp.With(a: 0x24);
                    var cpu = new Mos6502(state, mem);

                    // Run!
                    cpu.Step();

                    // Check the end state
                    Assert.Equal(0x66, cpu.CurrentState.A);
                }

                [Fact]
                public void WithCarryIn()
                {
                    var mem = CreateMemory((0x0000, new byte[] { 0x69, 0x42 }));
                    var state = Mos6502.State.PowerUp.With(a: 0x24, p: Mos6502.Flags.Carry);
                    var cpu = new Mos6502(state, mem);

                    // Run!
                    cpu.Step();

                    // Check the end state
                    Assert.Equal(0x67, cpu.CurrentState.A);
                    Assert.Equal(Mos6502.Flags.None, cpu.CurrentState.P & Mos6502.Flags.Carry);
                }

                [Fact]
                public void SetsCarryFlagWhenResultOverflows()
                {
                    var mem = CreateMemory((0x0000, new byte[] { 0x69, 0x04 }));
                    var state = Mos6502.State.PowerUp.With(a: 0xFD);
                    var cpu = new Mos6502(state, mem);

                    Assert.NotEqual(Mos6502.Flags.Carry, cpu.CurrentState.P & Mos6502.Flags.Carry);

                    // Run!
                    cpu.Step();

                    // Check the end state
                    Assert.Equal(0x01, cpu.CurrentState.A);
                    Assert.Equal(Mos6502.Flags.Carry, cpu.CurrentState.P & Mos6502.Flags.Carry);
                }

                [Fact]
                public void SetsZeroFlagWhenResultIsZero()
                {
                    var mem = CreateMemory((0x0000, new byte[] { 0x69, 0xFF }));
                    var state = Mos6502.State.PowerUp.With(a: 0x01);
                    var cpu = new Mos6502(state, mem);

                    Assert.Equal(Mos6502.Flags.None, cpu.CurrentState.P & Mos6502.Flags.Zero);

                    // Run!
                    cpu.Step();

                    // Check the end state
                    Assert.Equal(0x00, cpu.CurrentState.A);
                    Assert.Equal(Mos6502.Flags.Zero, cpu.CurrentState.P & Mos6502.Flags.Zero);
                }

                [Fact]
                public void SetsNegativeBitWhenResultIsNegative()
                {
                    var mem = CreateMemory((0x0000, new byte[] { 0x69, 0x7F }));
                    var state = Mos6502.State.PowerUp.With(a: 0x02);
                    var cpu = new Mos6502(state, mem);

                    Assert.Equal(Mos6502.Flags.None, cpu.CurrentState.P & Mos6502.Flags.Negative);

                    // Run!
                    cpu.Step();

                    // Check the end state
                    Assert.Equal(0x81, cpu.CurrentState.A);
                    Assert.Equal(Mos6502.Flags.Negative, cpu.CurrentState.P & Mos6502.Flags.Negative);
                }

                [Fact]
                public void SetsOverflowBitIfSignBitChangesDespiteBothValuesHavingSameSign()
                {
                    var mem = CreateMemory((0x0000, new byte[] { 0x69, 0x7B }));
                    var state = Mos6502.State.PowerUp.With(a: 0x2D);
                    var cpu = new Mos6502(state, mem);

                    Assert.Equal(Mos6502.Flags.None, cpu.CurrentState.P & Mos6502.Flags.Overflow);

                    // Run!
                    cpu.Step();

                    // Check the end state
                    Assert.Equal(0xA8, cpu.CurrentState.A);
                    Assert.Equal(Mos6502.Flags.Overflow, cpu.CurrentState.P & Mos6502.Flags.Overflow);
                }
            }
        }

        private static MemoryUnit CreateMemory(params (int Start, byte[] Data)[] memories)
        {
            var virt = new VirtualMemory();
            foreach (var memory in memories)
            {
                virt.Attach(memory.Start, new FixedMemory(memory.Data));
            }
            return virt;
        }
    }
}