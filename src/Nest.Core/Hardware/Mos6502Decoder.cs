using System;
using Nest.Memory;

namespace Nest.Hardware
{
    public static partial class Mos6502Decoder
    {
        public static Mos6502Instruction Decode(MemoryUnit memory, int offset)
        {
            var opcode = memory.ReadByte(offset);

            return new Mos6502Instruction(
                opcode,
                _operationTable[opcode],
                _addressingModeTable[opcode],
                _cycleCountTable[opcode],
                _operandSizeTable[opcode]);
        }
    }

    public readonly struct Mos6502Instruction
    {
        public Mos6502Instruction(int opcode, Mos6502Operation operation, Mos6502AddressingMode addressingMode, int cycleCount)
        {
            Opcode = opcode;
            Operation = operation;
            AddressingMode = addressingMode;
            CycleCount = cycleCount;
        }

        public int Opcode { get; }
        public Mos6502Operation Operation { get; }
        public Mos6502AddressingMode AddressingMode { get; }
        public int CycleCount { get; }
    }

    public enum Mos6502AddressingMode
    {
        Implicit,
        Accumulator,
        Immediate,
        ZeroPage,
        ZeroPageY,
        ZeroPageX,
        Relative,
        Absolute,
        AbsoluteX,
        AbsoluteY,
        Indirect,
        IndexedIndirect,
        IndirectIndexed,
    }

    public enum Mos6502Operation
    {
        ADC,
        AHX,
        ALR,
        ANC,
        AND,
        ARR,
        ASL,
        AXS,
        BCC,
        BCS,
        BEQ,
        BIT,
        BMI,
        BNE,
        BPL,
        BRK,
        BVC,
        BVS,
        CLC,
        CLD,
        CLI,
        CLV,
        CMP,
        CPX,
        CPY,
        DCP,
        DEC,
        DEX,
        DEY,
        EOR,
        INC,
        INX,
        INY,
        ISC,
        JMP,
        JSR,
        KIL,
        LAS,
        LAX,
        LDA,
        LDX,
        LDY,
        LSR,
        NOP,
        ORA,
        PHA,
        PHP,
        PLA,
        PLP,
        RLA,
        ROL,
        ROR,
        RRA,
        RTI,
        RTS,
        SAX,
        SBC,
        SEC,
        SED,
        SEI,
        SHX
        SHY,
        SLO,
        SRE,
        STA,
        STX,
        STY,
        TAS,
        TAX,
        TAY,
        TSX,
        TXA,
        TXS,
        TYA,
        XAA,
    }
}