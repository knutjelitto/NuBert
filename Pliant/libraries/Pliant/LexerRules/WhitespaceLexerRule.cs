﻿using Pliant.Automata;
using Pliant.Terminals;
using Pliant.Tokens;

namespace Pliant.LexerRules
{
    public class WhitespaceLexerRule : DfaLexer
    {
        private static readonly DfaState _start;
        private static readonly TokenType _staticTokenType = new TokenType(@"[\s]+");

        static WhitespaceLexerRule()
        {
            var start = DfaState.Inner();
            var end = DfaState.Final();

            start.AddTransition(WhitespaceTerminal.Instance, end);
            end.AddTransition(WhitespaceTerminal.Instance, end);

            _start = start;
        }

        public WhitespaceLexerRule()
            : base(_start, _staticTokenType)
        {
        }
    }
}
