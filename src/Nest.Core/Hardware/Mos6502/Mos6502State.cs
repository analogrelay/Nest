using System;

namespace Nest.Hardware.Mos6502
{
    // http://wiki.nesdev.com/w/index.php/CPU_registers
    public readonly struct Mos6502State : IEquatable<Mos6502State>
    {
        public static readonly Mos6502State PowerUp =
            new Mos6502State(a: 0, x: 0, y: 0, pc: 0, s: 0xFD, p: Mos6502Flags.PowerUp);

        public Mos6502State(int a, int x, int y, int pc, int s, Mos6502Flags p)
        {
            // PERF: Consider eliding bounds checks when internal operations are modifying the state.
            if (a > byte.MaxValue || a < byte.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(a), $"Value '{a}' is out of the allowed range for register A.");
            }

            if (x > byte.MaxValue || x < byte.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(x), $"Value '{x}' is out of the allowed range for register X.");
            }

            if (y > byte.MaxValue || y < byte.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(y), $"Value '{y}' is out of the allowed range for register Y.");
            }

            if (pc > ushort.MaxValue || pc < ushort.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(pc), $"Value '{pc}' is out of the allowed range (${ushort.MinValue:X4}-${ushort.MaxValue:X4}) for register PC.");
            }

            if (s > byte.MaxValue || s < byte.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(s), $"Value '{s}' is out of the allowed range for register S.");
            }

            A = a;
            X = x;
            Y = y;
            PC = pc;
            S = s;
            P = p;
        }

        public int A { get; }
        public int X { get; }
        public int Y { get; }
        public int PC { get; }
        public int S { get; }
        public Mos6502Flags P { get; }

        public override bool Equals(object? obj) => obj is Mos6502State state && Equals(state);

        public bool Equals(Mos6502State other) =>
                   A == other.A &&
                   X == other.X &&
                   Y == other.Y &&
                   PC == other.PC &&
                   S == other.S &&
                   P == other.P;

        public override int GetHashCode() => HashCode.Combine(A, X, Y, PC, S, P);

        public override string ToString() =>
            $"[A:${A:X2} X:${X:X2} Y:${Y:X2} PC:${PC:X4} S:${S:X2} P:${(byte)P:X2} ({P.ToDisplayString()})";

        public Mos6502State With(int? a = null, int? x = null, int? y = null, int? pc = null, int? s = null, Mos6502Flags? p = null)
        {
            return new Mos6502State(a ?? A, x ?? X, y ?? Y, pc ?? PC, s ?? S, p ?? P);
        }
    }

}