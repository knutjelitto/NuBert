using Pliant.Grammars;

namespace Pliant.Tokens
{
    public abstract class LexemeBase<TLexerRule> : AbstractLexeme
        where TLexerRule : LexerRule
    {
        protected LexemeBase(TLexerRule lexerRule, int position)
            : base(lexerRule, position)
        {
            ConcreteLexerRule = lexerRule;
        }

        public void Reset(TLexerRule lexerRule, int position)
        {
            ResetInternal(lexerRule, position);
            Reset();
        }

        protected TLexerRule ConcreteLexerRule { get; private set; }

        private void ResetInternal(TLexerRule lexerRule, int position)
        {
            this._leadingTrivia = null;
            this._trailingTrivia = null;

            LexerRule = lexerRule;
            ConcreteLexerRule = lexerRule;
            Position = position;
        }
    }
}