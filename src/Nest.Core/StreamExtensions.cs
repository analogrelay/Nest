using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nest
{
    internal static class StreamExtensions
    {
        /// <summary>
        /// Reads enough data from the stream to fill the buffer. Throws if the stream ends before the buffer can be filled.
        /// </summary>
        public static async Task<int> ReadExactAsync(this Stream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            var totalRead = 0;
            while (buffer.Length > 0)
            {
                var read = await stream.ReadAsync(buffer, cancellationToken);
                if (read == 0)
                {
                    throw new IOException("Stream does not contain enough data to fill the requested buffer.");
                }
                totalRead += read;
                buffer = buffer.Slice(read);
            }
            return totalRead;
        }
    }
}