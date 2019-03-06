using System.Collections.Generic;
using Pliant.Inputs;

namespace Pliant.Tokens
{
    public abstract class TokenBase : IToken
    {
        protected TokenBase(TokenClass tokenClass, Input start)
        {
            TokenClass = tokenClass;
            Start = End = start;
        }

        protected Input Start { get; }
        protected Input End { get; set; }

        public int Position => Start.Position;
        public string Value => Start.Upto(End);
        public TokenClass TokenClass { get; }
        public IReadOnlyList<ITrivia> LeadingTrivia { get; set; }
        public IReadOnlyList<ITrivia> TrailingTrivia { get; set; }
    }
}
