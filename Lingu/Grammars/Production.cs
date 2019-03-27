using System.Collections;
using System.Collections.Generic;

namespace Lingu.Grammars
{
    public class Production : IReadOnlyList<Symbol>
    {
        private readonly List<Symbol> body;

        public Production(Nonterminal head, List<Symbol> body)
        {
            Head = head;
            this.body = body;
        }

        public int Count => this.body.Count;

        public Nonterminal Head { get; }

        public Symbol this[int index] => this.body[index];

        public IEnumerator<Symbol> GetEnumerator()
        {
            return this.body.GetEnumerator();
        }

        public override string ToString()
        {
            return $"{Head} -> {string.Join(" ", this.body)}";
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}