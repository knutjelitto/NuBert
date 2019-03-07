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

        public DfaLexerRule(DfaState startState, TokenClass tokenClass)
            : base(tokenClass)
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
            return obj is DfaLexerRule other && TokenClass.Equals(other.TokenClass);
        }

        public override int GetHashCode()
        {
            return TokenClass.GetHashCode();
        }

        public override string ToString()
        {
            return TokenClass.ToString();
        }
    }
}