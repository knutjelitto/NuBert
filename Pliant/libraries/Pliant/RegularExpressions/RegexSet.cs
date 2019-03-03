namespace Pliant.RegularExpressions
{
    public class RegexSet : IRegexNode
    {
        public RegexSet(RegexCharacterClass characterClass, bool negate)
        {
            Negate = negate;
            CharacterClass = characterClass;
            this.hashCode = ComputeHashCode();
        }

        public RegexCharacterClass CharacterClass { get; }
        public bool Negate { get; }

        public override bool Equals(object obj)
        {
            return obj is RegexSet other &&
                   CharacterClass.Equals(other.CharacterClass) &&
                   Negate.Equals(other.Negate);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override string ToString()
        {
            return $"[{(Negate ? "^" : string.Empty)}{CharacterClass}]";
        }

        private int ComputeHashCode()
        {
            return (CharacterClass, Negate).GetHashCode();
        }

        private readonly int hashCode;
    }
}