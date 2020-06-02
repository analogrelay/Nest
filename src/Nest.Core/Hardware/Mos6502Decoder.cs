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
                (int)opcode,
                _operationTable[opcode],
                _addressingModeTable[opcode],
                _cycleCountTable[opcode]);
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

        public override string ToString() => $"${Opcode:X2} {Operation} ({AddressingMode}, Base Cycles: {CycleCount})";
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
        ADC = 0,
        AHX = 1,
        ALR = 2,
        ANC = 3,
        AND = 4,
        ARR = 5,
        ASL = 6,
        AXS = 7,
        BCC = 8,
        BCS = 9,
        BEQ = 10,
        BIT = 11,
        BMI = 12,
        BNE = 13,
        BPL = 14,
        BRK = 15,
        BVC = 16,
        BVS = 17,
        CLC = 18,
        CLD = 19,
        CLI = 20,
        CLV = 21,
        CMP = 22,
        CPX = 23,
        CPY = 24,
        DCP = 25,
        DEC = 26,
        DEX = 27,
        DEY = 28,
        EOR = 29,
        INC = 30,
        INX = 31,
        INY = 32,
        ISC = 33,
        JMP = 34,
        JSR = 35,
        KIL = 36,
        LAS = 37,
        LAX = 38,
        LDA = 39,
        LDX = 40,
        LDY = 41,
        LSR = 42,
        NOP = 43,
        ORA = 44,
        PHA = 45,
        PHP = 46,
        PLA = 47,
        PLP = 48,
        RLA = 49,
        ROL = 50,
        ROR = 51,
        RRA = 52,
        RTI = 53,
        RTS = 54,
        SAX = 55,
        SBC = 56,
        SEC = 57,
        SED = 58,
        SEI = 58,
        SHX = 59,
        SHY = 60,
        SLO = 61,
        SRE = 62,
        STA = 63,
        STX = 64,
        STY = 65,
        TAS = 66,
        TAX = 67,
        TAY = 68,
        TSX = 69,
        TXA = 70,
        TXS = 71,
        TYA = 72,
        XAA = 73,
    }
}