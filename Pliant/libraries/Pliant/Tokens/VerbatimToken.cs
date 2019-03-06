using System.Collections.Generic;
using Pliant.Utilities;

namespace Pliant.Tokens
{
    public sealed class VerbatimToken : ValueEqualityBase<VerbatimToken>, IToken
    {
        public VerbatimToken(int position, string value, TokenClass tokenClass)
        {
            Position = position;
            Value = value;
            TokenClass = tokenClass;
        }

        public IReadOnlyList<ITrivia> LeadingTrivia => noTrivia;
        public int Position { get; }
        public TokenClass TokenClass { get; }
        public IReadOnlyList<ITrivia> TrailingTrivia => noTrivia;
        public string Value { get; }

        protected override object ThisHashCode => (Position, Value, TokenClass);

        protected override bool ThisEquals(VerbatimToken other)
        {
            return Position.Equals(other.Position) &&
                   Value.Equals(other.Value) &&
                   TokenClass.Equals(other.TokenClass);
        }

        private static readonly ITrivia[] noTrivia = { };
    }
}