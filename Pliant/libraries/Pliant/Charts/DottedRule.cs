using System.Text;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public struct DottedRule
    {
        public DottedRule(Production production, int dot)
        {
            Production = production;
            Dot = dot;
            PostDotSymbol = GetPostDotSymbol(production, dot);
            PreDotSymbol = GetPreDotSymbol(production, dot);
            IsComplete = IsCompleted(production, dot);

            this.hashCode = (Dot, Production).GetHashCode();
        }

        public Production Production { get; }
        public int Dot { get; }

        public bool IsComplete { get; }

        public Symbol PostDotSymbol { get; }

        public Symbol PreDotSymbol { get; }

        private readonly int hashCode;

        public override bool Equals(object obj)
        {
            return obj is DottedRule other && Dot.Equals(other.Dot) && Production.Equals(other.Production);
        }

        public override int GetHashCode() => this.hashCode;

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