using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest.Hardware
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

        public override int Length => _content.Length;
        public Span<byte> Data => _content.Span;
        public bool CanRead { get; }
        public bool CanWrite { get; }

        public FixedMemory(int size) : this(new byte[size], true, true)
        {
        }

        public FixedMemory(int size, bool canRead, bool canWrite) : this(new byte[size], canRead, canWrite)
        {
        }

        public FixedMemory(Memory<byte> initialContent) : this(initialContent, true, true)
        {
        }

        public FixedMemory(Memory<byte> initialContent, bool canRead, bool canWrite)
        {
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

            buffer.CopyTo(_content.Slice(offset, buffer.Length).Span);
        }
    }

    public class MirroredMemory : MemoryUnit
    {
        private MemoryUnit _inner;
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
            while(length > 0) {
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
            while(length > 0) {
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

        public override int Length => _length;

        public void Attach(int offset, MemoryUnit unit)
        {
            // Check for overlap
            var end = offset + unit.Length;
            foreach (var mem in _memories)
            {
                if (mem.Key < end && offset < (mem.Key + mem.Value.Length))
                {
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
    }
}