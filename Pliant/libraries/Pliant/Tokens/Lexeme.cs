using System.Collections.Generic;
using Pliant.Grammars;
using Pliant.Inputs;
using Pliant.Utilities;

namespace Pliant.Tokens
{
    public abstract class Lexeme : IToken
    {
        protected Lexeme(LexerRule lexer, int position)
        {
            TokenName = lexer.TokenName;
            Position = position;
        }

        public IReadOnlyList<ITrivia> LeadingTrivia => this.leadingTrivia ?? (IReadOnlyList<ITrivia>) emptyTriviaArray;

        public int Position { get; }

        public TokenName TokenName { get; }

        public IReadOnlyList<ITrivia> TrailingTrivia => this.trailingTrivia ?? (IReadOnlyList<ITrivia>) emptyTriviaArray;

        public abstract string Value { get; }

        public void AddLeadingTrivia(ITrivia trivia)
        {
            if (this.leadingTrivia == null)
            {
                var pool = SharedPools.Default<List<ITrivia>>();
                this.leadingTrivia = ObjectPoolExtensions.Allocate(pool);
            }

            this.leadingTrivia.Add(trivia);
        }

        public void AddTrailingTrivia(ITrivia trivia)
        {
            if (this.trailingTrivia == null)
            {
                var pool = SharedPools.Default<List<ITrivia>>();
                this.trailingTrivia = ObjectPoolExtensions.Allocate(pool);
            }

            this.trailingTrivia.Add(trivia);
        }

        public abstract bool IsAccepted();
        public abstract bool Scan(char character);

        private static readonly ITrivia[] emptyTriviaArray = { };
        private List<ITrivia> leadingTrivia;
        private List<ITrivia> trailingTrivia;
    }
}