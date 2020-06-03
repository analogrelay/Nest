using static Nest.Hardware.Mos6502.Mos6502Operation;
using static Nest.Hardware.Mos6502.Mos6502AddressingMode;

namespace Nest.Hardware.Mos6502
{
    public static partial class Mos6502Decoder
    {
        // Borrowed mostly from https://github.com/fogleman/nes/blob/6d86d3ba286ec0e81acea05423f60455c222035e/nes/cpu.go
        // Which is licensed under the MIT License: https://github.com/fogleman/nes/blob/6d86d3ba286ec0e81acea05423f60455c222035e/LICENSE.md
        private static readonly Mos6502Operation[] _operationTable = new Mos6502Operation[] {
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

        private static readonly Mos6502AddressingMode[] _addressingModeTable = new Mos6502AddressingMode[] {
            Implicit, IndexedIndirect, Implicit, IndexedIndirect, ZeroPage, ZeroPage, ZeroPage, ZeroPage,
            Implicit, Immediate, Accumulator, Immediate, Absolute, Absolute, Absolute, Absolute,
            Relative, IndirectIndexed, Implicit, IndirectIndexed, ZeroPageX, ZeroPageX, ZeroPageX, ZeroPageX,
            Implicit, AbsoluteY, Implicit, AbsoluteY, AbsoluteX, AbsoluteX, AbsoluteX, AbsoluteX,
            Absolute, IndexedIndirect, Implicit, IndexedIndirect, ZeroPage, ZeroPage, ZeroPage, ZeroPage,
            Implicit, Immediate, Accumulator, Immediate, Absolute, Absolute, Absolute, Absolute,
            Relative, IndirectIndexed, Implicit, IndirectIndexed, ZeroPageX, ZeroPageX, ZeroPageX, ZeroPageX,
            Implicit, AbsoluteY, Implicit, AbsoluteY, AbsoluteX, AbsoluteX, AbsoluteX, AbsoluteX,
            Implicit, IndexedIndirect, Implicit, IndexedIndirect, ZeroPage, ZeroPage, ZeroPage, ZeroPage,
            Implicit, Immediate, Accumulator, Immediate, Absolute, Absolute, Absolute, Absolute,
            Relative, IndirectIndexed, Implicit, IndirectIndexed, ZeroPageX, ZeroPageX, ZeroPageX, ZeroPageX,
            Implicit, AbsoluteY, Implicit, AbsoluteY, AbsoluteX, AbsoluteX, AbsoluteX, AbsoluteX,
            Implicit, IndexedIndirect, Implicit, IndexedIndirect, ZeroPage, ZeroPage, ZeroPage, ZeroPage,
            Implicit, Immediate, Accumulator, Immediate, Indirect, Absolute, Absolute, Absolute,
            Relative, IndirectIndexed, Implicit, IndirectIndexed, ZeroPageX, ZeroPageX, ZeroPageX, ZeroPageX,
            Implicit, AbsoluteY, Implicit, AbsoluteY, AbsoluteX, AbsoluteX, AbsoluteX, AbsoluteX,
            Immediate, IndexedIndirect, Immediate, IndexedIndirect, ZeroPage, ZeroPage, ZeroPage, ZeroPage,
            Implicit, Immediate, Implicit, Immediate, Absolute, Absolute, Absolute, Absolute,
            Relative, IndirectIndexed, Implicit, IndirectIndexed, ZeroPageX, ZeroPageX, ZeroPageY, ZeroPageY,
            Implicit, AbsoluteY, Implicit, AbsoluteY, AbsoluteX, AbsoluteX, AbsoluteY, AbsoluteY,
            Immediate, IndexedIndirect, Immediate, IndexedIndirect, ZeroPage, ZeroPage, ZeroPage, ZeroPage,
            Implicit, Immediate, Implicit, Immediate, Absolute, Absolute, Absolute, Absolute,
            Relative, IndirectIndexed, Implicit, IndirectIndexed, ZeroPageX, ZeroPageX, ZeroPageY, ZeroPageY,
            Implicit, AbsoluteY, Implicit, AbsoluteY, AbsoluteX, AbsoluteX, AbsoluteY, AbsoluteY,
            Immediate, IndexedIndirect, Immediate, IndexedIndirect, ZeroPage, ZeroPage, ZeroPage, ZeroPage,
            Implicit, Immediate, Implicit, Immediate, Absolute, Absolute, Absolute, Absolute,
            Relative, IndirectIndexed, Implicit, IndirectIndexed, ZeroPageX, ZeroPageX, ZeroPageX, ZeroPageX,
            Implicit, AbsoluteY, Implicit, AbsoluteY, AbsoluteX, AbsoluteX, AbsoluteX, AbsoluteX,
            Immediate, IndexedIndirect, Immediate, IndexedIndirect, ZeroPage, ZeroPage, ZeroPage, ZeroPage,
            Implicit, Immediate, Implicit, Immediate, Absolute, Absolute, Absolute, Absolute,
            Relative, IndirectIndexed, Implicit, IndirectIndexed, ZeroPageX, ZeroPageX, ZeroPageX, ZeroPageX,
            Implicit, AbsoluteY, Implicit, AbsoluteY, AbsoluteX, AbsoluteX, AbsoluteX, AbsoluteX,
        };

        private static readonly int[] _cycleCountTable = new[] {
            7, 6, 2, 8, 3, 3, 5, 5, 3, 2, 2, 2, 4, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            6, 6, 2, 8, 3, 3, 5, 5, 4, 2, 2, 2, 4, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            6, 6, 2, 8, 3, 3, 5, 5, 3, 2, 2, 2, 3, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            6, 6, 2, 8, 3, 3, 5, 5, 4, 2, 2, 2, 5, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            2, 6, 2, 6, 3, 3, 3, 3, 2, 2, 2, 2, 4, 4, 4, 4,
            2, 6, 2, 6, 4, 4, 4, 4, 2, 5, 2, 5, 5, 5, 5, 5,
            2, 6, 2, 6, 3, 3, 3, 3, 2, 2, 2, 2, 4, 4, 4, 4,
            2, 5, 2, 5, 4, 4, 4, 4, 2, 4, 2, 4, 4, 4, 4, 4,
            2, 6, 2, 8, 3, 3, 5, 5, 2, 2, 2, 2, 4, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            2, 6, 2, 8, 3, 3, 5, 5, 2, 2, 2, 2, 4, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
        };

        private static readonly int[] _operandSizeTable = new[] {
            1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 2, 2, 2, 0,
            1, 1, 0, 0, 1, 1, 1, 0, 0, 2, 0, 0, 2, 2, 2, 0,
            2, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 2, 2, 2, 0,
            1, 1, 0, 0, 1, 1, 1, 0, 0, 2, 0, 0, 2, 2, 2, 0,
            0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 2, 2, 2, 0,
            1, 1, 0, 0, 1, 1, 1, 0, 0, 2, 0, 0, 2, 2, 2, 0,
            0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 2, 2, 2, 0,
            1, 1, 0, 0, 1, 1, 1, 0, 0, 2, 0, 0, 2, 2, 2, 0,
            1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 2, 2, 2, 0,
            1, 1, 0, 0, 1, 1, 1, 0, 0, 2, 0, 0, 0, 2, 0, 0,
            1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 0, 0, 2, 2, 2, 0,
            1, 1, 0, 0, 1, 1, 1, 0, 0, 2, 0, 0, 2, 2, 2, 0,
            1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 2, 2, 2, 0,
            1, 1, 0, 0, 1, 1, 1, 0, 0, 2, 0, 0, 2, 2, 2, 0,
            1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 2, 2, 2, 0,
            1, 1, 0, 0, 1, 1, 1, 0, 0, 2, 0, 0, 2, 2, 2, 0,
        };
    }
}