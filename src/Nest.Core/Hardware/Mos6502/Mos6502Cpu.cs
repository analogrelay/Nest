using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nest.Memory;

namespace Nest.Hardware.Mos6502
{
    public class Mos6502Cpu
    {
        public Mos6502State CurrentState { get; private set; }
        public MemoryUnit Memory { get; }

        private readonly ILogger _logger;

        public Mos6502Cpu(MemoryUnit memory) : this(Mos6502State.PowerUp, memory, NullLoggerFactory.Instance)
        {
        }

        public Mos6502Cpu(MemoryUnit memory, ILoggerFactory loggerFactory) : this(Mos6502State.PowerUp, memory, loggerFactory)
        {
        }


        public Mos6502Cpu(Mos6502State initialState, MemoryUnit memory) : this(initialState, memory, NullLoggerFactory.Instance)
        {
        }

        public Mos6502Cpu(Mos6502State initialState, MemoryUnit memory, ILoggerFactory loggerFactory)
        {
            CurrentState = initialState;
            Memory = memory;
            _logger = loggerFactory.CreateLogger<Mos6502Cpu>();

            _logger.LogTrace(new EventId(0, "Initializing"), "Initializing with state: {InitialMos6502State}", CurrentState);
        }

        /// <summary>
        /// Triggers the reset behavior of the CPU. This generally needs to be called before any operations
        /// are performed!
        /// </summary>
        public void Reset()
        {
            // Read the program counter value from the Reset vector
            var newPc = Memory.ReadUInt16LittleEndian(0xFFFC);

            // Change the state
            _logger.LogInformation(new EventId(0, "Resetting"), "Resetting CPU state.");
            ChangeMos6502State(new Mos6502State(
                CurrentState.A, CurrentState.X, CurrentState.Y,
                pc: newPc,
                CurrentState.S, CurrentState.P
            ));
        }

        /// <summary>
        /// Invokes a single step of the CPU.
        /// </summary>
        /// <remarks>
        /// This will execute a single instruction located at the address pointed to by <see cref="Mos6502State.PC" /> and update the CPU
        /// state and memory to reflect the results of executing the instruction.
        /// </remarks>
        public void Step()
        {
            // Fetch and decode the next instruction
            // The number of bytes in an instruction depends on the opcode, so we allow the decoder to handle it.
            var instruction = Mos6502Decoder.Decode(Memory, CurrentState.PC);

            // Execute the instruction
            var newState = Mos6502Executor.Execute(instruction, CurrentState, Memory, _logger);

            ChangeMos6502State(newState);
        }

        private void ChangeMos6502State(Mos6502State newState)
        {
            _logger.LogTrace(new EventId(0, "ChangingMos6502State"), "Changing state from {OldMos6502State} to {NewMos6502State}", CurrentState, newState);
            CurrentState = newState;
        }

    }
}