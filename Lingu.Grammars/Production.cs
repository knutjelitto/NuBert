using System.Collections;
using System.Collections.Generic;

namespace Lingu.Grammars
{
    public class Production : IReadOnlyList<Symbol>
    {
        public Production(Nonterminal head, List<Symbol> body)
        {
            Head = head;
            Body = body;
        }

        public int Count => Body.Count;

        public Nonterminal Head { get; }

        private List<Symbol> Body { get; }

        public Symbol this[int index] => Body[index];

        public IEnumerator<Symbol> GetEnumerator()
        {
            return Body.GetEnumerator();
        }

        public override string ToString()
        {
            return $"{Head} <- {string.Join(" ", Body)}";
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}