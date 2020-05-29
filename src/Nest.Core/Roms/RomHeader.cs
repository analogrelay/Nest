using System;

namespace Nest.Roms
{
    public struct RomHeader
    {
        public static readonly int Size = 16;
        public static readonly int ProgramRomBankSize = 16 * 1024;
        public static readonly int CharacterRomBankSize = 8 * 1024;

        public static ReadOnlySpan<byte> MagicNumber => new byte[] {
            (byte)'N', (byte)'E', (byte)'S', 0x1A /* ASCII EOF */
        };

        public RomHeader(RomVersion version, MemorySizes program, MemorySizes character, int mapper, int submapper, CpuTimingMode cpuTiming, RomFlags flags, ConsoleType console, int consoleTypeDetail, int miscellaneousRomCount, int defaultExpansionDevice)
        {
            Version = version;
            Program = program;
            Character = character;
            Mapper = mapper;
            Submapper = submapper;
            CpuTiming = cpuTiming;
            Flags = flags;
            Console = console;
            ConsoleTypeDetail = consoleTypeDetail;
            MiscellaneousRomCount = miscellaneousRomCount;
            DefaultExpansionDevice = defaultExpansionDevice;
        }

        public RomVersion Version { get; }
        public MemorySizes Program { get; }
        public MemorySizes Character { get; }
        public int Mapper { get; }
        public int Submapper { get; }
        public CpuTimingMode CpuTiming { get; }
        public RomFlags Flags { get; }
        public ConsoleType Console { get; }
        public int ConsoleTypeDetail { get; }
        public int MiscellaneousRomCount { get; }
        public int DefaultExpansionDevice { get; }
    }

    public struct MemorySizes
    {
        public MemorySizes(int romBanks, int ram, int nvRam)
        {
            RomBanks = romBanks;
            Ram = ram;
            NvRam = nvRam;
        }

        /// <summary>
        /// Gets the size of the memory in banks 
        public int RomBanks { get; }
        public int Ram { get; }
        public int NvRam { get; }
    }

    public enum RomVersion
    {
        Nes20,
        INes
    }

    public enum CpuTimingMode
    {
        Ntsc = 0,
        Pal = 1,
        MultiRegion = 2,
        Dendy = 3,
    }

    public enum ConsoleType
    {
        NintendoEntertainmentSystem = 0,
        NintendoVsSystem = 1,
        NintendoPlaychoice10 = 2,
        Extended = 3,
    }

    public enum RomFlags
    {
        None = 0,
        VerticalMirroring = 0b0000_0001,
        BatteryIsPresent = 0b0000_0010,
        TrainerIsPresent = 0b0000_0100,
        FourScreenMode = 0b0000_1000,

    }
}