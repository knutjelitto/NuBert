using Pliant.Utilities;

namespace Pliant.RegularExpressions
{
    public class RegexCharacterUnitRange : RegexNode
    {
        public RegexCharacterClassCharacter StartCharacter { get; set; }

        public RegexCharacterUnitRange(RegexCharacterClassCharacter startCharacter)
        {
            StartCharacter = startCharacter;
            this._hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var characterRange = obj as RegexCharacterUnitRange;
            if (characterRange == null)
            {
                return false;
            }

            return characterRange.StartCharacter.Equals(StartCharacter);
        }

        private readonly int _hashCode;

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                    StartCharacter.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override RegexNodeType NodeType => RegexNodeType.RegexCharacterUnitRange;

        public override string ToString()
        {
            return StartCharacter.ToString();
        }
    }

    public class RegexCharacterRange : RegexCharacterUnitRange
    {
        public RegexCharacterClassCharacter EndCharacter { get; set; }

        public RegexCharacterRange(
            RegexCharacterClassCharacter startCharacter,
            RegexCharacterClassCharacter endCharacter)
            : base(startCharacter)
        {
            EndCharacter = endCharacter;
            this._hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var characterRangeSet = obj as RegexCharacterRange;
            if (characterRangeSet == null)
            {
                return false;
            }

            return
                StartCharacter.Equals(characterRangeSet.StartCharacter)
                && EndCharacter.Equals(characterRangeSet.EndCharacter);
        }
        
        private readonly int _hashCode;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                    StartCharacter.GetHashCode(),
                    EndCharacter.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override RegexNodeType NodeType => RegexNodeType.RegexCharacterRange;

        public override string ToString()
        {
            return $"{StartCharacter}-{EndCharacter}";
        }
    }
}