using System;
using System.Buffers.Binary;

namespace Nest.Memory
{
    /// <summary>
    /// Extension methods for reading useful data types from a <see cref="MemoryUnit" />
    /// </summary>
    public static class MemoryUnitExtensions
    {
        public static byte ReadByte(this MemoryUnit self, int offset)
        {
            Span<byte> buf = stackalloc byte[1];
            self.Read(offset, buf);
            return buf[0];
        }

        public static void WriteByte(this MemoryUnit self, int offset, byte value)
        {
            Span<byte> buf = stackalloc byte[1];
            buf[0] = value;
            self.Write(offset, buf);
        }

        // UInt16
        public static ushort ReadUInt16BigEndian(this MemoryUnit self, int offset)
        {
            Span<byte> buf = stackalloc byte[2];
            self.Read(offset, buf);
            return BinaryPrimitives.ReadUInt16BigEndian(buf);
        }

        public static void WriteUInt16BigEndian(this MemoryUnit self, int offset, ushort value)
        {
            Span<byte> buf = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(buf, value);
            self.Write(offset, buf);
        }

        public static ushort ReadUInt16LittleEndian(this MemoryUnit self, int offset)
        {
            Span<byte> buf = stackalloc byte[2];
            self.Read(offset, buf);
            return BinaryPrimitives.ReadUInt16LittleEndian(buf);
        }

        public static void WriteUInt16LittleEndian(this MemoryUnit self, int offset, ushort value)
        {
            Span<byte> buf = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16LittleEndian(buf, value);
            self.Write(offset, buf);
        }

        // Int16
        public static short ReadInt16BigEndian(this MemoryUnit self, int offset)
        {
            Span<byte> buf = stackalloc byte[2];
            self.Read(offset, buf);
            return BinaryPrimitives.ReadInt16BigEndian(buf);
        }

        public static void WriteInt16BigEndian(this MemoryUnit self, int offset, short value)
        {
            Span<byte> buf = stackalloc byte[2];
            BinaryPrimitives.WriteInt16BigEndian(buf, value);
            self.Write(offset, buf);
        }

        public static short ReadInt16LittleEndian(this MemoryUnit self, int offset)
        {
            Span<byte> buf = stackalloc byte[2];
            self.Read(offset, buf);
            return BinaryPrimitives.ReadInt16LittleEndian(buf);
        }

        public static void WriteInt16LittleEndian(this MemoryUnit self, int offset, short value)
        {
            Span<byte> buf = stackalloc byte[2];
            BinaryPrimitives.WriteInt16LittleEndian(buf, value);
            self.Write(offset, buf);
        }

        // UInt32
        public static uint ReadUInt32BigEndian(this MemoryUnit self, int offset)
        {
            Span<byte> buf = stackalloc byte[4];
            self.Read(offset, buf);
            return BinaryPrimitives.ReadUInt32BigEndian(buf);
        }

        public static void WriteUInt32BigEndian(this MemoryUnit self, int offset, uint value)
        {
            Span<byte> buf = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(buf, value);
            self.Write(offset, buf);
        }

        public static uint ReadUInt32LittleEndian(this MemoryUnit self, int offset)
        {
            Span<byte> buf = stackalloc byte[4];
            self.Read(offset, buf);
            return BinaryPrimitives.ReadUInt32LittleEndian(buf);
        }

        public static void WriteUInt32LittleEndian(this MemoryUnit self, int offset, uint value)
        {
            Span<byte> buf = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32LittleEndian(buf, value);
            self.Write(offset, buf);
        }

        // Int32
        public static int ReadInt32BigEndian(this MemoryUnit self, int offset)
        {
            Span<byte> buf = stackalloc byte[4];
            self.Read(offset, buf);
            return BinaryPrimitives.ReadInt32BigEndian(buf);
        }

        public static void WriteInt32BigEndian(this MemoryUnit self, int offset, int value)
        {
            Span<byte> buf = stackalloc byte[4];
            BinaryPrimitives.WriteInt32BigEndian(buf, value);
            self.Write(offset, buf);
        }

        public static int ReadInt32LittleEndian(this MemoryUnit self, int offset)
        {
            Span<byte> buf = stackalloc byte[4];
            self.Read(offset, buf);
            return BinaryPrimitives.ReadInt32LittleEndian(buf);
        }

        public static void WriteInt32LittleEndian(this MemoryUnit self, int offset, int value)
        {
            Span<byte> buf = stackalloc byte[4];
            BinaryPrimitives.WriteInt32LittleEndian(buf, value);
            self.Write(offset, buf);
        }

        // UInt64
        public static ulong ReadUInt64BigEndian(this MemoryUnit self, int offset)
        {
            Span<byte> buf = stackalloc byte[8];
            self.Read(offset, buf);
            return BinaryPrimitives.ReadUInt64BigEndian(buf);
        }

        public static void WriteUInt64BigEndian(this MemoryUnit self, int offset, ulong value)
        {
            Span<byte> buf = stackalloc byte[8];
            BinaryPrimitives.WriteUInt64BigEndian(buf, value);
            self.Write(offset, buf);
        }

        public static ulong ReadUInt64LittleEndian(this MemoryUnit self, int offset)
        {
            Span<byte> buf = stackalloc byte[8];
            self.Read(offset, buf);
            return BinaryPrimitives.ReadUInt64LittleEndian(buf);
        }

        public static void WriteUInt64LittleEndian(this MemoryUnit self, int offset, ulong value)
        {
            Span<byte> buf = stackalloc byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(buf, value);
            self.Write(offset, buf);
        }

        // Int64
        public static long ReadInt64BigEndian(this MemoryUnit self, int offset)
        {
            Span<byte> buf = stackalloc byte[8];
            self.Read(offset, buf);
            return BinaryPrimitives.ReadInt64BigEndian(buf);
        }

        public static void WriteInt64BigEndian(this MemoryUnit self, int offset, long value)
        {
            Span<byte> buf = stackalloc byte[8];
            BinaryPrimitives.WriteInt64BigEndian(buf, value);
            self.Write(offset, buf);
        }

        public static long ReadInt64LittleEndian(this MemoryUnit self, int offset)
        {
            Span<byte> buf = stackalloc byte[8];
            self.Read(offset, buf);
            return BinaryPrimitives.ReadInt64LittleEndian(buf);
        }

        public static void WriteInt64LittleEndian(this MemoryUnit self, int offset, long value)
        {
            Span<byte> buf = stackalloc byte[8];
            BinaryPrimitives.WriteInt64LittleEndian(buf, value);
            self.Write(offset, buf);
        }
    }
}