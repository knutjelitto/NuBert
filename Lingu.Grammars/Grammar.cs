using System.Collections.Generic;
using System.Linq;

namespace Lingu.Grammars
{
    public class Grammar
    {
        public Grammar(Nonterminal start)
        {
            Start = start;
            Symbols = AllSymbols(start).ToList();

            Rules = new HashSet<Nonterminal>(Symbols.OfType<Nonterminal>()).ToList();
            Terminals = new HashSet<Terminal>(Symbols.OfType<Terminal>()).ToList();
            Productions = MakeProductions(Rules).ToList();
        }

        public Nonterminal Start { get; }

        public Terminal Whitespace { get; } = null;
        public List<Nonterminal> Rules { get; }

        public List<Symbol> Symbols { get; }
        public List<Terminal> Terminals { get; }
        public List<Production> Productions { get; }

        private IEnumerable<Production> MakeProductions(IEnumerable<Nonterminal> rules)
        {
            foreach (var rule in rules)
            {
                foreach (var chain in rule.Body)
                {
                    yield return new Production(rule, chain);
                }
            }
        }

        public IEnumerable<Production> ProductionsFor(Nonterminal nonterminal)
        {
            return Productions.Where(production => production.Head.Equals(nonterminal));
        }

        public IEnumerable<Production> ProductionsForStart()
        {
            return ProductionsFor(Start);
        }

        private static IEnumerable<Symbol> AllSymbols(Nonterminal start)
        {
            var already = new HashSet<Symbol>();

            void all(Nonterminal current)
            {
                already.Add(current);

                foreach (var chain in current.Body)
                {
                    foreach (var symbol in chain)
                    {
                        if (!already.Contains(symbol))
                        {
                            if (symbol is Nonterminal rule)
                            {
                                all(rule);
                            }
                            else
                            {
                                already.Add(symbol);
                            }
                        }
                    }
                }
            }

            all(start);

            return already;
        }

        public bool IsTransitiveNullable(Nonterminal nonTerminal)
        {
            return false;
            //throw new System.NotImplementedException();
        }
    }
}
