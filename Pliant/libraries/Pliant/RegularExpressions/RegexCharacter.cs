namespace Pliant.RegularExpressions
{
    public class RegexCharacter : RegexNode
    {
        public RegexCharacter(char value, bool isEscaped = false)
        {
            Value = value;
            IsEscaped = isEscaped;
        }

        public char Value { get; }

        public bool IsEscaped { get; }

        public override RegexNodeType NodeType => RegexNodeType.RegexCharacterClassCharacter;

        public override int GetHashCode()
        {
            return (Value, IsEscaped).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is RegexCharacter regexCharacter && 
                   regexCharacter.Value.Equals(Value) && 
                   regexCharacter.IsEscaped.Equals(IsEscaped);
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