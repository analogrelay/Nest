using static Nest.Hardware.Mos6502.Mos6502Operation;
using static Nest.Hardware.Mos6502.Mos6502AddressingMode;

namespace Nest.Hardware.Mos6502
{
    public static partial class Mos6502Decoder
    {
        // Borrowed mostly from https://github.com/fogleman/nes/blob/6d86d3ba286ec0e81acea05423f60455c222035e/nes/cpu.go
        // Which is licensed under the MIT License: https://github.com/fogleman/nes/blob/6d86d3ba286ec0e81acea05423f60455c222035e/LICENSE.md
        private static readonly Mos6502Operation[] _operationTable = new Mos6502Operation[256] {
            /*            x0   x1   x2   x3   x4   x5   x6   x7   x8   x9   xA   xB   xC   xD   xE   xF  */
            /* 0x0y */    BRK, ORA, KIL, SLO, NOP, ORA, ASL, SLO, PHP, ORA, ASL, ANC, NOP, ORA, ASL, SLO,
            /* 0x1y */    BPL, ORA, KIL, SLO, NOP, ORA, ASL, SLO, CLC, ORA, NOP, SLO, NOP, ORA, ASL, SLO,
            /* 0x2y */    JSR, AND, KIL, RLA, BIT, AND, ROL, RLA, PLP, AND, ROL, ANC, BIT, AND, ROL, RLA,
            /* 0x3y */    BMI, AND, KIL, RLA, NOP, AND, ROL, RLA, SEC, AND, NOP, RLA, NOP, AND, ROL, RLA,
            /* 0x4y */    RTI, EOR, KIL, SRE, NOP, EOR, LSR, SRE, PHA, EOR, LSR, ALR, JMP, EOR, LSR, SRE,
            /* 0x5y */    BVC, EOR, KIL, SRE, NOP, EOR, LSR, SRE, CLI, EOR, NOP, SRE, NOP, EOR, LSR, SRE,
            /* 0x6y */    RTS, ADC, KIL, RRA, NOP, ADC, ROR, RRA, PLA, ADC, ROR, ARR, JMP, ADC, ROR, RRA,
            /* 0x7y */    BVS, ADC, KIL, RRA, NOP, ADC, ROR, RRA, SEI, ADC, NOP, RRA, NOP, ADC, ROR, RRA,
            /* 0x8y */    NOP, STA, NOP, SAX, STY, STA, STX, SAX, DEY, NOP, TXA, XAA, STY, STA, STX, SAX,
            /* 0x9y */    BCC, STA, KIL, AHX, STY, STA, STX, SAX, TYA, STA, TXS, TAS, SHY, STA, SHX, AHX,
            /* 0xAy */    LDY, LDA, LDX, LAX, LDY, LDA, LDX, LAX, TAY, LDA, TAX, LAX, LDY, LDA, LDX, LAX,
            /* 0xBy */    BCS, LDA, KIL, LAX, LDY, LDA, LDX, LAX, CLV, LDA, TSX, LAS, LDY, LDA, LDX, LAX,
            /* 0xCy */    CPY, CMP, NOP, DCP, CPY, CMP, DEC, DCP, INY, CMP, DEX, AXS, CPY, CMP, DEC, DCP,
            /* 0xDy */    BNE, CMP, KIL, DCP, NOP, CMP, DEC, DCP, CLD, CMP, NOP, DCP, NOP, CMP, DEC, DCP,
            /* 0xEy */    CPX, SBC, NOP, ISC, CPX, SBC, INC, ISC, INX, SBC, NOP, SBC, CPX, SBC, INC, ISC,
            /* 0xFy */    BEQ, SBC, KIL, ISC, NOP, SBC, INC, ISC, SED, SBC, NOP, ISC, NOP, SBC, INC, ISC,
        };

