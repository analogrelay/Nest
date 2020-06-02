using Nest.Hardware;
using Nest.Memory;
using Xunit;

namespace Nest.Tests.Hardware
{
    internal class Mos6502TestBuilder
    {
        public VirtualMemory Memory { get; } = new VirtualMemory();
        public Mos6502.State InitialState { get; set; }
        public Mos6502.State ResultState { get; set; }

        public Mos6502TestBuilder WithMemory(int start, params byte[] data)
        {
            Memory.Attach(start, new FixedMemory(data));
            return this;
        }

        public Mos6502TestBuilder WithInitialState(Mos6502.State state)
        {
            InitialState = state;
            return this;
        }

        public Mos6502TestBuilder WithInitialState(
            int? a = null, int? x = null, int? y = null, int? pc = null, int? s = null, Mos6502.Flags? p = null)
        {
            InitialState = Mos6502.State.PowerUp.With(a, x, y, pc, s, p);
            return this;
        }

        public Mos6502TestBuilder WithResultState(Mos6502.State state)
        {
            ResultState = state;
            return this;
        }

        public Mos6502TestBuilder WithResultState(
            int? a = null, int? x = null, int? y = null, int? pc = null, int? s = null, Mos6502.Flags? p = null)
        {
            ResultState = Mos6502.State.PowerUp.With(a, x, y, pc, s, p);
            return this;
        }

        public void Run()
        {
            var cpu = new Mos6502(InitialState, Memory);

            // Run!
            cpu.Step();

            // Check the end state
            Assert.Equal(ResultState, cpu.CurrentState);
        }
    }
}