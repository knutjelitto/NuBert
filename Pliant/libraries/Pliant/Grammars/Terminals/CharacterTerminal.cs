using System.Collections.Generic;

namespace Pliant.Terminals
{
    public sealed class CharacterTerminal : AtomTerminal
    {
        public CharacterTerminal(char character)
        {
            Character = character;
        }

        public char Character { get; }

        public override bool Equals(object obj)
        {
            return obj is CharacterTerminal other && other.Character.Equals(Character);
        }

        public override int GetHashCode()
        {
            return Character.GetHashCode();
        }

        public override IReadOnlyList<Interval> GetIntervals()
        {
            return this.intervals ?? (this.intervals = new[] {new Interval(Character)});
        }

        public override bool CanApply(char character)
        {
            return Character == character;
        }

        public override string ToString()
        {
            return Character.ToString();
        }

        private Interval[] intervals;
    }
}