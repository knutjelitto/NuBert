namespace Pliant.RegularExpressions
{
    public abstract class RegexCharacters : IRegexNode
    {
    }

    public sealed class RegexCharactersUnit : RegexCharacters
    {
        public RegexCharactersUnit(RegexCharacterClassCharacter character)
        {
            Character = character;
        }

        public RegexCharacterClassCharacter Character { get; }

        public override bool Equals(object obj)
        {
            return obj is RegexCharactersUnit other &&
                   other.Character.Equals(Character);
        }

        public override int GetHashCode()
        {
            return Character.GetHashCode();
        }

        public override string ToString()
        {
            return Character.ToString();
        }
    }

    public sealed class RegexCharactersRange : RegexCharacters
    {
        public RegexCharactersRange(
            RegexCharacterClassCharacter startCharacter,
            RegexCharacterClassCharacter endCharacter)
        {
            StartCharacter = startCharacter;
            EndCharacter = endCharacter;
        }

        public RegexCharacterClassCharacter StartCharacter { get; }
        public RegexCharacterClassCharacter EndCharacter { get; }

        public override bool Equals(object obj)
        {
            return obj is RegexCharactersRange other && 
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