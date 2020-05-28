using McMaster.Extensions.CommandLineUtils;

namespace Nest.CommandLine.Commands
{
    [Subcommand(typeof(HelpCommand))]
    internal abstract class CommandGroupBase
    {
        internal virtual void OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
        }
    }
}