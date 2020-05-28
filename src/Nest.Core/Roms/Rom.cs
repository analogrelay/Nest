using System;

namespace Nest.Roms
{
    public class Rom
    {
        public Rom(RomHeader header, ReadOnlyMemory<byte> programRom, ReadOnlyMemory<byte> characterRom)
        {
            Header = header;
            ProgramRom = programRom;
            CharacterRom = characterRom;
        }

        public RomHeader Header { get; }
        public ReadOnlyMemory<byte> ProgramRom { get; }
        public ReadOnlyMemory<byte> CharacterRom { get; }
    }
}