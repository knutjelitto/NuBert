using Pliant.Utilities;

namespace Pliant.Tokens
{
    public sealed class TokenClass : ValueEqualityBase<TokenClass>
    {
        public TokenClass(string classId)
        {
            Id = classId;
        }

        public string Id { get; }

        protected override bool ThisEquals(TokenClass other)
        {
            return Id.Equals(other.Id);
        }

        protected override object ThisHashCode => Id.GetHashCode();

        public override string ToString()
        {
            return Id;
        }
    }
}