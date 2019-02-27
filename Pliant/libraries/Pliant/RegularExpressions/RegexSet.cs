using Pliant.Utilities;

namespace Pliant.RegularExpressions
{
    public class RegexSet : RegexNode
    {
        public RegexSet(bool negate, RegexCharacterClass characterClass)
        {
            Negate = negate;
            CharacterClass = characterClass;
            this.hashCode = ComputeHashCode();
        }

        public bool Negate { get; }
        public RegexCharacterClass CharacterClass { get; }

        public override bool Equals(object obj)
        {
            return obj is RegexSet other &&
                   Negate.Equals(other.Negate) &&
                   CharacterClass.Equals(other.CharacterClass);
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                Negate.GetHashCode(),
                CharacterClass.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override string ToString()
        {
            return $"[{(Negate ? "^" : string.Empty)}{CharacterClass}]";
        }

        private readonly int hashCode;
    }
}