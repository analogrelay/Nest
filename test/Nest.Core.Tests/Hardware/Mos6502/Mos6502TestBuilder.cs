using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Nest.Memory;
using Xunit;

namespace Nest.Hardware.Mos6502
{
    internal class Mos6502TestBuilder
    {
        public VirtualMemory Memory { get; } = new VirtualMemory();
        public Mos6502State InitialState { get; set; }
        public Mos6502State ResultState { get; set; }

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

        public void Run()
        {
            var cpu = new Mos6502Cpu(InitialState, Memory);

            // Run!
            cpu.Step();

            // Check the end state
            Assert.Equal(ResultState, cpu.CurrentState);
        }
    }
}