using Pliant.Automata;
using Pliant.Terminals;
using Pliant.Tokens;

namespace Pliant.LexerRules
{
    public sealed class StringLiteralLexerRule : DfaLexerRule
    {
        public StringLiteralLexerRule(string literal, TokenName tokenName)
            : base(MakeAutomaton(literal), tokenName)
        {
        }

        public StringLiteralLexerRule(string literal)
            : base(MakeAutomaton(literal), literal)
        {
        }

        public string Literal => TokenName.Id;

        public override bool Equals(object obj)
        {
            return obj is StringLiteralLexerRule other &&
                   Literal.Equals(other.Literal);
        }

        public override int GetHashCode()
        {
            return (TokenName, Literal).GetHashCode();
        }

        public override string ToString()
        {
            return Literal;
        }

        private static DfaState MakeAutomaton(string literal)
        {
            return MakeAutomaton(literal, 0);
        }

        private static DfaState MakeAutomaton(string literal, int index)
        {
            var state = DfaState.Inner();
            if (index < literal.Length - 1)
            {
                state.AddTransition(new CharacterTerminal(literal[index]), MakeAutomaton(literal, index + 1));
                return state;
            }

            var end = DfaState.Final();
            state.AddTransition(new CharacterTerminal(literal[index]), end);
            return state;
        }
    }
}