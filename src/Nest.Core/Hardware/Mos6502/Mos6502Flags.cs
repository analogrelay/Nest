using System.Text;

namespace Nest.Hardware.Mos6502
{
    // http://wiki.nesdev.com/w/index.php/Status_flags
    public enum Mos6502Flags
    {
        None = 0b0000_0000,
        Carry = 0b0000_0001,
        Zero = 0b0000_0010,
        InterruptDisable = 0b0000_0100,
        Decimal = 0b0000_1000, // Not used on NES
                               // Set ON the stack when the flags are pushed to the stack by an instruction
                               // Clear ON the stack when the flags are pushed to the stack by an interrupt
                               // Has no effect on the CPU.
        Break = 0b0001_0000,
        // Set ON the stack when the flags are pushed to the stack
        // Has no effect on the CPU
        Reserved = 0b0010_0000,
        Overflow = 0b0100_0000,
        Negative = 0b1000_0000,

        PowerUp = Reserved | Break | InterruptDisable,
    }

    public static class Mos6502Mos6502FlagsExtensions
    {
        public static string ToDisplayString(this Mos6502Flags flags)
        {
            var builder = new StringBuilder();
            builder.Append((flags & Mos6502Flags.Carry) != 0 ? "C" : "c");
            builder.Append((flags & Mos6502Flags.Zero) != 0 ? "Z" : "z");
            builder.Append((flags & Mos6502Flags.InterruptDisable) != 0 ? "I" : "i");
            builder.Append((flags & Mos6502Flags.Decimal) != 0 ? "D" : "d");
            builder.Append((flags & Mos6502Flags.Break) != 0 ? "B" : "b");
            builder.Append((flags & Mos6502Flags.Reserved) != 0 ? "R" : "r");
            builder.Append((flags & Mos6502Flags.Overflow) != 0 ? "V" : "v");
            builder.Append((flags & Mos6502Flags.Negative) != 0 ? "N" : "n");
            return builder.ToString();
        }
    }
}