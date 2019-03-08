using Pliant.Utilities;

namespace Pliant.Tokens
{
    public sealed class TokenName : ValueEqualityBase<TokenName>
    {
        public TokenName(string classId)
        {
            Id = classId;
        }

        public string Id { get; }

        protected override bool ThisEquals(TokenName other)
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