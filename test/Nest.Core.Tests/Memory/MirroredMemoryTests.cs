using System;
using Nest.Memory;
using Xunit;

namespace Nest.Tests.Memory
{
    public class MirroredMemoryTests
    {
        [Fact]
        public void ReadThrowsIfWouldGoOutOfBounds()
        {
            var ram = CreateFixedMemory(10);
            var mem = new MirroredMemory(50, ram);
            var buf = new byte[5];

            Assert.Throws<IndexOutOfRangeException>(() => mem.Read(49, buf));
        }

        [Fact]
        public void ReadThrowsIfOffsetIsNegative()
        {
            var mem = new MirroredMemory(10, new FixedMemory(10));
            var buf = new byte[5];
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => mem.Read(-1, buf));
            Assert.Equal("offset", ex.ParamName);
        }

        [Fact]
        public void ReadMirrorsAsManyTimesAsNecessaryToFillRead()
        {
            var ram = CreateFixedMemory(5);
            var mem = new MirroredMemory(15, ram);
            var buf = new byte[10];

            mem.Read(3, buf);

            Assert.Equal(
                new byte[] { 4, 5, 1, 2, 3, 4, 5, 1, 2, 3 },
                buf);
        }

        [Fact]
        public void WriteThrowsIfWouldGoOutOfBounds()
        {
            var mem = new MirroredMemory(10, new FixedMemory(10));
            var buf = new byte[5];
            Assert.Throws<IndexOutOfRangeException>(() => mem.Write(9, buf));
        }

        [Fact]
        public void WriteThrowsIfOffsetIsNegative()
        {
            var mem = new MirroredMemory(10, new FixedMemory(10));
            var buf = new byte[5];
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => mem.Write(-1, buf));
            Assert.Equal("offset", ex.ParamName);
        }

        [Fact]
        public void WriteMirrorsAsManyTimesAsNecessaryToFillWrite()
        {
            var ram = new FixedMemory(5);
            var mem = new MirroredMemory(20, ram);
            var buf = CreateData(15);

            mem.Write(3, buf.Span);

            Assert.Equal(
                new byte[] { 13, 14, 15, 11, 12 },
                ram.Data.ToArray());
        }

        private Memory<byte> CreateData(byte size)
        {
            var buf = new byte[size];
            for (int i = 0; i < size; i++)
            {
                buf[i] = (byte)(i + 1);
            }
            return buf;
        }

        private FixedMemory CreateFixedMemory(byte size)
        {
            return new FixedMemory(CreateData(size));
        }
    }
}