using Pliant.Automata;
using Pliant.Terminals;
using Pliant.Tokens;

namespace Pliant.LexerRules
{
    public class SimpleDoubleQuoteStringLexerRule : DfaLexerRule
    {
        // ["][^"]*["]
        public static readonly TokenType TokenTypeDescriptor = new TokenType(@"[""][^""]*[""]");
        private static readonly DfaState Start;

        static SimpleDoubleQuoteStringLexerRule()
        {
            var start = DfaState.Inner();
            var innner = DfaState.Inner();
            var final = DfaState.Final();

            var quote = new CharacterTerminal('"');
            var notQuote = new NegationTerminal(quote);

            start.AddTransition(quote, innner);
            innner.AddTransition(notQuote, innner);
            innner.AddTransition(quote, final);

            Start = start;
        }

        public SimpleDoubleQuoteStringLexerRule()
            : base(Start, TokenTypeDescriptor)
        {
        }
    }
}
