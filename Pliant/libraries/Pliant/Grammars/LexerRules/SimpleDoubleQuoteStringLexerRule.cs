using Pliant.Automata;
using Pliant.Terminals;
using Pliant.Tokens;

namespace Pliant.LexerRules
{
    public class SimpleDoubleQuoteStringLexerRule : DfaLexerRule
    {
        // ["][^"]*["]
        public static readonly TokenName Class = new TokenName(@"[""][^""]*[""]");
        private static readonly DfaState enter;

        static SimpleDoubleQuoteStringLexerRule()
        {
            var start = DfaState.Inner();
            var inner = DfaState.Inner();
            var final = DfaState.Final();

            var quote = new CharacterTerminal('"');
            var notQuote = new NegationTerminal(quote);

            start.AddTransition(quote, inner);
            inner.AddTransition(notQuote, inner);
            inner.AddTransition(quote, final);

            enter = start;
        }

        public SimpleDoubleQuoteStringLexerRule()
            : base(enter, Class)
        {
        }
    }
}
