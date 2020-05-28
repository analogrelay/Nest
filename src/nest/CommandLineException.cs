using System;

namespace Nest.CommandLine
{
    [Serializable]
    internal class CommandLineException : Exception
    {
        public int ExitCode { get; }

        public CommandLineException(string message) : this(message, 1)
        {
        }

        public CommandLineException(string message, Exception innerException) : this(message, 1, innerException)
        {
        }

        public CommandLineException(string message, int exitCode) : base(message)
        {
            ExitCode = exitCode;
        }

        public CommandLineException(string message, int exitCode, Exception innerException) : base(message, innerException)
        {
            ExitCode = exitCode;
        }
    }
}