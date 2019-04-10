using System;
using System.Collections.Generic;
using System.Linq;

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

        private Terminal Map(TerminalExpr terminalExpr)
        {
            if (!Terminals.TryGetValue(terminalExpr, out var mapped))
            {
                mapped = terminalExpr.Provision.Terminal;
                Terminals.Add(terminalExpr, mapped);
            }

            return mapped;
        }

        private Nonterminal Map(RuleExpr rule)
        {
            if (!Rules.TryGetValue(rule, out var mapped))
            {
                mapped = new Nonterminal(rule.Name);
                Rules.Add(rule, mapped);
                mapped.Body = Map(mapped, rule.Body);
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

        private List<Production> Map(Nonterminal rule, BodyExpr body)
        {
            return body.Select(chain => Map(rule, chain)).ToList();
        }

        private Production Map(Nonterminal rule, ChainExpr chain)
        {
            return new Production(rule, chain.Select(Map).ToList());
        }
    }
}