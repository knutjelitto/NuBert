using System.Collections.Generic;
using System.Linq;

namespace Lingu.Grammars
{
    public class Rule : Nonterminal
    {
        public Rule(string name, IEnumerable<Production> productions)
            : base(name)
        {
            Productions = productions.ToList();
        }

        public List<Production> Productions { get; }
    }
}