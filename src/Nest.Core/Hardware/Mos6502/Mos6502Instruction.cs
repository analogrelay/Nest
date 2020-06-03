namespace Nest.Hardware.Mos6502
{
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
}