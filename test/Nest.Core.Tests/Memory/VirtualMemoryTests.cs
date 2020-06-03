using System;
using System.Linq;
using Xunit;

namespace Nest.Memory
{
    public class VirtualMemoryTests
    {
        [Fact]
        public void InitialLengthIsZero()
        {
            var mem = new VirtualMemory();
            Assert.Equal(0, mem.Length);
        }

        [Fact]
        public void AttachAndDetachUpdateLength()
        {
            var mem = new VirtualMemory();
            var ram1 = new FixedMemory(10);
            var ram2 = new FixedMemory(5);
            mem.Attach(10, ram1);
            mem.Attach(20, ram2);

            Assert.Equal(25, mem.Length);

            mem.Detach(ram2);

            Assert.Equal(20, mem.Length);
        }

        [Fact]
        public void AttachThrowsIfMemoriesWouldOverlap()
        {
            var mem = new VirtualMemory();
            var ram1 = new FixedMemory(10);
            var ram2 = new FixedMemory(5);
            mem.Attach(10, ram1);

            // Dangling off the end
            Assert.Throws<InvalidOperationException>(() => mem.Attach(18, ram2));

            // Fully contained within
            Assert.Throws<InvalidOperationException>(() => mem.Attach(11, ram2));

            // Dangling off the start
            Assert.Throws<InvalidOperationException>(() => mem.Attach(9, ram2));
        }

        [Fact]
        public void ReadOutsideOfAnyMemoryProducesAllZeros()
        {
            var mem = new VirtualMemory();
            var ram1 = CreateFixedMemory(5);
            var ram2 = CreateFixedMemory(5);
            mem.Attach(0, ram1);
            mem.Attach(20, ram2);

            var buf = new byte[5];
            mem.Read(10, buf);
            Assert.True(buf.All(b => b == 0));
        }

        [Fact]
        public void ReadContainedInOneMemory()
        {
            var mem = new VirtualMemory();
            var ram1 = CreateFixedMemory(10);
            mem.Attach(0, ram1);

            var buf = new byte[5];
            mem.Read(1, buf);
            Assert.Equal(new byte[] { 2, 3, 4, 5, 6 }, buf);
        }

        [Fact]
        public void ReadSpanningTwoAdjacentMemories()
        {
            var mem = new VirtualMemory();
            var ram1 = CreateFixedMemory(10);
            var ram2 = CreateFixedMemory(10);
            mem.Attach(0, ram1);
            mem.Attach(10, ram2);

            var buf = new byte[5];
            mem.Read(8, buf);
            Assert.Equal(new byte[] { 9, 10, 1, 2, 3 }, buf);
        }

        [Fact]
        public void ReadSpanningTwoMemoriesWithAGap()
        {
            var mem = new VirtualMemory();
            var ram1 = CreateFixedMemory(10);
            var ram2 = CreateFixedMemory(10);
            mem.Attach(0, ram1);
            mem.Attach(15, ram2);

            var buf = new byte[15];
            mem.Read(5, buf);
            Assert.Equal(new byte[] {
                6, 7, 8, 9, 10,
                0, 0, 0, 0, 0,
                1, 2, 3, 4, 5
            }, buf);
        }

        [Fact]
        public void ReadThatGoesOverTheEndThrows()
        {
            var mem = new VirtualMemory();
            var buf = new byte[20];

            Assert.Throws<IndexOutOfRangeException>(() => mem.Read(0, buf));

            mem.Attach(0, new FixedMemory(5));
            mem.Attach(5, new FixedMemory(5));

            Assert.Throws<IndexOutOfRangeException>(() => mem.Read(0, buf));
            Assert.Throws<IndexOutOfRangeException>(() => mem.Read(7, buf));
            Assert.Throws<IndexOutOfRangeException>(() => mem.Read(15, buf));
        }

