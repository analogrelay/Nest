using Microsoft.Extensions.Logging.Abstractions;
using Nest.Memory;
using Xunit;

namespace Nest.Hardware.Mos6502
{
    public class Mos6502ExecutorTests
    {
        [Fact]
        public void ResolveImmediateAddress()
        {
            var memory = new FixedMemory(new byte[] { 0x00, 0x01, 0x02, 0x03 });
            var state = Mos6502State.PowerUp.With(pc: 0x01);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.Immediate, in state, memory, NullLogger.Instance);

            Assert.Equal(0x02, address);
            Assert.False(crossesBoundary);
        }

        [Fact]
        public void ResolveZeroPageAddress()
        {
            var memory = new FixedMemory(new byte[] { 0x00, 0x0A, 0x0B });
            var state = Mos6502State.PowerUp.With(pc: 0x00);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.ZeroPage, in state, memory, NullLogger.Instance);

            Assert.Equal(0x000A, address);
            Assert.False(crossesBoundary);
        }

        [Fact]
        public void ResolveZeroPageAddressIndexedByX()
        {
            var memory = new FixedMemory(new byte[] { 0x00, 0x0A, 0x0B });
            var state = Mos6502State.PowerUp.With(pc: 0x00, x: 0x10);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.ZeroPageX, in state, memory, NullLogger.Instance);

            Assert.Equal(0x001A, address);
            Assert.False(crossesBoundary);
        }

        [Theory]
        [InlineData(0x01, 0x07)]
        [InlineData(0xFF, 0x05)]
        public void ResolveRelativeAddress(byte offset, ushort expectedAddress) {
            var memory = new FixedMemory(new byte[] { 0xCD, 0xCD, 0xCD, 0xCD, 0x00, offset });
            var state = Mos6502State.PowerUp.With(pc: 0x04);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.Relative, in state, memory, NullLogger.Instance);

            Assert.Equal(expectedAddress, address);
            Assert.False(crossesBoundary);
        }

        [Fact]
        public void ResolveAbsoluteAddress()
        {
            var memory = new FixedMemory(new byte[] { 0x00, 0x0A, 0x0B });
            var state = Mos6502State.PowerUp.With(pc: 0x00);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.Absolute, in state, memory, NullLogger.Instance);

            Assert.Equal(0x0B0A, address);
            Assert.False(crossesBoundary);
        }

        [Fact]
        public void ResolveAbsoluteAddressIndexedByX()
        {
            var memory = new FixedMemory(new byte[] { 0x00, 0x0A, 0x0B });
            var state = Mos6502State.PowerUp.With(pc: 0x00, x: 0x10);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.AbsoluteX, in state, memory, NullLogger.Instance);

            Assert.Equal(0x0B1A, address);
            Assert.False(crossesBoundary);
        }

        [Fact]
        public void ResolveAbsoluteAddressIndexedByY()
        {
            var memory = new FixedMemory(new byte[] { 0x00, 0x0A, 0x0B });
            var state = Mos6502State.PowerUp.With(pc: 0x00, y: 0x10);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.AbsoluteY, in state, memory, NullLogger.Instance);

            Assert.Equal(0x0B1A, address);
            Assert.False(crossesBoundary);
        }

        [Fact]
        public void IndicatesIfAPageBoundaryIsCrossedByIndexingOnX()
        {
            var memory = new FixedMemory(new byte[] { 0x00, 0xFF, 0x0B });
            var state = Mos6502State.PowerUp.With(pc: 0x00, x: 0x10);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.AbsoluteX, in state, memory, NullLogger.Instance);

            Assert.Equal(0x0C0F, address);
            Assert.True(crossesBoundary);
        }

        [Fact]
        public void IndicatesIfAPageBoundaryIsCrossedByIndexingOnY()
        {
            var memory = new FixedMemory(new byte[] { 0x00, 0xFF, 0x0B });
            var state = Mos6502State.PowerUp.With(pc: 0x00, y: 0x10);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.AbsoluteY, in state, memory, NullLogger.Instance);

            Assert.Equal(0x0C0F, address);
            Assert.True(crossesBoundary);
        }
    }
}