        private static readonly Mos6502AddressingMode[] _addressingModeTable = new Mos6502AddressingMode[256] {
            /*         +0         +1               +2           +3               +4         +5         +6         +7 */ 
            /* 0x00 */ Implicit,  IndexedIndirect, Implicit,    IndexedIndirect, ZeroPage,  ZeroPage,  ZeroPage,  ZeroPage,
            /* 0x08 */ Implicit,  Immediate,       Accumulator, Immediate,       Absolute,  Absolute,  Absolute,  Absolute,
            /* 0x10 */ Relative,  IndirectIndexed, Implicit,    IndirectIndexed, ZeroPageX, ZeroPageX, ZeroPageX, ZeroPageX,
            /* 0x18 */ Implicit,  AbsoluteY,       Implicit,    AbsoluteY,       AbsoluteX, AbsoluteX, AbsoluteX, AbsoluteX,
            /* 0x20 */ Absolute,  IndexedIndirect, Implicit,    IndexedIndirect, ZeroPage,  ZeroPage,  ZeroPage,  ZeroPage,
            /* 0x28 */ Implicit,  Immediate,       Accumulator, Immediate,       Absolute,  Absolute,  Absolute,  Absolute,
            /* 0x30 */ Relative,  IndirectIndexed, Implicit,    IndirectIndexed, ZeroPageX, ZeroPageX, ZeroPageX, ZeroPageX,
            /* 0x38 */ Implicit,  AbsoluteY,       Implicit,    AbsoluteY,       AbsoluteX, AbsoluteX, AbsoluteX, AbsoluteX,
            /* 0x40 */ Implicit,  IndexedIndirect, Implicit,    IndexedIndirect, ZeroPage,  ZeroPage,  ZeroPage,  ZeroPage,
            /* 0x48 */ Implicit,  Immediate,       Accumulator, Immediate,       Absolute,  Absolute,  Absolute,  Absolute,
            /* 0x50 */ Relative,  IndirectIndexed, Implicit,    IndirectIndexed, ZeroPageX, ZeroPageX, ZeroPageX, ZeroPageX,
            /* 0x58 */ Implicit,  AbsoluteY,       Implicit,    AbsoluteY,       AbsoluteX, AbsoluteX, AbsoluteX, AbsoluteX,
            /* 0x60 */ Implicit,  IndexedIndirect, Implicit,    IndexedIndirect, ZeroPage,  ZeroPage,  ZeroPage,  ZeroPage,
            /* 0x68 */ Implicit,  Immediate,       Accumulator, Immediate,       Indirect,  Absolute,  Absolute,  Absolute,
            /* 0x70 */ Relative,  IndirectIndexed, Implicit,    IndirectIndexed, ZeroPageX, ZeroPageX, ZeroPageX, ZeroPageX,
            /* 0x78 */ Implicit,  AbsoluteY,       Implicit,    AbsoluteY,       AbsoluteX, AbsoluteX, AbsoluteX, AbsoluteX,
            /* 0x80 */ Immediate, IndexedIndirect, Immediate,   IndexedIndirect, ZeroPage,  ZeroPage,  ZeroPage,  ZeroPage,
            /* 0x88 */ Implicit,  Immediate,       Implicit,    Immediate,       Absolute,  Absolute,  Absolute,  Absolute,
            /* 0x90 */ Relative,  IndirectIndexed, Implicit,    IndirectIndexed, ZeroPageX, ZeroPageX, ZeroPageY, ZeroPageY,
            /* 0x98 */ Implicit,  AbsoluteY,       Implicit,    AbsoluteY,       AbsoluteX, AbsoluteX, AbsoluteY, AbsoluteY,
            /* 0xA0 */ Immediate, IndexedIndirect, Immediate,   IndexedIndirect, ZeroPage,  ZeroPage,  ZeroPage,  ZeroPage,
            /* 0xA8 */ Implicit,  Immediate,       Implicit,    Immediate,       Absolute,  Absolute,  Absolute,  Absolute,
            /* 0xB0 */ Relative,  IndirectIndexed, Implicit,    IndirectIndexed, ZeroPageX, ZeroPageX, ZeroPageY, ZeroPageY,
            /* 0xB8 */ Implicit,  AbsoluteY,       Implicit,    AbsoluteY,       AbsoluteX, AbsoluteX, AbsoluteY, AbsoluteY,
            /* 0xC0 */ Immediate, IndexedIndirect, Immediate,   IndexedIndirect, ZeroPage,  ZeroPage,  ZeroPage,  ZeroPage,
            /* 0xC8 */ Implicit,  Immediate,       Implicit,    Immediate,       Absolute,  Absolute,  Absolute,  Absolute,
            /* 0xD0 */ Relative,  IndirectIndexed, Implicit,    IndirectIndexed, ZeroPageX, ZeroPageX, ZeroPageX, ZeroPageX,
            /* 0xD8 */ Implicit,  AbsoluteY,       Implicit,    AbsoluteY,       AbsoluteX, AbsoluteX, AbsoluteX, AbsoluteX,
            /* 0xE0 */ Immediate, IndexedIndirect, Immediate,   IndexedIndirect, ZeroPage,  ZeroPage,  ZeroPage,  ZeroPage,
            /* 0xE8 */ Implicit,  Immediate,       Implicit,    Immediate,       Absolute,  Absolute,  Absolute,  Absolute,
            /* 0xF0 */ Relative,  IndirectIndexed, Implicit,    IndirectIndexed, ZeroPageX, ZeroPageX, ZeroPageX, ZeroPageX,
            /* 0xF8 */ Implicit,  AbsoluteY,       Implicit,    AbsoluteY,       AbsoluteX, AbsoluteX, AbsoluteX, AbsoluteX,
        };

