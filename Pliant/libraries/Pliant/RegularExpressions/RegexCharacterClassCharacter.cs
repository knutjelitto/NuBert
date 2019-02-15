using Pliant.Utilities;

namespace Pliant.RegularExpressions
{
    public class RegexCharacterClassCharacter : RegexNode
    {
        private readonly int _hashCode;

        public char Value { get; private set; }

        public bool IsEscaped { get; private set; }

        public override RegexNodeType NodeType => RegexNodeType.RegexCharacterClassCharacter;

        public RegexCharacterClassCharacter(char value, bool isEscaped = false)
        {
            Value = value;
            IsEscaped = isEscaped;
            this._hashCode = ComputeHashCode();
        }
        
        private int ComputeHashCode()
        {
            return HashCode.Compute(
                    Value.GetHashCode());
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

            var characterClassCharacter = obj as RegexCharacterClassCharacter;
            if (characterClassCharacter == null)
            {
                return false;
            }

            return characterClassCharacter.Value.Equals(Value)
                && characterClassCharacter.IsEscaped.Equals(IsEscaped);
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