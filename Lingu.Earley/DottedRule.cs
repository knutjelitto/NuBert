using System.Collections;
using System.Collections.Generic;
using System.Text;
using Lingu.Grammars;

namespace Lingu.Earley
{
    public class DottedRule : IReadOnlyList<Symbol>
    {
        private readonly DottedRuleFactory factory;

        public DottedRule(DottedRuleFactory factory, Production production, int dot)
        {
            this.factory = factory;
            Production = production;
            Dot = dot;
        }

        public DottedRule Next => this.factory.Get(Production, Dot + 1);

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

        public override string ToString()
        {
            const string dot = "\u25CF";

            var builder = new StringBuilder();

            builder.Append($"{Production.Head} ->");

            for (var p = 0; p < Production.Count; p++)
            {
                builder.AppendFormat(
                    "{0}{1}",
                    p == Dot ? dot : " ",
                    Production[p]);
            }

            if (Dot == Production.Count)
            {
                builder.Append(dot);
            }

            return builder.ToString();
        }
    }
}