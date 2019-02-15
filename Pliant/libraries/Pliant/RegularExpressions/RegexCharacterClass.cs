using Pliant.Utilities;

namespace Pliant.RegularExpressions
{
    public class RegexCharacterClass : RegexNode
    {
        public RegexCharacterUnitRange CharacterRange { get; private set; }

        public RegexCharacterClass(RegexCharacterUnitRange characterRange)
        {
            CharacterRange = characterRange;
            this._hashCode = ComputeHashCode();
        }

        private readonly int _hashCode;

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                    CharacterRange.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var characterClass = obj as RegexCharacterClass;
            if (characterClass == null)
            {
                return false;
            }

            return characterClass.CharacterRange.Equals(CharacterRange);
        }

        public override RegexNodeType NodeType => RegexNodeType.RegexCharacterClass;

        public override string ToString()
        {
            return CharacterRange.ToString();
        }
    }

    public class RegexCharacterClassAlteration : RegexCharacterClass
    {
        public RegexCharacterClass CharacterClass { get; private set; }

        public RegexCharacterClassAlteration(
            RegexCharacterUnitRange characterRange,
            RegexCharacterClass characterClass)
            : base(characterRange)
        {
            CharacterClass = characterClass;
            this._hashCode = ComputeHashCode();
        }

        private readonly int _hashCode;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                    CharacterRange.GetHashCode(),
                    CharacterClass.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var characterClassList = obj as RegexCharacterClassAlteration;
            if (characterClassList == null)
            {
                return false;
            }

            return characterClassList.CharacterRange.Equals(CharacterRange)
                && characterClassList.CharacterClass.Equals(CharacterClass);
        }

        public override RegexNodeType NodeType => RegexNodeType.RegexCharacterClassAlteration;

        public override string ToString()
        {
            return $"{CharacterRange}{CharacterClass}";
        }
    }
}