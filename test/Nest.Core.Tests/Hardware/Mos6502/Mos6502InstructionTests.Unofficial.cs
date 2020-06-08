
using Nest.Memory;
using Xunit;

namespace Nest.Hardware.Mos6502
{
    public partial class Mos6502Tests
    {
        // Unofficial opcode
        public class Ahx
        {
            [Fact]
            public void SetsOperandToAAndXAndHighBitOfOwnAddress() => new Mos6502TestBuilder()
                .WithMemory(0x0000, 0x9F, 0x01, 0x3C)
                .WithMemory(0x3C00, new FixedMemory(10))
                .WithInitialState(a: 0x3F, x: 0xF0)
                .WithResultState(a: 0x3F, x: 0xF0, pc: 0x03)
                .ExpectWrite(0x3C01, 0x30)
                .Run();
        }
    }
}