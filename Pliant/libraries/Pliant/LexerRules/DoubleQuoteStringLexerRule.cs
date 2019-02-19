using Pliant.Automata;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.LexerRules
{
    public class DoubleQuoteStringLexerRule : DfaLexerRule
    {
        // ["][^"]*["]
        public static readonly TokenType TokenTypeDescriptor = new TokenType(@"[""][^""]*[""]");
        private static readonly DfaState Start;

        static DoubleQuoteStringLexerRule()
        {
            var states = new DfaState[3]
            {
                DfaState.Inner(),
                DfaState.Inner(),
                DfaState.Final()
            };

            var quote = new CharacterTerminal('"');
            var notQuote = new NegationTerminal(quote);

            var quoteToNotQuote = new DfaTransition(quote, states[1]);
            var notQuoteToNotQuote = new DfaTransition(notQuote, states[1]);
            var notQuoteToQuote = new DfaTransition(quote, states[2]);

            states[0].AddTransition(quoteToNotQuote);
            states[1].AddTransition(notQuoteToNotQuote);
            states[1].AddTransition(notQuoteToQuote);

            Start = states[0];
        }

        public DoubleQuoteStringLexerRule()
            : base(Start, TokenTypeDescriptor)
        {
        }
    }
}
