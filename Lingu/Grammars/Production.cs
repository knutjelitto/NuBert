using System.Collections.Generic;
using System.Linq;

namespace Lingu.Grammars
{
    public class Production
    {
        public Production(Nonterminal head, IEnumerable<Symbol> body)
        {
            Head = head;
            Body = body.ToList();
        }

        public List<Symbol> Body { get; }

        public Nonterminal Head { get; }

        public override string ToString()
        {
            return $"{Head} -> {string.Join(" ", Body)}";
        }
    }
}