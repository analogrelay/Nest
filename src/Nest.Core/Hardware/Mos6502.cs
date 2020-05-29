namespace Nest.Hardware
{
    public class Mos6502
    {
        public State CurrentState { get; }

        // http://wiki.nesdev.com/w/index.php/CPU_registers
        public struct State
        {
            public byte A { get; }
            public byte X { get; }
            public byte Y { get; }
            public byte PC { get; }
            public byte S { get; }
            public Mos6502.Flags P { get; }
        }

        // http://wiki.nesdev.com/w/index.php/Status_flags
        public enum Flags
        {
            Carry = 0b0000_0001,
            Zero = 0b0000_0010,
            InterruptDisable = 0b0000_0100,
            Decimal = 0b0000_1000, // Not used on NES
            Overflow = 0b0100_0000,
            Negative = 0b1000_0000
        }
    }
}