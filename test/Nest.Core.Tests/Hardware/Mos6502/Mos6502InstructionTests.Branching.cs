using Xunit;

namespace Nest.Hardware.Mos6502
{
    public partial class Mos6502Tests
    {
        public class Bcc
        {
            [Fact]
            public void AdvancesPCAsNormalIfCarryBitSet() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x90, 0x42)
                .WithInitialState(p: Mos6502Flags.Carry)
                .WithResultState(pc: 0x02, p: Mos6502Flags.Carry)
                .Run();

            [Fact]
            public void AdvancesPCByOffsetIfCarryBitClear() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x90, 0x42)
                .WithResultState(pc: 0x44)
                .Run();
        }
    }
}