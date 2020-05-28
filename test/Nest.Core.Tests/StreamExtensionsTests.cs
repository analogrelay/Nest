using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Nest.Core.Tests
{
    public class StreamExtensionsTests
    {
        [Fact]
        public async Task SingleBuffer()
        {
            var data = Encoding.UTF8.GetBytes("Hello, World!");
            var stream = new MemoryStream(data);

            var buf = new byte[5];
            var read = await stream.ReadExactAsync(buf);
            Assert.Equal(5, read);
            Assert.Equal("Hello", Encoding.UTF8.GetString(buf));
        }

        [Fact]
        public async Task StreamTooShort()
        {
            var data = Encoding.UTF8.GetBytes("Hi");
            var stream = new MemoryStream(data);

            var buf = new byte[5];
            var ex = await Assert.ThrowsAsync<IOException>(() => stream.ReadExactAsync(buf));
            Assert.Equal("Stream does not contain enough data to fill the requested buffer.", ex.Message);
        }

        [Fact]
        public async Task StreamGivesOutSmallerBuffers()
        {
            var data = Encoding.UTF8.GetBytes("Hello, World!");
            var stream = new ByteAtATimeStream(new MemoryStream(data));

            var buf = new byte[5];
            var read = await stream.ReadExactAsync(buf);
            Assert.Equal(5, read);
            Assert.Equal("Hello", Encoding.UTF8.GetString(buf));
        }

        private class ByteAtATimeStream : Stream
        {
            private readonly Stream _inner;

            public ByteAtATimeStream(Stream inner)
            {
                _inner = inner;
            }

            public override bool CanRead => _inner.CanRead;

            public override bool CanSeek => _inner.CanSeek;

            public override bool CanWrite => _inner.CanWrite;

            public override long Length => _inner.Length;

            public override long Position { get => _inner.Position; set => _inner.Position = value; }

            public override void Flush()
            {
                _inner.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _inner.Read(buffer, 0, 1);
            }

            public override ValueTask<int> ReadAsync(Memory<byte> buffer, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
            {
                return _inner.ReadAsync(buffer.Slice(0, 1), cancellationToken);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _inner.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _inner.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _inner.Write(buffer, offset, count);
            }
        }
    }
}
