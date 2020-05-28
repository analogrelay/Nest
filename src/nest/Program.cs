using System;
using McMaster.Extensions.CommandLineUtils;
using Nest.CommandLine.Commands;

namespace Nest.CommandLine
{
    [Command("nest", Description = "NES Emulation tools for .NET")]
    [Subcommand(typeof(RomCommandGroup))]
    internal class Program: CommandGroupBase
    {
        static int Main(string[] args)
        {
            try
            {
                CommandLineApplication.Execute<Program>(args);
            }
            catch (CommandLineException clex)
            {
                var oldFg = Console.ForegroundColor;
                try
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine(clex.Message);
                    return clex.ExitCode;
                } finally {
                    Console.ForegroundColor = oldFg;
                }
            }
            catch(Exception ex)
            {
                var oldFg = Console.ForegroundColor;
                try
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("Unexpected exception!");
                    Console.Error.WriteLine(ex.ToString());
                    return 1;
                } finally {
                    Console.ForegroundColor = oldFg;
                }
            }

            return 0;
        }
    }
}
