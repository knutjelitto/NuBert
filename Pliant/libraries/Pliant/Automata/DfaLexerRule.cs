using Pliant.Grammars;
using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Automata
{
    public class DfaLexerRule : LexerRule
    {
        public DfaLexerRule(DfaState state, string tokenType)
            : this(state, new TokenType(tokenType))
        {
        }

        public DfaLexerRule(DfaState state, TokenType tokenType)
            : base(DfaLexerRuleType, tokenType)
        {
            StartState = state;
            this._hashCode = ComputeHashCode(DfaLexerRuleType, tokenType);
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
            return obj is DfaLexerRule dfaLexerRule && 
                   TokenType.Equals(dfaLexerRule.TokenType);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return TokenType.ToString();
        }

        public static readonly LexerRuleType DfaLexerRuleType = new LexerRuleType("Dfa");

        private static int ComputeHashCode(LexerRuleType dfaLexerRuleType, TokenType tokenType)
        {
            return HashCode.Compute(
                dfaLexerRuleType.GetHashCode(),
                tokenType.GetHashCode());
        }

        private readonly int _hashCode;
    }
}