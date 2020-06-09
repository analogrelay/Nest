using Xunit;

// We don't use the 'mnemonic' parameter but it's very useful as a diagnostic tool.
#pragma warning disable xUnit1026

namespace Nest.Hardware.Mos6502
{
    public partial class Mos6502Tests
    {
        public class BranchInstructions
        {
            public static readonly TheoryData<string, byte, Mos6502Flags, bool> Branches = new TheoryData<string, byte, Mos6502Flags, bool>() {
                { "BCC", 0x90, Mos6502Flags.Carry, false },
                { "BCS", 0xB0, Mos6502Flags.Carry, true },
                { "BEQ", 0xF0, Mos6502Flags.Zero, true },
                { "BNE", 0xD0, Mos6502Flags.Zero, false },
                { "BMI", 0x30, Mos6502Flags.Negative, true },
                { "BPL", 0x10, Mos6502Flags.Negative, false },
                { "BVS", 0x70, Mos6502Flags.Overflow, true },
                { "BVC", 0x50, Mos6502Flags.Overflow, false },
            };

            [Theory]
            [MemberData(nameof(Branches))]
            public void AdvancesPCAsNormalIfConditionFalse(string mnemonic, byte opcode, Mos6502Flags flag, bool branchIfSet) => new Mos6502TestBuilder()
                .WithMemory(0x0000, opcode, 0x42)
                .WithInitialState(p: branchIfSet ? Mos6502Flags.None : flag)
                .WithResultState(pc: 0x02, p: branchIfSet ? Mos6502Flags.None : flag, clock: 2)
                .Run();

            [Theory]
            [MemberData(nameof(Branches))]
            public void TakesTwoCyclesIfConditionFalseEvenIfOffsetCrossesBoundary(string mnemonic, byte opcode, Mos6502Flags flag, bool branchIfSet) => new Mos6502TestBuilder()
                .WithMemory(0x0000, opcode, 0xFE)
                .WithInitialState(p: branchIfSet ? Mos6502Flags.None : flag)
                .WithResultState(pc: 0x02, p: branchIfSet ? Mos6502Flags.None : flag, clock: 2)
                .Run();

            [Theory]
            [MemberData(nameof(Branches))]
            public void AdvancesPCByOffsetIfConditionTrue(string mnemonic, byte opcode, Mos6502Flags flag, bool branchIfSet) => new Mos6502TestBuilder()
                .WithMemory(0x0000, opcode, 0x42)
                .WithInitialState(p: branchIfSet ? flag : Mos6502Flags.None)
                .WithResultState(pc: 0x44, p: branchIfSet ? flag : Mos6502Flags.None, clock: 3)
                .Run();

            [Theory]
            [MemberData(nameof(Branches))]
            public void TakesFourCyclesIfConditionTrueAndCrossesBoundary(string mnemonic, byte opcode, Mos6502Flags flag, bool branchIfSet) => new Mos6502TestBuilder()
                .WithMemory(0x00F0, opcode, 0x20)
                .WithInitialState(pc: 0x00F0, p: branchIfSet ? flag : Mos6502Flags.None)
                .WithResultState(pc: 0x0112, p: branchIfSet ? flag : Mos6502Flags.None, clock: 4)
                .Run();
        }
    }
}