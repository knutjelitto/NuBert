using Pliant.Automata;
using Pliant.Terminals;
using Pliant.Tokens;

namespace Pliant.LexerRules
{
    public class SimpleSingleQuoteStringLexerRule : DfaLexerRule
    {
        // ['][^']*[']
        private const string _pattern = @"['][^']*[']";
        public static readonly TokenName Class = new TokenName(_pattern);
        private static readonly DfaState _start;

        static SimpleSingleQuoteStringLexerRule()
        {
            var start = DfaState.Inner();
            var innner = DfaState.Inner();
            var final = DfaState.Final();

            var quote = new CharacterTerminal('\'');
            var notQuote = new NegationTerminal(quote);

            start.AddTransition(quote, innner);
            innner.AddTransition(notQuote, innner);
            innner.AddTransition(quote, final);

            _start = start;
        }

        public SimpleSingleQuoteStringLexerRule()
            : base(_start, Class)
        {
        }
    }
}
