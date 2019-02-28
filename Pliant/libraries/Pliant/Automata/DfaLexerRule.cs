using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Automata
{
    public class DfaLexerRule : LexerRule
    {
        public DfaLexerRule(DfaState state, string tokenType)
            : this(state, new TokenType(tokenType))
        {
        }

        public DfaLexerRule(DfaState state, TokenType tokenType)
            : base(tokenType)
        {
            StartState = state;
        }

        public DfaState StartState { get; }

        public override bool CanApply(char c)
        {
            foreach (var transition in StartState.Transitions)
            {
                if (transition.Terminal.IsMatch(c))
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
            return obj is DfaLexerRule other &&  TokenType.Equals(other.TokenType);
        }

        public override int GetHashCode()
        {
            return TokenType.GetHashCode();
        }

        public override string ToString()
        {
            return TokenType.ToString();
        }
    }
}