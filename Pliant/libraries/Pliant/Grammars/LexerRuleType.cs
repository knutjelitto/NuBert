namespace Pliant.Grammars
{
    public class LexerRuleType
    {
        public LexerRuleType(string id)
        {
            Id = id;
            this._hashCode = ComputeHashCode(id);
        }

        public string Id { get; }

        public static bool operator ==(LexerRuleType first, LexerRuleType second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(LexerRuleType first, LexerRuleType second)
        {
            return !first.Equals(second);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as LexerRuleType;

            if ((object) other == null)
            {
                return false;
            }

            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        private static int ComputeHashCode(string id)
        {
            return id.GetHashCode();
        }

        private readonly int _hashCode;
    }
}