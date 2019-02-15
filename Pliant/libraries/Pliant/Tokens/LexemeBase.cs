using System.Collections.Generic;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Tokens
{
    public abstract class LexemeBase<TLexerRule> : ILexeme
    {
        protected LexemeBase(TLexerRule lexerRule, int position)
        {
            LexerRule = lexerRule as ILexerRule;
            ConcreteLexerRule = lexerRule;
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

        public ILexerRule LexerRule { get; private set; }

        public int Position { get; private set; }

        public TokenType TokenType => LexerRule.TokenType;

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

        public void Reset(TLexerRule lexerRule, int position)
        {
            ResetInternal(lexerRule, position);
            Reset();
        }

        public abstract bool Scan(char c);

        protected TLexerRule ConcreteLexerRule { get; private set; }

        private void ResetInternal(TLexerRule lexerRule, int position)
        {
            var pool = SharedPools.Default<List<ITrivia>>();
            if (this._leadingTrivia != null)
            {
                pool.ClearAndFree(this._leadingTrivia);
            }

            if (this._trailingTrivia != null)
            {
                pool.ClearAndFree(this._trailingTrivia);
            }

            LexerRule = lexerRule as ILexerRule;
            ConcreteLexerRule = lexerRule;
            Position = position;
        }

        private static readonly ITrivia[] EmptyTriviaArray = { };
        private List<ITrivia> _leadingTrivia;
        private List<ITrivia> _trailingTrivia;
    }
}