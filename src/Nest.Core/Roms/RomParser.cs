using System;
using System.IO;
using System.Threading.Tasks;

namespace Nest.Roms
{
    // Based on http://wiki.nesdev.com/w/index.php/NES_2.0
    public static class RomParser
    {
        public static async Task<Rom> LoadRomAsync(Stream input)
        {
            // Read the header
            var headerBuf = new byte[16];
            await input.ReadExactAsync(headerBuf);
            var header = ParseHeader(headerBuf);

            // Read PRG ROM banks
            var prgRom = new byte[header.Program.RomBanks * RomHeader.ProgramRomBankSize];
            await input.ReadExactAsync(prgRom);

            // Read CHR ROM banks
            var chrRom = new byte[header.Character.RomBanks * RomHeader.CharacterRomBankSize];
            await input.ReadExactAsync(chrRom);

            return new Rom(header, prgRom, chrRom);
        }

        public static RomHeader ParseHeader(ReadOnlySpan<byte> data)
        {
            int GetRamSize(int shiftCount) => shiftCount == 0 ? 0 : 64 << shiftCount;

            // Check minimum size
            if (data.Length < RomHeader.Size)
            {
                throw new InvalidDataException("Unable to parse ROM. Header must be at least 16 bytes.");
            }

            // Check for the magic number
            if (!data.Slice(0, 4).SequenceEqual(RomHeader.MagicNumber))
            {
                throw new InvalidDataException("Invalid Magic Number");
            }

            // Check if this is NES 2.0 or INES
            // In NES 2.0, the 7th byte of the header has bit 2 clear and bit 3 set.
            var version = (data[7] & 0b1100) == 0b1000 ? RomVersion.Nes20 : RomVersion.INes;

            // Read the data! First, the LSB of the ROM sizes
            var prgRomSize = (int)data[4];
            var chrRomSize = (int)data[5];

            // Read flags and mapper number
            var flags = (RomFlags)(data[6] & 0b0000_1111);
            var mapper = (int)((data[6] & 0b1111_0000) >> 4);

            // Read console type and more mapper number
            var consoleType = (ConsoleType)(data[7] & 0b0000_0011);
            mapper |= (int)(data[7] & 0b1111_0000);

            // Even more mapper, and submapper
            mapper |= (int)((data[8] & 0b0000_1111) << 8);
            var submapper = (int)((data[8] & 0b1111_0000) >> 4);

            // MSBs for ROM sizes
            prgRomSize |= (int)(data[9] & 0b0000_1111) << 8;
            chrRomSize |= (int)(data[9] & 0b1111_0000) << 4;

            // RAM and NVRAM sizes
            var prgRamSize = GetRamSize(data[10] & 0b0000_1111);
            var prgNvRamSize = GetRamSize((data[10] & 0b1111_0000) >> 4);
            var chrRamSize = GetRamSize(data[11] & 0b0000_1111);
            var chrNvRamSize = GetRamSize((data[11] & 0b1111_0000) >> 4);

            // CPU timing
            var timing = (CpuTimingMode)(data[12] & 0b0000_0011);

            // Extended Console Type data
            var consoleTypeDetail = (int)data[13];

            var miscRoms = (int)(data[14] & 0b0000_0011);
            var expansionDevice = (int)(data[15] & 0b0011_1111);

            return new RomHeader(
                version,
                new MemorySizes(prgRomSize, prgRamSize, prgNvRamSize),
                new MemorySizes(chrRomSize, chrRamSize, chrNvRamSize),
                mapper,
                submapper,
                timing,
                flags,
                consoleType,
                consoleTypeDetail,
                miscRoms,
                expansionDevice);
        }
    }
}