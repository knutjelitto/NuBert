﻿using Pliant.Automata;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.LexerRules
{
    public class SingleQuoteStringLexerRule : DfaLexerRule
    {
        // ['][^']*[']
        private const string _pattern = @"['][^']*[']";
        public static readonly TokenType TokenTypeDescriptor = new TokenType(_pattern);
        private static readonly DfaState _start;

        static SingleQuoteStringLexerRule()
        {
            var states = new DfaState[3]
            {
                DfaState.Inner(),
                DfaState.Inner(),
                DfaState.Final(),
            };

            var quote = new CharacterTerminal('\'');
            var notQuote = new NegationTerminal(quote);

            var quoteToNotQuote = new DfaTransition(quote, states[1]);
            var notQuoteToNotQuote = new DfaTransition(notQuote, states[1]);
            var notQuoteToQuote = new DfaTransition(quote, states[2]);

            states[0].AddTransition(quoteToNotQuote);
            states[1].AddTransition(notQuoteToNotQuote);
            states[1].AddTransition(notQuoteToQuote);

            _start = states[0];
        }

        public SingleQuoteStringLexerRule()
            : base(_start, TokenTypeDescriptor)
        {
        }
    }
}
