using Pliant.Utilities;

namespace Pliant.RegularExpressions
{
    public class RegexSet : RegexNode
    {
        public bool Negate { get; private set; }

        public RegexCharacterClass CharacterClass { get; private set; }

        public override RegexNodeType NodeType => RegexNodeType.RegexSet;

        public RegexSet(bool negate, RegexCharacterClass characterClass)
        {
            Negate = negate;
            CharacterClass = characterClass;
            this._hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var set = obj as RegexSet;
            if (set == null)
            {
                return false;
            }

            return CharacterClass.Equals(set.CharacterClass)
                && Negate.Equals(set.Negate);
        }
        
        private readonly int _hashCode ;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                Negate.GetHashCode(),
                CharacterClass.GetHashCode());
        }
        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return $"[{(Negate ? "^" : string.Empty)}{CharacterClass}]";
        }
    }
}