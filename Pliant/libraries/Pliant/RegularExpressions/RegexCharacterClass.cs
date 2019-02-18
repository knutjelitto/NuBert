using Pliant.Utilities;

namespace Pliant.RegularExpressions
{
    public class RegexCharacterClass : RegexNode
    {
        public RegexCharacterClass(RegexCharacterUnitRange characterRange)
        {
            CharacterRange = characterRange;
        }

        public RegexCharacterUnitRange CharacterRange { get; }

        public override int GetHashCode()
        {
            return CharacterRange.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is RegexCharacterClass other && 
                   other.CharacterRange.Equals(CharacterRange);
        }

        public override string ToString()
        {
            return CharacterRange.ToString();
        }
    }

    public class RegexCharacterClassAlteration : RegexCharacterClass
    {
        public RegexCharacterClassAlteration(
            RegexCharacterUnitRange characterRange,
            RegexCharacterClass characterClass)
            : base(characterRange)
        {
            CharacterClass = characterClass;
        }

        public RegexCharacterClass CharacterClass { get; }

        public override int GetHashCode()
        {
            return (CharacterRange, CharacterClass).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is RegexCharacterClassAlteration other && 
                   other.CharacterRange.Equals(CharacterRange) && 
                   other.CharacterClass.Equals(CharacterClass);
        }

        public override string ToString()
        {
            return $"{CharacterRange}{CharacterClass}";
        }
    }
}