using System.Collections.Generic;
using Lingu.Grammars;

namespace Lingu.Builders
{
    public class GrammarBuilder
    {
        public Dictionary<NonterminalExpr, Nonterminal> Nonterminals { get; } = new Dictionary<NonterminalExpr, Nonterminal>();
        public List<Production> Productions { get; } = new List<Production>();
        public Dictionary<TerminalExpr, Terminal> Terminals { get; } = new Dictionary<TerminalExpr, Terminal>();

        public Grammar From(NonterminalExpr startSymbol)
        {
            return new Grammar(Map(startSymbol), Productions);
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

        private Nonterminal Map(NonterminalExpr rule)
        {
            if (!Nonterminals.TryGetValue(rule, out var mapped))
            {
                mapped = new Nonterminal(rule.Name);
                Nonterminals.Add(rule, mapped);

                foreach (var chain in rule.Body)
                {
                    var body = new List<Symbol>();

                    foreach (var symbol in chain)
                    {
                        switch (symbol)
                        {
                            case TerminalExpr terminal:
                                body.Add(Map(terminal));
                                break;
                            case NonterminalExpr nonterminal:
                                body.Add(Map(nonterminal));
                                break;
                        }
                    }

                    Productions.Add(new Production(mapped, body));
                }
            }

            return mapped;
        }
    }
}