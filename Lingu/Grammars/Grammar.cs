using System.Collections.Generic;
using System.Linq;

namespace Lingu.Grammars
{
    public class Grammar
    {
        public Grammar(Nonterminal start, List<Production> productions)
        {
            Start = start;
            Productions = productions;
            Nonterminals = new HashSet<Nonterminal>(AllUsedSymbols(productions).OfType<Nonterminal>().Concat(Enumerable.Repeat(start, 1))).ToList();
            Terminals = new HashSet<Terminal>(AllUsedSymbols(productions).OfType<Terminal>()).ToList();
        }

        public Nonterminal Start { get; }
        public List<Rule> Rules { get; }
        public Terminal Whitespace { get; } = null;
        public List<Nonterminal> Nonterminals { get; }
        public List<Terminal> Terminals { get; }
        public List<Production> Productions { get; }

        private static IEnumerable<Symbol> AllUsedSymbols(IEnumerable<Production> productions)
        {
            foreach (var production in productions)
            {
                foreach (var symbol in production.Body)
                {
                    yield return symbol;
                }
            }
        }
    }
}
