namespace Nest.Hardware.Mos6502
{
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
}