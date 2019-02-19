namespace Pliant.RegularExpressions
{
    public class RegexCharacterUnitRange : RegexNode
    {
        public RegexCharacterUnitRange(RegexCharacterClassCharacter startCharacter)
        {
            StartCharacter = startCharacter;
        }

        public RegexCharacterClassCharacter StartCharacter { get; }

        public override RegexNodeType NodeType => RegexNodeType.RegexCharacterUnitRange;

        public override bool Equals(object obj)
        {
            return obj is RegexCharacterUnitRange other &&
                   other.StartCharacter.Equals(StartCharacter);
        }

        public override int GetHashCode()
        {
            return StartCharacter.GetHashCode();
        }

        public override string ToString()
        {
            return StartCharacter.ToString();
        }
    }

    public class RegexCharacterRange : RegexCharacterUnitRange
    {
        public RegexCharacterRange(
            RegexCharacterClassCharacter startCharacter,
            RegexCharacterClassCharacter endCharacter)
            : base(startCharacter)
        {
            EndCharacter = endCharacter;
        }

        public RegexCharacterClassCharacter EndCharacter { get; }

        public override RegexNodeType NodeType => RegexNodeType.RegexCharacterRange;

        public override bool Equals(object obj)
        {
            return obj is RegexCharacterRange other && 
                   StartCharacter.Equals(other.StartCharacter) && 
                   EndCharacter.Equals(other.EndCharacter);
        }

        public override int GetHashCode()
        {
            return (StartCharacter, EndCharacter).GetHashCode();
        }

        public override string ToString()
        {
            return $"{StartCharacter}-{EndCharacter}";
        }
    }
}