using System.Text;
using Pliant.Diagnostics;

namespace Pliant.Grammars
{
    public sealed class DottedRule
    {
        public DottedRule(Production production, int dot)
        {
            Assert.IsNotNull(production, nameof(production));
            Assert.IsGreaterThanEqualToZero(dot, nameof(dot));

            Production = production;
            Dot = dot;
            PostDotSymbol = GetPostDotSymbol(production, dot);
            PreDotSymbol = GetPreDotSymbol(production, dot);
            IsComplete = IsCompleted(dot, production);
        }

        public bool IsComplete { get; }

        public int Dot { get; }

        public Symbol PostDotSymbol { get; }

        public Symbol PreDotSymbol { get; }

        public Production Production { get; }

        public override bool Equals(object obj)
        {
            //return obj is DottedRule other && Id.Equals(other.Id);
            return obj is DottedRule other &&
                   Production.Equals(other.Production) &&
                   Dot.Equals(other.Dot);
        }

        public override int GetHashCode()
        {
            return (Production, Dot).GetHashCode();
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder()
                .AppendFormat("{0} ->", Production.LeftHandSide.Value);
            const string dot = "\u25CF";

            for (var p = 0; p < Production.RightHandSide.Count; p++)
            {
                stringBuilder.AppendFormat(
                    "{0}{1}",
                    p == Dot ? dot : " ",
                    Production.RightHandSide[p]);
            }

            if (Dot == Production.RightHandSide.Count)
            {
                stringBuilder.Append(dot);
            }

            return stringBuilder.ToString();
        }

        private static Symbol GetPostDotSymbol(Production production, int position)
        {
            var productionRightHandSide = production.RightHandSide;
            if (position >= productionRightHandSide.Count)
            {
                return null;
            }

            return productionRightHandSide[position];
        }

        private static Symbol GetPreDotSymbol(Production production, int position)
        {
            if (position == 0 || production.IsEmpty)
            {
                return null;
            }

            return production.RightHandSide[position - 1];
        }

        private static bool IsCompleted(int position, Production production)
        {
            return position == production.RightHandSide.Count;
        }
    }
}