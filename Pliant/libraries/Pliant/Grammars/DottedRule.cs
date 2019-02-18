using System;
using System.Text;
using Pliant.Diagnostics;

namespace Pliant.Grammars
{
    public class DottedRule //: IComparable<DottedRule> //, IDottedRule
    {
        public DottedRule(Production production, int dot)
        {
            Assert.IsNotNull(production, nameof(production));
            Assert.IsGreaterThanEqualToZero(dot, nameof(dot));

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

#if false
        public int CompareTo(DottedRule other)
        {
            return GetHashCode().CompareTo(other.GetHashCode());
        }
#endif

        public override bool Equals(object obj)
        {
            return obj is DottedRule other && 
                   other.Production.Equals(Production) && 
                   other.Dot.Equals(Dot);
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
                stringBuilder.Append(Dot);
            }

            return stringBuilder.ToString();
        }

        private static Symbol GetPostDotSymbol(Production production, int dot)
        {
            if (dot >= production.RightHandSide.Count)
            {
                return null;
            }

            return production.RightHandSide[dot];
        }

        private static Symbol GetPreDotSymbol(Production production, int dot)
        {
            if (dot == 0 || production.IsEmpty)
            {
                return null;
            }

            return production.RightHandSide[dot - 1];
        }

        private static bool IsCompleted(Production production, int dot)
        {
            return dot == production.RightHandSide.Count;
        }
    }
}