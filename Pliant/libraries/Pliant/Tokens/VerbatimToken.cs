using System.Collections.Generic;
using Pliant.Utilities;

namespace Pliant.Tokens
{
    public sealed class VerbatimToken : ValueEqualityBase<VerbatimToken>, IToken
    {
        public VerbatimToken(TokenName tokenName, int position, string value)
        {
            Position = position;
            Value = value;
            TokenName = tokenName;
        }

        public IReadOnlyList<ITrivia> LeadingTrivia => noTrivia;
        public int Position { get; }
        public TokenName TokenName { get; }
        public IReadOnlyList<ITrivia> TrailingTrivia => noTrivia;
        public string Value { get; }

        protected override object ThisHashCode => (Position, Value, TokenName);

        protected override bool ThisEquals(VerbatimToken other)
        {
            return Position.Equals(other.Position) &&
                   Value.Equals(other.Value) &&
                   TokenName.Equals(other.TokenName);
        }

        private static readonly ITrivia[] noTrivia = { };
    }
}