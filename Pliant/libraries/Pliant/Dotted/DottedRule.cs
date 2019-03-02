using System.Text;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Dotted
{
    public sealed class DottedRule : ValueEqualityBase<DottedRule>
    {
        public DottedRule(Production production, int dot)
            : base((production, dot).GetHashCode())
        {
            Production = production;
            Dot = dot;
            PostDotSymbol = GetPostDotSymbol(production, dot);
            PreDotSymbol = GetPreDotSymbol(production, dot);
            IsComplete = IsCompleted(production, dot);
        }

        public bool IsComplete { get; }

        public int Dot { get; }

        public Symbol PostDotSymbol { get; }

        public Symbol PreDotSymbol { get; }

        public Production Production { get; }

        public override bool ThisEquals(DottedRule other)
        {
            return Production.Equals(other.Production) && Dot.Equals(other.Dot);
        }

        public override string ToString()
        {
            const string dot = "\u25CF";

            var builder = new StringBuilder();

            builder.Append($"{Production.LeftHandSide} ->");

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

        private static Symbol GetPostDotSymbol(Production production, int dot)
        {
            if (dot >= production.Count)
            {
                return null;
            }

            return production[dot];
        }

        private static Symbol GetPreDotSymbol(Production production, int dot)
        {
            if (dot <= 0 || production.Count == 0)
            {
                return null;
            }

            return production[dot - 1];
        }

        private static bool IsCompleted(Production production, int dot)
        {
            return dot == production.Count;
        }
    }
}