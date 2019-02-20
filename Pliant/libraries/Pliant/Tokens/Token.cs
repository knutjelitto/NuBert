using System.Collections.Generic;
using Pliant.Utilities;

namespace Pliant.Tokens
{
    public class Token : Trivia, IToken
    {
        public Token(int position, string value, TokenType tokenType)
            : base(position, value, tokenType)
        {
            LeadingTrivia = null;
            TrailingTrivia = null;
            this._hashCode = ComputeHashCode();
        }

        public virtual IReadOnlyList<ITrivia> LeadingTrivia { get; }

        public virtual IReadOnlyList<ITrivia> TrailingTrivia { get; }

        public override bool Equals(object obj)
        {
            return obj is Token other &&
                   Position.Equals(other.Position) &&
                   Value.Equals(other.Value) && 
                   TokenType.Equals(other.TokenType);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                Position.GetHashCode(),
                Value.GetHashCode(),
                TokenType.GetHashCode());
        }

        private readonly int _hashCode;
    }
}