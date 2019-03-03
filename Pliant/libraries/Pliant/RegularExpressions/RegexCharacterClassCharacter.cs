namespace Pliant.RegularExpressions
{
    public sealed class RegexCharacterClassCharacter : IRegexNode
    {
        public RegexCharacterClassCharacter(char value, bool isEscaped = false)
        {
            Value = value;
            IsEscaped = isEscaped;
        }

        public char Value { get; }

        public bool IsEscaped { get; }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is RegexCharacterClassCharacter other && 
                   other.Value.Equals(Value) && 
                   other.IsEscaped.Equals(IsEscaped);
        }

        public override string ToString()
        {
            if (IsEscaped)
            {
                return $"\\{Value}";
            }

            return Value.ToString();
        }
    }
}