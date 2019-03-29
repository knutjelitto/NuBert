using System;
using System.Collections.Generic;
using System.Linq;
using Lingu.Grammars;

namespace Lingu.Grammars.Build
{
    public class GrammarBuilder
    {
        public Dictionary<RuleExpr, Nonterminal> Rules { get; } = new Dictionary<RuleExpr, Nonterminal>();
        public Dictionary<TerminalExpr, Terminal> Terminals { get; } = new Dictionary<TerminalExpr, Terminal>();

        public Grammar From(RuleExpr startSymbol)
        {
            return new Grammar(Map(startSymbol));
        }

        private Terminal Map(TerminalExpr terminal)
        {
            if (!Terminals.TryGetValue(terminal, out var mapped))
            {
                mapped = new Terminal(terminal.Provision);
                Terminals.Add(terminal, mapped);
            }

            return mapped;
        }

        private Nonterminal Map(RuleExpr rule)
        {
            if (!Rules.TryGetValue(rule, out var mapped))
            {
                mapped = new Nonterminal(rule.Name);
                Rules.Add(rule, mapped);
                mapped.Body = Map(rule.Body);
            }

            return mapped;
        }

        private Symbol Map(SymbolExpr symbol)
        {
            switch (symbol)
            {
                case TerminalExpr terminal:
                    return Map(terminal);
                case RuleExpr nonterminal:
                    return Map(nonterminal);
                default:
                    throw new NotImplementedException();
            }
        }

        private Body Map(BodyExpr body)
        {
            return new Body(body.Select(Map));
        }

        private Chain Map(ChainExpr chain)
        {
            return new Chain(chain.Select(Map));
        }
    }
}