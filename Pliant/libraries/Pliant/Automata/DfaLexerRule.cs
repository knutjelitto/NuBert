using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Automata
{
    public class DfaLexerRule : LexerRule
    {
        public DfaLexerRule(DfaState startState, string tokenType)
            : this(startState, new TokenClass(tokenType))
        {
        }

        public DfaLexerRule(DfaState startState, TokenClass tokenType)
            : base(tokenType)
        {
            StartState = startState;
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
            return obj is DfaLexerRule other && TokenType.Equals(other.TokenType);
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