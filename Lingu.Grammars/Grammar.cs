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

            Nonterminals = new HashSet<Nonterminal>(Symbols.OfType<Nonterminal>()).ToList();
            Terminals = new HashSet<Terminal>(Symbols.OfType<Terminal>()).ToList();
            Productions = MakeProductions(Nonterminals).ToList();
            Whitespace = new List<Terminal>();

            MakeIsNullable();
        }

        public Nonterminal Start { get; }

        public List<Terminal> Whitespace { get; }
        public List<Nonterminal> Nonterminals { get; }
        public List<Terminal> Terminals { get; }

        public List<Symbol> Symbols { get; }
        public List<Production> Productions { get; }

        private IEnumerable<Production> MakeProductions(IEnumerable<Nonterminal> rules)
        {
            return rules.SelectMany(rule => rule.Body);
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

        private void MakeIsNullable()
        {
            var nullables = new HashSet<Symbol>();
            var loop = true;

            while (loop)
            {
                loop = false;
                foreach (var rule in Nonterminals.Where(rule => !nullables.Contains(rule)))
                {
                    foreach (var production in rule.Body)
                    {
                        var count = production.Count(symbol => !nullables.Contains(symbol));

                        if (count == 0)
                        {
                            loop = nullables.Add(production.Head);
                            break;
                        }
                    }
                }
            }

            foreach (var nullable in nullables)
            {
                nullable.IsNullable = true;
            }

        }
    }
}