        private static readonly int[] _cycleCountTable = new int[256] {
            /*         0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F */
            /* 0x0y */ 7, 6, 2, 8, 3, 3, 5, 5, 3, 2, 2, 2, 4, 4, 6, 6,
            /* 0x1y */ 2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            /* 0x2y */ 6, 6, 2, 8, 3, 3, 5, 5, 4, 2, 2, 2, 4, 4, 6, 6,
            /* 0x3y */ 2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            /* 0x4y */ 6, 6, 2, 8, 3, 3, 5, 5, 3, 2, 2, 2, 3, 4, 6, 6,
            /* 0x5y */ 2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            /* 0x6y */ 6, 6, 2, 8, 3, 3, 5, 5, 4, 2, 2, 2, 5, 4, 6, 6,
            /* 0x7y */ 2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            /* 0x8y */ 2, 6, 2, 6, 3, 3, 3, 3, 2, 2, 2, 2, 4, 4, 4, 4,
            /* 0x9y */ 2, 6, 2, 6, 4, 4, 4, 4, 2, 5, 2, 5, 5, 5, 5, 5,
            /* 0xAy */ 2, 6, 2, 6, 3, 3, 3, 3, 2, 2, 2, 2, 4, 4, 4, 4,
            /* 0xBy */ 2, 5, 2, 5, 4, 4, 4, 4, 2, 4, 2, 4, 4, 4, 4, 4,
            /* 0xCy */ 2, 6, 2, 8, 3, 3, 5, 5, 2, 2, 2, 2, 4, 4, 6, 6,
            /* 0xDy */ 2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            /* 0xEy */ 2, 6, 2, 8, 3, 3, 5, 5, 2, 2, 2, 2, 4, 4, 6, 6,
            /* 0xFy */ 2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
        };

        private static readonly int[] _instructionSizeTable = new int[256] {
            /*         0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F */
            /* 0x0y */ 2, 2, 0, 0, 2, 2, 2, 0, 1, 2, 1, 0, 3, 3, 3, 0,
            /* 0x1y */ 2, 2, 0, 0, 2, 2, 2, 0, 1, 3, 1, 0, 3, 3, 3, 0,
            /* 0x2y */ 3, 2, 0, 0, 2, 2, 2, 0, 1, 2, 1, 0, 3, 3, 3, 0,
            /* 0x3y */ 2, 2, 0, 0, 2, 2, 2, 0, 1, 3, 1, 0, 3, 3, 3, 0,
            /* 0x4y */ 1, 2, 0, 0, 2, 2, 2, 0, 1, 2, 1, 0, 3, 3, 3, 0,
            /* 0x5y */ 2, 2, 0, 0, 2, 2, 2, 0, 1, 3, 1, 0, 3, 3, 3, 0,
            /* 0x6y */ 1, 2, 0, 0, 2, 2, 2, 0, 1, 2, 1, 0, 3, 3, 3, 0,
            /* 0x7y */ 2, 2, 0, 0, 2, 2, 2, 0, 1, 3, 1, 0, 3, 3, 3, 0,
            /* 0x8y */ 2, 2, 0, 0, 2, 2, 2, 0, 1, 0, 1, 0, 3, 3, 3, 0,
            /* 0x9y */ 2, 2, 0, 0, 2, 2, 2, 0, 1, 3, 1, 0, 0, 3, 0, 3,
            /* 0xAy */ 2, 2, 2, 0, 2, 2, 2, 0, 1, 2, 1, 0, 3, 3, 3, 0,
            /* 0xBy */ 2, 2, 0, 0, 2, 2, 2, 0, 1, 3, 1, 0, 3, 3, 3, 0,
            /* 0xCy */ 2, 2, 0, 0, 2, 2, 2, 0, 1, 2, 1, 0, 3, 3, 3, 0,
            /* 0xDy */ 2, 2, 0, 0, 2, 2, 2, 0, 1, 3, 1, 0, 3, 3, 3, 0,
            /* 0xEy */ 2, 2, 0, 0, 2, 2, 2, 0, 1, 2, 1, 0, 3, 3, 3, 0,
            /* 0xFy */ 2, 2, 0, 0, 2, 2, 2, 0, 1, 3, 1, 0, 3, 3, 3, 0,
        };
    }
}