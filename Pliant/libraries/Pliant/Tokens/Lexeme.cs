using System.Collections.Generic;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Tokens
{
    public abstract class Lexeme : ILexeme
    {
        protected Lexeme(LexerRule lexerRule, int position)
        {
            LexerRule = lexerRule;
            TokenType = lexerRule.TokenType;
            Position = position;
        }

        public IReadOnlyList<ITrivia> LeadingTrivia
        {
            get
            {
                if (this._leadingTrivia == null)
                {
                    return EmptyTriviaArray;
                }

                return this._leadingTrivia;
            }
        }

        public LexerRule LexerRule { get; protected set; }

        public int Position { get; protected set; }

        public TokenType TokenType { get; protected set; }

        public IReadOnlyList<ITrivia> TrailingTrivia
        {
            get
            {
                if (this._trailingTrivia == null)
                {
                    return EmptyTriviaArray;
                }

                return this._trailingTrivia;
            }
        }

        public abstract string Value { get; }

        public void AddLeadingTrivia(ITrivia trivia)
        {
            if (this._leadingTrivia == null)
            {
                var pool = SharedPools.Default<List<ITrivia>>();
                this._leadingTrivia = pool.AllocateAndClear();
            }

            this._leadingTrivia.Add(trivia);
        }

        public void AddTrailingTrivia(ITrivia trivia)
        {
            if (this._trailingTrivia == null)
            {
                var pool = SharedPools.Default<List<ITrivia>>();
                this._trailingTrivia = pool.AllocateAndClear();
            }

            this._trailingTrivia.Add(trivia);
        }

        public abstract bool IsAccepted();

        public abstract void Reset();

        public abstract bool Scan(char c);
        private static readonly ITrivia[] EmptyTriviaArray = { };
        protected List<ITrivia> _leadingTrivia;
        protected List<ITrivia> _trailingTrivia;
    }
}