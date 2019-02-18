using System.Collections.Generic;
using Pliant.Diagnostics;
using Pliant.Utilities;

namespace Pliant.Tokens
{
    public class Token : IToken
    {
        public Token(string value, int position, TokenType tokenType)
        {
            Value = value;
            Position = position;
            TokenType = tokenType;
            this._hashCode = ComputeHashCode();
        }

        public Token(string value,
                     int position,
                     TokenType tokenType,
                     IReadOnlyList<ITrivia> leadingTrivia,
                     IReadOnlyList<ITrivia> trailingTrivia)
            : this(value, position, tokenType)
        {
            Assert.IsNotNull(leadingTrivia, nameof(leadingTrivia));
            Assert.IsNotNull(trailingTrivia, nameof(trailingTrivia));

            LeadingTrivia = new List<ITrivia>(leadingTrivia);
            TrailingTrivia = new List<ITrivia>(trailingTrivia);
        }

        public IReadOnlyList<ITrivia> LeadingTrivia { get; }

        public int Position { get; }

        public TokenType TokenType { get; }

        public IReadOnlyList<ITrivia> TrailingTrivia { get; }

        public string Value { get; }

        public override bool Equals(object obj)
        {
            return obj is Token token && 
                   Value.Equals(token.Value) && 
                   Position.Equals(token.Position) && 
                   TokenType.Equals(token.TokenType);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                TokenType.GetHashCode(),
                Position.GetHashCode(),
                Value.GetHashCode());
        }

        private static readonly ITrivia[] EmptyTriviaArray = { };

        private readonly int _hashCode;
    }
}