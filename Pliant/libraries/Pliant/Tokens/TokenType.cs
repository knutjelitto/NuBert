namespace Pliant.Tokens
{
    public sealed class TokenType
    {
        public TokenType(string id)
        {
            Id = id;
            this._hashCode = ComputeHashCode(Id);
        }

        public string Id { get; }

        public override bool Equals(object obj)
        {
            return obj is TokenType other && other.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return Id;
        }

        private static int ComputeHashCode(string id)
        {
            return id.GetHashCode();
        }

        private readonly int _hashCode;
    }
}