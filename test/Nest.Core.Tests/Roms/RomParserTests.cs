using System.IO;
using Nest.Roms;
using Xunit;

namespace Nest.Core.Tests
{
    public class RomParserTests
    {
        [Fact]
        public void ParseHeader_ThrowsIfInsufficientData()
        {
            var buf = new byte[2];
            Assert.Throws<InvalidDataException>(() => RomParser.ParseHeader(buf));
        }

        [Fact]
        public void ParseHeader_ThrowsIfMissingMagicNumber()
        {
            var buf = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            Assert.Throws<InvalidDataException>(() => RomParser.ParseHeader(buf));
        }
    }
}