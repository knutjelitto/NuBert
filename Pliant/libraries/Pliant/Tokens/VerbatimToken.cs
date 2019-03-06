using System.Collections.Generic;

namespace Pliant.Tokens
{
    public sealed class VerbatimToken : Trivia, IToken
    {
        public VerbatimToken(int position, string value, TokenType tokenType)
            : base(position, value, tokenType)
        {
            LeadingTrivia = null;
            TrailingTrivia = null;
            this.hashCode = (position, value, tokenType).GetHashCode();
        }

        public IReadOnlyList<ITrivia> LeadingTrivia { get; }

        public IReadOnlyList<ITrivia> TrailingTrivia { get; }

        public override bool Equals(object obj)
        {
            return obj is VerbatimToken other &&
                   Position.Equals(other.Position) &&
                   Value.Equals(other.Value) && 
                   TokenType.Equals(other.TokenType);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        private readonly int hashCode;
    }
}