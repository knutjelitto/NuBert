using System.Collections.Generic;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Tokens
{
    public abstract class Lexeme : IToken
    {
        protected Lexeme(Lexer lexer, int position)
        {
            TokenType = lexer.TokenType;
            Position = position;
        }

        public IReadOnlyList<ITrivia> LeadingTrivia
        {
            get
            {
                if (this.leadingTrivia == null)
                {
                    return emptyTriviaArray;
                }

                return this.leadingTrivia;
            }
        }

        public int Position { get; }

        public TokenType TokenType { get; }

        public IReadOnlyList<ITrivia> TrailingTrivia
        {
            get
            {
                if (this.trailingTrivia == null)
                {
                    return emptyTriviaArray;
                }

                return this.trailingTrivia;
            }
        }

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
        public abstract bool Scan(char c);

        private static readonly ITrivia[] emptyTriviaArray = { };
        private List<ITrivia> leadingTrivia;
        private List<ITrivia> trailingTrivia;
    }
}