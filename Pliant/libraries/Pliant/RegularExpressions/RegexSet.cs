namespace Pliant.RegularExpressions
{
    public class RegexSet : RegexNode
    {
        public RegexSet(bool negate, RegexCharacterClass characterClass)
        {
            Negate = negate;
            CharacterClass = characterClass;
        }

        public RegexCharacterClass CharacterClass { get; }
        public bool Negate { get; }

        public override bool Equals(object obj)
        {
            return obj is RegexSet other &&
                   Negate.Equals(other.Negate) &&
                   CharacterClass.Equals(other.CharacterClass);
        }

        public override int GetHashCode()
        {
            return (Negate, CharacterClass).GetHashCode();
        }

        public override string ToString()
        {
            return $"[{(Negate ? "^" : string.Empty)}{CharacterClass}]";
        }
    }
}