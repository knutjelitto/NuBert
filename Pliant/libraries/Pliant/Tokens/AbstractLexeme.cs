using System;
using System.Collections.Generic;
using System.Text;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Tokens
{
    public abstract class AbstractLexeme : ILexeme
    {
        protected AbstractLexeme(LexerRule lexerRule, int position)
        {
            LexerRule = lexerRule;
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
                this._leadingTrivia = new List<ITrivia>();
            }

            this._leadingTrivia.Add(trivia);
        }

        public void AddTrailingTrivia(ITrivia trivia)
        {
            if (this._trailingTrivia == null)
            {
                this._trailingTrivia = new List<ITrivia>();
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
