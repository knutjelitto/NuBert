using System;
using System.Collections.Generic;
using System.Text;
using Pliant.Inputs;

namespace Pliant.Tokens
{
    public abstract class TokenBase : IToken
    {
        protected TokenBase(TokenType tokenType, Cursor start)
        {
            TokenType = tokenType;
            Start = End = start;
        }

        protected Cursor Start { get; }
        protected Cursor End { get; set; }

        public int Position => Start.Position;
        public string Value => Start.Upto(End);
        public TokenType TokenType { get; }
        public IReadOnlyList<ITrivia> LeadingTrivia { get; set; }
        public IReadOnlyList<ITrivia> TrailingTrivia { get; set; }
    }
}
