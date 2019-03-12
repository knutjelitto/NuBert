using System.Diagnostics;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Automata
{
    public class DfaLexerRule : LexerRule
    {
        public DfaLexerRule(DfaState start, string tokenType)
            : this(start, new TokenName(tokenType))
        {
        }

        public DfaLexerRule(DfaState start, TokenName tokenName)
            : base(tokenName)
        {
            Start = start;
            Debug.Assert(!Start.IsFinal);
        }

        public DfaState Start { get; }

        public override bool CanApply(char c)
        {
            foreach (var transition in Start.Transitions)
            {
                if (transition.Terminal.CanApply(c))
                {
                    return true;
                }
            }

            return false;
        }

        public override Lexeme CreateLexeme(int position)
        {
            return new DfaLexeme(this, position);
        }

        public override bool Equals(object obj)
        {
            return obj is DfaLexerRule other && TokenName.Equals(other.TokenName);
        }

        public override int GetHashCode()
        {
            return TokenName.GetHashCode();
        }

        public override string ToString()
        {
            return TokenName.ToString();
        }
    }
}