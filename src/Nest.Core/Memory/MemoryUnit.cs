using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nest.Memory
{
    public abstract class MemoryUnit
    {
        public abstract int Length { get; }

        public abstract void Read(int offset, Span<byte> buffer);
        public abstract void Write(int offset, ReadOnlySpan<byte> buffer);
    }

    public class FixedMemory : MemoryUnit
    {
        private readonly Memory<byte> _content;
        private readonly ILogger _logger;

        public override int Length => _content.Length;
        public Span<byte> Data => _content.Span;
        public bool CanRead { get; }
        public bool CanWrite { get; }

        public FixedMemory(int size) : this(NullLogger.Instance, new byte[size], true, true)
        {
        }

        public FixedMemory(int size, bool canRead, bool canWrite) : this(NullLogger.Instance, new byte[size], canRead, canWrite)
        {
        }

        public FixedMemory(Memory<byte> initialContent) : this(NullLogger.Instance, initialContent, true, true)
        {
        }

        public FixedMemory(Memory<byte> initialContent, bool canRead, bool canWrite): this(NullLogger.Instance, initialContent, canRead, canWrite)
        {
        }

        public FixedMemory(ILogger logger, int size) : this(logger, new byte[size], true, true)
        {
        }

        public FixedMemory(ILogger logger, int size, bool canRead, bool canWrite) : this(logger, new byte[size], canRead, canWrite)
        {
        }

        public FixedMemory(ILogger logger, Memory<byte> initialContent) : this(logger, initialContent, true, true)
        {
        }

        public FixedMemory(ILogger logger, Memory<byte> initialContent, bool canRead, bool canWrite)
        {
            _logger = logger;
            _content = initialContent;
            CanRead = canRead;
            CanWrite = canWrite;
        }

        public override void Read(int offset, Span<byte> buffer)
        {
            if (!CanRead)
            {
                throw new InvalidOperationException("Reading from this memory is not permitted.");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset must be non-negative.");
            }

            if (offset + buffer.Length > _content.Length)
            {
                throw new IndexOutOfRangeException("The read goes out of the bounds of the memory.");
            }

            _logger.LogTrace(new EventId(0, "Reading"), "Reading ${Start:X4}-${End:X4} ({Length} bytes)", offset, offset + buffer.Length, buffer.Length);
            _content.Slice(offset, buffer.Length).Span.CopyTo(buffer);
        }

        public override void Write(int offset, ReadOnlySpan<byte> buffer)
        {
            if (!CanWrite)
            {
                throw new InvalidOperationException("Writing to this memory is not permitted.");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset must be non-negative.");
            }

            if (offset + buffer.Length > _content.Length)
            {
                throw new IndexOutOfRangeException("The write goes out of the bounds of the memory.");
            }

            _logger.LogTrace(new EventId(0, "Writing"), "Writing ${Start:X4}-${End:X4} ({Length} bytes)", offset, offset + buffer.Length, buffer.Length);
            buffer.CopyTo(_content.Slice(offset, buffer.Length).Span);
        }
    }

    public class MirroredMemory : MemoryUnit
    {
        private readonly MemoryUnit _inner;
        public override int Length { get; }

        public MirroredMemory(int length, MemoryUnit inner)
        {
            Length = length;
            _inner = inner;
        }

        public override void Read(int offset, Span<byte> buffer)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset must be non-negative.");
            }

            if (offset + buffer.Length > Length)
            {
                throw new IndexOutOfRangeException("The read goes out of the bounds of the memory.");
            }

            // Read in a loop until we've read all the data
            var length = buffer.Length;
            var bufferOffset = 0;
            while (length > 0)
            {
                // Mirror the offset
                offset = offset % _inner.Length;

                var toRead = Math.Min(length - offset, _inner.Length - offset);
                var toFill = buffer.Slice(bufferOffset, toRead);
                _inner.Read(offset, toFill);

                bufferOffset += toFill.Length;
                offset += toFill.Length;
                length -= toFill.Length;
            }
        }

        public override void Write(int offset, ReadOnlySpan<byte> buffer)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset must be non-negative.");
            }

            if (offset + buffer.Length > Length)
            {
                throw new IndexOutOfRangeException("The write goes out of the bounds of the memory.");
            }

            // PERF: We could, in theory, optimize away writes we know will be overwritten due to mirroring
            // However, it's rare that you'd actually INTEND to overwrite data with this mirroring behavior.

            // Write in a loop until we've read all the data
            var length = buffer.Length;
            var bufferOffset = 0;
            while (length > 0)
            {
                // Mirror the offset
                offset = offset % _inner.Length;

                var toRead = Math.Min(length - offset, _inner.Length - offset);
                var toWrite = buffer.Slice(bufferOffset, toRead);
                _inner.Write(offset, toWrite);

                bufferOffset += toWrite.Length;
                offset += toWrite.Length;
                length -= toWrite.Length;
            }
        }
    }

    public class VirtualMemory : MemoryUnit
    {
        private int _length;
        private SortedList<int, MemoryUnit> _memories = new SortedList<int, MemoryUnit>();
        private readonly ILogger _logger;

        public override int Length => _length;

        public VirtualMemory() : this(NullLogger.Instance)
        {
        }

        public VirtualMemory(ILogger logger)
        {
            _logger = logger;
        }

        public void Attach(int offset, byte[] contents) => Attach(offset, new FixedMemory(contents));
        public void Attach(int offset, MemoryUnit unit)
        {
            // Check for overlap
            var end = offset + unit.Length;
            foreach (var mem in _memories)
            {
                if (mem.Key < end && offset < (mem.Key + mem.Value.Length))
                {
                    _logger.LogError(
                        new EventId(0, "AttachedMemoryOverlapped"),
                        "Failed to attach new memory unit '{Memory}' at ${Start:X4}. " +
                        "It overlaps with existing unit '{ExistingMemory}' (${ExistingStart:X4}-${ExistingEnd:X4})",
                        unit, offset, mem, mem.Key, mem.Key + mem.Value.Length);
                    throw new InvalidOperationException("Cannot attach memory as it would overlap with existing memory!");
                }

                // Early exit
                if (mem.Key > end)
                {
                    break;
                }
            }

            _memories.Add(offset, unit);

            var lastMem = _memories.Last();
            _length = lastMem.Key + lastMem.Value.Length;

            _logger.LogInformation(
                new EventId(0, "AttachedMemory"),
                "Attached new memory unit '{Memory}' at ${Start:X4}-${End:X4} ({Length} bytes). Total length is now {TotalLength} bytes",
                unit, offset, offset + unit.Length, unit.Length, _length);

            if (_logger.IsEnabled(LogLevel.Trace))
            {
                LogMemoryMap();
            }
        }

        public void Detach(MemoryUnit unit)
        {
            var index = _memories.IndexOfValue(unit);
            if (index < 0)
            {
                throw new InvalidOperationException("Cannot remove memory unit, it is not attached.");
            }
            _memories.RemoveAt(index);

            var lastMem = _memories.Last();
            _length = lastMem.Key + lastMem.Value.Length;

            if (_logger.IsEnabled(LogLevel.Trace))
            {
                LogMemoryMap();
            }
        }

        public override void Read(int offset, Span<byte> buffer)
        {
            var readStart = offset;
            var length = buffer.Length;

            if (readStart + length > _length)
            {
                throw new IndexOutOfRangeException("The read goes out of the bounds of the memory.");
            }

            foreach (var (segmentStart, mem) in _memories)
            {
                var segmentEnd = segmentStart + mem.Length;
                if (readStart < segmentEnd)
                {
                    var readEnd = Math.Min(segmentEnd, readStart + length);

                    // There may have been a gap before this segment, so align the start point.
                    readStart = Math.Max(readStart, segmentStart);

                    if (readStart > readEnd)
                    {
                        // The read ended before this segment, so just return what we've seen already.
                        return;
                    }

                    // The read is contained in this segment
                    var dest = buffer.Slice(readStart - offset, readEnd - readStart);
                    mem.Read(readStart - segmentStart, dest);

                    // Update state
                    readStart += dest.Length;
                    length -= dest.Length;
                }

                if (length == 0)
                {
                    return;
                }
            }
        }

        public override void Write(int offset, ReadOnlySpan<byte> buffer)
        {
            var writeStart = offset;
            var length = buffer.Length;
            if (writeStart + length > _length)
            {
                throw new IndexOutOfRangeException("The write goes out of the bounds of the memory.");
            }

            foreach (var (segmentStart, mem) in _memories)
            {
                var segmentEnd = segmentStart + mem.Length;
                if (writeStart < segmentEnd)
                {
                    if (writeStart < segmentStart)
                    {
                        // We're writing into gap
                        throw new InvalidOperationException("Cannot write to ${writeStart:X4}. There is no memory there.");
                    }

                    var writeEnd = writeStart + length;
                    writeEnd = Math.Min(writeEnd, segmentEnd);

                    var toWrite = buffer.Slice(writeStart - offset, writeEnd - writeStart);
                    mem.Write(writeStart - segmentStart, toWrite);

                    // Update loop state
                    writeStart += toWrite.Length;
                    length -= toWrite.Length;
                }

                if (length == 0)
                {
                    return;
                }
            }
        }

        private void LogMemoryMap()
        {
            _logger.LogTrace(new EventId(0, "DumpingMemoryMap"), "Dumping memory map.");
            foreach (var (start, mem) in _memories)
            {
                _logger.LogTrace(
                    new EventId(0, "MemoryMapElement"),
                    "{Memory} memory - ${Start:X4} - ${End:X4} ({Length} bytes)",
                    mem, start, start + mem.Length, mem.Length);
            }
        }
    }
}