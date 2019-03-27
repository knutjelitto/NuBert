using System.Collections;
using System.Collections.Generic;
using Lingu.Grammars;

namespace Lingu.Charts
{
    public class DottedRule : IReadOnlyList<Symbol>
    {
        public DottedRule(Production production, int dot)
        {
            Production = production;
            Dot = dot;
        }

        public int Dot { get; }
        public Production Production { get; }

        public bool IsComplete => Dot == Count;

        public Symbol PostDot => Dot < Count ? this[Dot] : null;
        public Symbol PreDot => Dot > 0 ? this[Dot - 1] : null;

        public override bool Equals(object obj)
        {
            return obj is DottedRule other && Dot.Equals(other.Dot) && Production.Equals(other.Production);
        }

        public override int GetHashCode()
        {
            return (Production, Dot).GetHashCode();
        }

        public IEnumerator<Symbol> GetEnumerator()
        {
            return Production.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => Production.Count;

        public Symbol this[int index] => Production[index];
    }
}