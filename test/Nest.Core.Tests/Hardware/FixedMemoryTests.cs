using System;
using System.Linq;
using Nest.Hardware;
using Xunit;

namespace Nest.Tests.Hardware
{
    public class FixedMemoryTests
    {
        [Fact]
        public void InitializesWithAllZeros()
        {
            var mem = new FixedMemory(10);

            var buf = new byte[10];
            mem.Read(0, buf);
            Assert.True(buf.All(b => b == 0));
        }

        [Fact]
        public void CanBeInitializedWithData()
        {
            var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var mem = new FixedMemory(data);

            var buf = new byte[10];
            mem.Read(0, buf);
            Assert.Equal(data, buf);
        }

        [Fact]
        public void ReadThrowsIfWouldGoOutOfBounds()
        {
            var mem = new FixedMemory(10);
            var buf = new byte[20];
            Assert.Throws<IndexOutOfRangeException>(() => mem.Read(0, buf));
        }

        [Fact]
        public void ReadThrowsIfOffsetIsNegative()
        {
            var mem = new FixedMemory(10);
            var buf = new byte[5];
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => mem.Read(-1, buf));
            Assert.Equal("offset", ex.ParamName);
        }

        [Fact]
        public void WriteUpdatesData()
        {
            var mem = new FixedMemory(10);
            var buf = new byte[5] { 1, 2, 3, 4, 5 };
            mem.Write(2, buf.AsSpan());

            Assert.Equal(new byte[] { 0, 0, 1, 2, 3, 4, 5, 0, 0, 0 }, mem.Data.ToArray());
        }

        [Fact]
        public void WriteThrowsIfWouldGoOutOfBounds()
        {
            var mem = new FixedMemory(10);
            var buf = new byte[20];
            Assert.Throws<IndexOutOfRangeException>(() => mem.Write(0, buf));
        }

        [Fact]
        public void WriteThrowsIfOffsetIsNegative()
        {
            var mem = new FixedMemory(10);
            var buf = new byte[5];
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => mem.Write(-1, buf));
            Assert.Equal("offset", ex.ParamName);
        }

        [Fact]
        public void WriteThrowsIfReadOnly()
        {
            var mem = new FixedMemory(10, canRead: true, canWrite: false);
            var buf = new byte[5] { 1, 2, 3, 4, 5 };
            Assert.Throws<InvalidOperationException>(() => mem.Write(0, buf));
        }

        [Fact]
        public void ReadThrowsIfWriteOnly()
        {
            var mem = new FixedMemory(10, canRead: false, canWrite: true);
            var buf = new byte[5];
            Assert.Throws<InvalidOperationException>(() => mem.Read(0, buf));
        }
    }
}