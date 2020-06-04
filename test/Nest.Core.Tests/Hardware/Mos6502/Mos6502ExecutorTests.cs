using Microsoft.Extensions.Logging.Abstractions;
using Nest.Memory;
using Xunit;

namespace Nest.Hardware.Mos6502
{
    public class Mos6502ExecutorTests
    {
        // Reminder: The PC points to where the *instruction* starts, not the address.

        [Fact]
        public void ResolveImmediateAddress()
        {
            var memory = new FixedMemory(new byte[] { 0x00, 0x01, 0x02, 0x03 });
            var state = Mos6502State.PowerUp.With(pc: 0x01);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.Immediate, in state, memory);

            Assert.Equal(0x02, address);
            Assert.False(crossesBoundary);
        }

        [Fact]
        public void ResolveZeroPageAddress()
        {
            var memory = new FixedMemory(new byte[] { 0x00, 0x0A, 0x0B });
            var state = Mos6502State.PowerUp.With(pc: 0x00);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.ZeroPage, in state, memory);

            Assert.Equal(0x000A, address);
            Assert.False(crossesBoundary);
        }

        [Fact]
        public void ResolveZeroPageAddressIndexedByX()
        {
            var memory = new FixedMemory(new byte[] { 0x00, 0x0A, 0x0B });
            var state = Mos6502State.PowerUp.With(pc: 0x00, x: 0x10);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.ZeroPageX, in state, memory);

            Assert.Equal(0x001A, address);
            Assert.False(crossesBoundary);
        }

        [Theory]
        [InlineData(0x01, 0x07)]
        [InlineData(0xFF, 0x05)]
        public void ResolveRelativeAddress(byte offset, ushort expectedAddress)
        {
            var memory = new FixedMemory(new byte[] { 0xCD, 0xCD, 0xCD, 0xCD, 0x00, offset });
            var state = Mos6502State.PowerUp.With(pc: 0x04);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.Relative, in state, memory);

            Assert.Equal(expectedAddress, address);
            Assert.False(crossesBoundary);
        }

        [Fact]
        public void ResolveAbsoluteAddress()
        {
            var memory = new FixedMemory(new byte[] { 0x00, 0x0A, 0x0B });
            var state = Mos6502State.PowerUp.With(pc: 0x00);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.Absolute, in state, memory);

            Assert.Equal(0x0B0A, address);
            Assert.False(crossesBoundary);
        }

        [Fact]
        public void ResolveAbsoluteAddressIndexedByX()
        {
            var memory = new FixedMemory(new byte[] { 0x00, 0x0A, 0x0B });
            var state = Mos6502State.PowerUp.With(pc: 0x00, x: 0x10);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.AbsoluteX, in state, memory);

            Assert.Equal(0x0B1A, address);
            Assert.False(crossesBoundary);
        }

        [Fact]
        public void ResolveAbsoluteAddressIndexedByY()
        {
            var memory = new FixedMemory(new byte[] { 0x00, 0x0A, 0x0B });
            var state = Mos6502State.PowerUp.With(pc: 0x00, y: 0x10);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.AbsoluteY, in state, memory);

            Assert.Equal(0x0B1A, address);
            Assert.False(crossesBoundary);
        }

        [Fact]
        public void IndicatesIfAPageBoundaryIsCrossedByAbsoluteIndexingOnX()
        {
            var memory = new FixedMemory(new byte[] { 0x00, 0xFF, 0x0B });
            var state = Mos6502State.PowerUp.With(pc: 0x00, x: 0x10);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.AbsoluteX, in state, memory);

            Assert.Equal(0x0C0F, address);
            Assert.True(crossesBoundary);
        }

        [Fact]
        public void IndicatesIfAPageBoundaryIsCrossedByAbsoluteIndexingOnY()
        {
            var memory = new FixedMemory(new byte[] { 0x00, 0xFF, 0x0B });
            var state = Mos6502State.PowerUp.With(pc: 0x00, y: 0x10);
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.AbsoluteY, in state, memory);

            Assert.Equal(0x0C0F, address);
            Assert.True(crossesBoundary);
        }

        [Fact]
        public void ResolvesIndirectAddress()
        {
            var memory = new VirtualMemory();
            var state = Mos6502State.PowerUp.With(pc: 0x01);
            memory.Attach(0x0000, new byte[] { 0xAB, 0xAB, 0x04, 0x10 });
            memory.Attach(0x1000, new byte[] { 0xAB, 0xAB, 0xAB, 0xAB, 0xEF, 0xBE });
            var (address, _) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.Indirect, in state, memory);

            Assert.Equal(0xBEEF, address);
        }

        [Fact]
        public void ResolvesIndexedIndirectAddress()
        {
            var memory = new VirtualMemory();
            var state = Mos6502State.PowerUp.With(pc: 0x01, x: 0x02);
            memory.Attach(0x0000, new byte[] { 0xAB, 0xAB, 0x10 });
            memory.Attach(0x0010, new byte[] { 0xAB, 0xAB, 0xEF, 0xBE });
            var (address, _) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.IndexedIndirect, in state, memory);

            Assert.Equal(0xBEEF, address);
        }

        [Fact]
        public void ResolvesIndirectIndexedAddress()
        {
            var memory = new VirtualMemory();
            var state = Mos6502State.PowerUp.With(pc: 0x01, y: 0x02);
            memory.Attach(0x0000, new byte[] { 0xAB, 0xAB, 0x12 });
            memory.Attach(0x0010, new byte[] { 0xAB, 0xAB, 0x10, 0x10 });
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.IndirectIndexed, in state, memory);

            Assert.Equal(0x1012, address);
            Assert.False(crossesBoundary);
        }

        [Fact]
        public void IndicatesIfAPageBoundaryIsCrossedByIndirectIndexing()
        {
            var memory = new VirtualMemory();
            var state = Mos6502State.PowerUp.With(pc: 0x01, y: 0x01);
            memory.Attach(0x0000, new byte[] { 0xAB, 0xAB, 0x12 });
            memory.Attach(0x0010, new byte[] { 0xAB, 0xAB, 0xFF, 0x1F });
            var (address, crossesBoundary) = Mos6502Executor.ResolveAddress(Mos6502AddressingMode.IndirectIndexed, in state, memory);

            Assert.Equal(0x2000, address);
            Assert.True(crossesBoundary);
        }
    }
}