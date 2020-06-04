using System;
using System.Collections.Generic;
using System.Linq;
using Nest.Memory;
using Xunit;

namespace Nest.Hardware.Mos6502
{
    internal class Mos6502TestBuilder
    {
        private List<MemoryOperation> _expectedOperations = new List<MemoryOperation>();

        public VirtualMemory Memory { get; } = new VirtualMemory();
        public Mos6502State InitialState { get; set; } = Mos6502State.PowerUp;
        public Mos6502State ResultState { get; set; } = Mos6502State.PowerUp;

        public Mos6502TestBuilder WithMemory(IEnumerable<(int Start, byte[] Data)> memories)
        {
            foreach (var memory in memories)
            {
                Memory.Attach(memory.Start, new FixedMemory(memory.Data));
            }
            return this;
        }

        public Mos6502TestBuilder WithMemory(int start, params byte[] data)
        {
            Memory.Attach(start, new FixedMemory(data));
            return this;
        }

        public Mos6502TestBuilder WithMemory(int start, MemoryUnit memory)
        {
            Memory.Attach(start, memory);
            return this;
        }

        public Mos6502TestBuilder WithInitialState(Mos6502State state)
        {
            InitialState = state;
            return this;
        }

        public Mos6502TestBuilder WithInitialState(
            int? a = null, int? x = null, int? y = null, int? pc = null, int? s = null, Mos6502Flags? p = null)
        {
            InitialState = Mos6502State.PowerUp.With(a, x, y, pc, s, p);
            return this;
        }

        public Mos6502TestBuilder WithResultState(Mos6502State state)
        {
            ResultState = state;
            return this;
        }

        public Mos6502TestBuilder WithResultState(
            int? a = null, int? x = null, int? y = null, int? pc = null, int? s = null, Mos6502Flags p = Mos6502Flags.None)
        {
            ResultState = Mos6502State.PowerUp.With(a, x, y, pc, s, p: Mos6502Flags.PowerUp | p);
            return this;
        }

        public Mos6502TestBuilder ExpectWrite(int start, params byte[] expectedData)
        {
            _expectedOperations.Add(new MemoryOperation(MemoryOperationType.Write, start, expectedData));
            return this;
        }

        public void Run()
        {
            static string ListOperations(IReadOnlyList<MemoryOperation> list, int offset)
                => string.Join(Environment.NewLine, list.Skip(offset).Select(o => $"* {o}"));

            var trackingMemory = new TrackingMemory(Memory);
            var cpu = new Mos6502Cpu(InitialState, trackingMemory);

            // Run!
            cpu.Step();

            // Check the end state
            Assert.Equal(ResultState, cpu.CurrentState);

            // Ignore reads
            int offset = 0;
            foreach (var operation in trackingMemory.Operations.Where(o => o.Type != MemoryOperationType.Read))
            {
                Assert.True(offset < _expectedOperations.Count, $"Unexpected: {operation}");
                var expectation = _expectedOperations[offset];

                Assert.Equal(expectation.Data.ToArray(), operation.Data.ToArray());

                offset += 1;
            }

            Assert.True(
                offset == _expectedOperations.Count,
                $"Expected additional memory operations!{Environment.NewLine}{ListOperations(_expectedOperations, offset)}");
        }

        // Tracks access to the memory unit so we can find un-expected reads/writes.
        private class TrackingMemory : MemoryUnit
        {
            private List<MemoryOperation> _operations = new List<MemoryOperation>();
            private readonly MemoryUnit _inner;

            public IReadOnlyList<MemoryOperation> Operations => _operations;

            public override int Length => _inner.Length;

            public TrackingMemory(MemoryUnit inner)
            {
                _inner = inner;
            }

            public override void Read(int offset, Span<byte> buffer)
            {
                _inner.Read(offset, buffer);
                _operations.Add(new MemoryOperation(MemoryOperationType.Read, offset, buffer.ToArray()));
            }

            public override void Write(int offset, ReadOnlySpan<byte> buffer)
            {
                _inner.Write(offset, buffer);
                _operations.Add(new MemoryOperation(MemoryOperationType.Write, offset, buffer.ToArray()));
            }
        }

        private struct MemoryOperation
        {
            public MemoryOperation(MemoryOperationType type, int offset, ReadOnlyMemory<byte> data)
            {
                Type = type;
                Offset = offset;
                Data = data;
            }

            public MemoryOperationType Type { get; }
            public int Offset { get; }
            public ReadOnlyMemory<byte> Data { get; }

            public override string ToString()
            {
                var dataStr = string.Join(" ", Data.ToArray().Select(d => $"${d:X2}"));
                return $"{Type} ${Offset:X4} [ {dataStr} ]";
            }
        }

        private enum MemoryOperationType
        {
            Read,
            Write
        }
    }
}