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
            Start = state;
            this._hashCode = ComputeHashCode(DfaLexerRuleType, tokenType);
        }

        public DfaState Start { get; }

        public override bool CanApply(char c)
        {
            for (var i = 0; i < Start.Transitions.Count; i++)
            {
                var transition = Start.Transitions[i];
                if (transition.Terminal.IsMatch(c))
                {
                    return true;
                }
            }

            return false;
        }

        public override ILexeme CreateLexeme(int position)
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