        [Fact]
        public void ReadDoesNotIncludeDetachedMemory()
        {
            var mem = new VirtualMemory();
            var ram1 = CreateFixedMemory(10);
            var ram2 = CreateFixedMemory(10);
            mem.Attach(0, ram1);
            mem.Attach(15, ram2);
            mem.Detach(ram1);

            var buf = new byte[15];
            mem.Read(5, buf);
            Assert.Equal(new byte[] {
                0, 0, 0, 0, 0,
                0, 0, 0, 0, 0,
                1, 2, 3, 4, 5
            }, buf);
        }

        [Fact]
        public void WriteOutsideOfAnyMemoryThrows()
        {
            var mem = new VirtualMemory();
            var ram1 = new FixedMemory(5);
            var ram2 = new FixedMemory(5);
            mem.Attach(0, ram1);
            mem.Attach(20, ram2);

            var buf = CreateData(5);
            Assert.Throws<InvalidOperationException>(() => mem.Write(10, buf.Span));
        }

        [Fact]
        public void WriteContainedInOneMemory()
        {
            var mem = new VirtualMemory();
            var ram1 = new FixedMemory(10);
            mem.Attach(0, ram1);

            var buf = CreateData(5);
            mem.Write(1, buf.Span);
            Assert.Equal(new byte[] { 0, 1, 2, 3, 4, 5, 0, 0, 0, 0 }, ram1.Data.ToArray());
        }

        [Fact]
        public void WriteSpanningTwoAdjacentMemories()
        {
            var mem = new VirtualMemory();
            var ram1 = new FixedMemory(10);
            var ram2 = new FixedMemory(10);
            mem.Attach(0, ram1);
            mem.Attach(10, ram2);

            var buf = CreateData(10);
            mem.Write(5, buf.Span);
            Assert.Equal(
                new byte[] { 0, 0, 0, 0, 0, 1, 2, 3, 4, 5 },
                ram1.Data.ToArray());
            Assert.Equal(
                new byte[] { 6, 7, 8, 9, 10, 0, 0, 0, 0, 0 },
                ram2.Data.ToArray());
        }

        [Fact]
        public void WriteSpanningTwoMemoriesWithAGapThrowsButDoesWriteEverythingBeforeTheGap()
        {
            var mem = new VirtualMemory();
            var ram1 = new FixedMemory(10);
            var ram2 = new FixedMemory(10);
            mem.Attach(0, ram1);
            mem.Attach(15, ram2);

            var buf = CreateData(15);
            Assert.Throws<InvalidOperationException>(() => mem.Write(5, buf.Span));

            // Part of our contract is that we WILL write everything we could before the gap!
            Assert.Equal(
                new byte[] { 0, 0, 0, 0, 0, 1, 2, 3, 4, 5 },
                ram1.Data.ToArray()
            );

            // But not after the gap
            Assert.Equal(
                new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                ram2.Data.ToArray()
            );
        }

        [Fact]
        public void WriteThatGoesOverTheEndThrows()
        {
            var mem = new VirtualMemory();
            var buf = CreateData(20);

            Assert.Throws<IndexOutOfRangeException>(() => mem.Write(0, buf.Span));

            mem.Attach(0, new FixedMemory(5));
            mem.Attach(5, new FixedMemory(5));

            Assert.Throws<IndexOutOfRangeException>(() => mem.Write(0, buf.Span));
            Assert.Throws<IndexOutOfRangeException>(() => mem.Write(7, buf.Span));
            Assert.Throws<IndexOutOfRangeException>(() => mem.Write(15, buf.Span));
        }

        [Fact]
        public void WriteOverDetachedMemoryThrows()
        {
            var mem = new VirtualMemory();
            var ram1 = CreateFixedMemory(10);
            var ram2 = CreateFixedMemory(10);
            mem.Attach(0, ram1);
            mem.Attach(15, ram2);
            mem.Detach(ram1);

            var buf = CreateData(15);
            Assert.Throws<InvalidOperationException>(() => mem.Write(5, buf.Span));
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