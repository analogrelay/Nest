using System;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;

namespace Nest.CommandLine.Commands
{
    [Command("help", Description = "Get help for a command, or a list of available commands.")]
    internal class HelpCommand
    {
        [Argument(0, "<COMMAND>", "The command to get help for. If not specified, a list of available commands will be shown.")]
        public string Command { get; set; }

        public void OnExecute(CommandLineApplication cmd, IConsole console)
        {
            var app = cmd.Parent;
            if (string.IsNullOrEmpty(Command))
            {
                app.ShowHelp();
            }
            else
            {
                var command = app.Commands
                    .FirstOrDefault(c => c.Names.Any(n => string.Equals(n, Command, StringComparison.OrdinalIgnoreCase)));
                if (command == null)
                {
                    console.Error.WriteLine($"Unknown command '{Command}'.");
                    app.ShowHelp();
                }
                else {
                    command.ShowHelp();
                }
            }
        }
    }
}