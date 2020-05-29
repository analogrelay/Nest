using McMaster.Extensions.CommandLineUtils;

namespace Nest.CommandLine.Commands
{
    [Command("rom", Description = "Commands for working with iNES ROMS")]
    [Subcommand(typeof(RomDumpCommand))]
    internal class RomCommandGroup : CommandGroupBase
    {
    }
}