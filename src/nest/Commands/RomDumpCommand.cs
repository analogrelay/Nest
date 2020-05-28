using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Nest.Roms;

namespace Nest.CommandLine.Commands
{
    [Command("dump", Description = "Dump data about an iNES ROM")]
    internal class RomDumpCommand
    {
        [Argument(0, "<FILE>", "The ROM file to dump metadata for.")]
        public string FilePath { get; set; }
        internal async Task OnExecuteAsync(IConsole console)
        {
            void WriteSizes(MemorySizes sizes)
            {
                console.WriteLine($"    ROM: {sizes.RomBanks} banks");
                console.WriteLine($"    RAM: {sizes.Ram} bytes");
                console.WriteLine($"    NVRAM: {sizes.NvRam} bytes");
            }

            // Load the file and read 16 bytes
            var buf = new byte[16];
            using var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var read = 0;
            while ((read = await stream.ReadAsync(buf, 0, 16)) < RomHeader.Size) ;

            // Parse the header
            var header = RomParser.ParseHeader(buf);

            console.WriteLine($"{header.Version} ROM");
            console.WriteLine("Program Memory:");
            WriteSizes(header.Program);
            console.WriteLine("Character Memory:");
            WriteSizes(header.Character);

            console.WriteLine($"Mapper: {header.Mapper}.{header.Submapper}");
            console.WriteLine($"CPU Timing: {header.CpuTiming}");
            console.WriteLine($"Flags: {header.Flags}");
            console.WriteLine($"Console: {header.Console}");
            if (header.Console == ConsoleType.NintendoVsSystem || header.Console == ConsoleType.Extended)
            {
                console.WriteLine($"    Detail: 0x{header.ConsoleTypeDetail:X}");
            }
            console.WriteLine($"Misc. ROMS: {header.MiscellaneousRomCount}");
            console.WriteLine($"Default Expansion Device: {header.DefaultExpansionDevice}");
        }
    }
}