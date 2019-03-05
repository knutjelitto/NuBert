using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Automata
{
    public class DfaLexer : LexerRule
    {
        public DfaLexer(DfaState startState, string tokenType)
            : this(startState, new TokenType(tokenType))
        {
        }

        public DfaLexer(DfaState startState, TokenType tokenType)
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
            return obj is DfaLexer other && TokenType.Equals(other.TokenType);
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