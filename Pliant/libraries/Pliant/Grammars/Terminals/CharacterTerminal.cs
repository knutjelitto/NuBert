using System.Collections.Generic;

namespace Pliant.Grammars
{
    public sealed class CharacterTerminal : Terminal
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
            return this._intervals ?? (this._intervals = new[] {new Interval(Character)});
        }

        public override bool IsMatch(char character)
        {
            return Character == character;
        }

        public override string ToString()
        {
            return Character.ToString();
        }

        private Interval[] _intervals;
    }
}