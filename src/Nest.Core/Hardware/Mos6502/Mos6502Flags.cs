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

        // Utility methods.
        // Some simple examples (see sharplab link below) indicate that these methods
        // should be inlined during JIT (even without AggressiveInlining) so it seems
        // reasonable to have these utilities.
        // See https://sharplab.io/#v2:C4LghgzgtgPgAgJgIwFgBQcAMACOSB0ASgK4B2wAllAKb4DCA9lAA4UA21ATgMpcBuFAMbUIAbnToA2gDE2YAOYQAuujgBmbNVLEo2ALIMIANgCsmBLIUR0Ab3TYH2AHINS1bAF5smAEaZ/mAD6AZgANPaOdGCcnACent5+AcH+SOFojtgAWlwMCb4hKUhhEQ4AkuRcnMTMwAAiFBBgPhz5Sf7BxSUZjnXUglRgbG2FXWHYAPQTzgzA2MQQ1AAm2K7OAKLcpZk7u3sOU9i8cwDyTtjAABbuEMBgggDW2ADu16QX19gAZnKK2NHuZgLa4rYB5K43O6PbA+eJgd4UUi3aqCSiubb7TG7Q50DjRbBnD6Q+5PV5aInfX4Qf6cQHA5YXcGfW4kmFwhGVGI1YAYrF8ybTAASkGwpDy1C+X36czWEOwdAACgBVfDbABCtLATy8BVSKX86Uyh2OBPOcpZ0LJ7zlPysNLpEBBjIpFoe20OwupYs0kulq2tn0VSu2hBE/AZOqSxX13UyJz4XB+DGeIy6McNjic1HkYEoCbaY3TEh6DgVya4SuYCVDi04CZWMGwGuoWuwjYqwCq3IaTRa1HSAF9i+pcEgjLgENh1tg7GhDo5JHpqFcGEsyiw2AAKJcrtcbk61CiuCD4ACC8nktIgEAoCYqbERiPkAEoVCXcBo8OODMYzBYqUcy5lF8m5XI0+iGKY5iWH8iJAsAoQwgwDDDIIrhLBQaKkIhP5Qf+dpgscz6eAAfLyDhoaQGFYdgAD82BwcQcyNoRy7YCADGkPB2AAGTYAAfqxwDiGgQ5oKon6TnQM7bCOX4Qb+0EAUqiw7pcq6boicx8EMOGQX+MHUraijEbO+xwAA7JSVj4McwGbjpwweDqemKfhij4DknAMM+ImZGJmSyZ+Y4KXhhnYPeiLUJp5DYI5rlhQBjHAKZ5G4FZjmeDqdGcdxja4QZVKebk7G5UxvECQVSk2V5DB+Y4YkDkAA==
        public static Mos6502Flags Set(this Mos6502Flags val, Mos6502Flags toSet) => val | toSet;
        public static Mos6502Flags Clear(this Mos6502Flags val, Mos6502Flags toClear) => val & ~toClear;
        public static Mos6502Flags SetIf(this Mos6502Flags val, bool condition, Mos6502Flags flag) =>
            condition ? val.Set(flag) : val.Clear(flag);
        public static bool IsSet(this Mos6502Flags val, Mos6502Flags flag) => (val & flag) != 0;
    }
}