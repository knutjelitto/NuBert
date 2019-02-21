﻿using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.LexerRules;

namespace Pliant.Tests.Common.Grammars
{
    public class SimpleExpressionGrammar : GrammarWrapper
    {
        private static readonly IGrammar grammar;

        static SimpleExpressionGrammar()
        {
            var number = new NumberLexerRule();
            ProductionExpression E = "E";
            E.Rule =
                (E + "+" + E)
                | (E + "*" + E)
                | (E + "/" + E)
                | (E + "-" + E)
                | number;
            grammar = new GrammarExpression(E, new[] { E }).ToGrammar();
        }

        public SimpleExpressionGrammar()
            : base(grammar)
        { }
    }
}
