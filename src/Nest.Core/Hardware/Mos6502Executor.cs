using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nest.Memory;

namespace Nest.Hardware
{
    public static class Mos6502Executor
    {
        public static Mos6502.State Execute(Mos6502Instruction instruction, Mos6502.State state, VirtualMemory memory, ILogger? logger = null)
        {
            logger ??= NullLogger.Instance;

            logger.LogTrace("Executing {Instruction} (Current CPU State: {State})", instruction, state);

            // TODO: Implementation
            var newState = state;

            logger.LogTrace("Executed {Instruction} (New CPU State: {State})", instruction, newState);

            return newState;
        }
    }
}