using Pliant.Automata;
using Pliant.Terminals;
using Pliant.Tokens;

namespace Pliant.LexerRules
{
    public sealed class StringLiteralLexer : DfaLexerRule
    {
        public StringLiteralLexer(string literal, TokenClass tokenClass)
            : base(MakeAutomaton(literal), tokenClass)
        {
        }

        public StringLiteralLexer(string literal)
            : base(MakeAutomaton(literal), literal)
        {
        }

        public string Literal => TokenClass.Id;

        public override bool Equals(object obj)
        {
            return obj is StringLiteralLexer other &&
                   Literal.Equals(other.Literal);
        }

        public override int GetHashCode()
        {
            return (TokenClass, Literal).GetHashCode();
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