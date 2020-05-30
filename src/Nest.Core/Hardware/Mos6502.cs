using System;
using Microsoft.Extensions.Logging;
using Nest.Memory;

namespace Nest.Hardware
{
    public class Mos6502
    {
        public State CurrentState { get; private set; }
        public VirtualMemory Memory { get; }

        private readonly ILogger<Mos6502> _logger;

        public Mos6502(State initialState, VirtualMemory memory, ILoggerFactory loggerFactory)
        {
            CurrentState = initialState;
            Memory = memory;
            _logger = loggerFactory.CreateLogger<Mos6502>();

            _logger.LogTrace(new EventId(0, "Initializing"), "Initializing with state: {InitialState}", CurrentState);
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
            ChangeState(new State(
                CurrentState.A, CurrentState.X, CurrentState.Y,
                pc: newPc,
                CurrentState.S, CurrentState.P
            ));
        }

        /// <summary>
        /// Invokes a single step of the CPU.
        /// </summary>
        /// <remarks>
        /// This will execute a single instruction located at the address pointed to by <see cref="State.PC" /> and update the CPU
        /// state and memory to reflect the results of executing the instruction.
        /// </remarks>
        public void Step()
        {
            // Fetch and decode the next instruction
            // The number of bytes in an instruction depends on the opcode, so we allow the decoder to handle it.
            var instruction = Mos6502Decoder.Decode(Memory, CurrentState.PC);

            // Execute the instruction
            var newState = Mos6502Executor.Execute(instruction, CurrentState, Memory, _logger);

            ChangeState(newState);
        }

        private void ChangeState(State newState) {
            _logger.LogTrace(new EventId(0, "ChangingState"), "Changing state from {OldState} to {NewState}", CurrentState, newState);
            CurrentState = newState;
        }

        // http://wiki.nesdev.com/w/index.php/CPU_registers
        public readonly struct State
        {
            public static readonly State PowerUp =
                new State(a: 0, x: 0, y: 0, pc: 0, s: 0xFD, p: Flags.PushedByInstruction | Flags.PushedToStack | Flags.InterruptDisable);

            public State(int a, int x, int y, int pc, int s, Flags p)
            {
                // PERF: Consider eliding bounds checks when internal operations are modifying the state.
                if (a > byte.MaxValue || a < byte.MinValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(a), $"Value '{a}' is out of the allowed range for register A.");
                }

                if (x > byte.MaxValue || x < byte.MinValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(x), $"Value '{x}' is out of the allowed range for register X.");
                }

                if (y > byte.MaxValue || y < byte.MinValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(y), $"Value '{y}' is out of the allowed range for register Y.");
                }

                if (pc > ushort.MaxValue || pc < ushort.MinValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(pc), $"Value '{pc}' is out of the allowed range (${ushort.MinValue:X4}-${ushort.MaxValue:X4}) for register PC.");
                }

                if (s > byte.MaxValue || s < byte.MinValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(s), $"Value '{s}' is out of the allowed range for register S.");
                }

                A = a;
                X = x;
                Y = y;
                PC = pc;
                S = s;
                P = p;
            }

            public int A { get; }
            public int X { get; }
            public int Y { get; }
            public int PC { get; }
            public int S { get; }
            public Mos6502.Flags P { get; }

            public override string ToString() =>
                $"[A:${A:X2} X:${X:X2} Y:${Y:X2} PC:${PC:X4} S:${S:X2} P:${(byte)P:X2} ({P})]";
        }

        // http://wiki.nesdev.com/w/index.php/Status_flags
        public enum Flags
        {
            Carry = 0b0000_0001,
            Zero = 0b0000_0010,
            InterruptDisable = 0b0000_0100,
            Decimal = 0b0000_1000, // Not used on NES
            // Set ON the stack when the flags are pushed to the stack by an instruction
            // Clear ON the stack when the flags are pushed to the stack by an interrupt
            // Has no effect on the CPU.
            PushedByInstruction = 0b0001_0000,
            // Set ON the stack when the flags are pushed to the stack
            // Has no effect on the CPU
            PushedToStack = 0b0010_0000,
            Overflow = 0b0100_0000,
            Negative = 0b1000_0000
        }
    }
}