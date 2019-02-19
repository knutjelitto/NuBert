using System;
using System.Text;
using Pliant.Diagnostics;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class DottedRule : IComparable<DottedRule>
    {
        public DottedRule(Production production, int position)
        {
            Assert.IsNotNull(production, nameof(production));
            Assert.IsGreaterThanEqualToZero(position, nameof(position));

            Production = production;
            Position = position;
            PostDotSymbol = GetPostDotSymbol(production, position);
            PreDotSymbol = GetPreDotSymbol(production, position);
            IsComplete = IsCompleted(position, production);

            this._hashCode = ComputeHashCode(Production, Position);
        }

        public bool IsComplete { get; }

        public int Position { get; }

        public Symbol PostDotSymbol { get; }

        public Symbol PreDotSymbol { get; }

        public Production Production { get; }

        public int CompareTo(DottedRule other)
        {
            return GetHashCode().CompareTo(other.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            return obj is DottedRule other && other.Production.Equals(Production) && other.Position == Position;
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder()
                .AppendFormat("{0} ->", Production.LeftHandSide.Value);
            const string Dot = "\u25CF";

            for (var p = 0; p < Production.RightHandSide.Count; p++)
            {
                stringBuilder.AppendFormat(
                    "{0}{1}",
                    p == Position ? Dot : " ",
                    Production.RightHandSide[p]);
            }

            if (Position == Production.RightHandSide.Count)
            {
                stringBuilder.Append(Dot);
            }

            return stringBuilder.ToString();
        }

        private static int ComputeHashCode(Production production, int position)
        {
            return HashCode.Compute(production.GetHashCode(), position.GetHashCode());
        }

        private static Symbol GetPostDotSymbol(Production production, int position)
        {
            var productionRighHandSide = production.RightHandSide;
            if (position >= productionRighHandSide.Count)
            {
                return null;
            }

            return productionRighHandSide[position];
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

        private readonly int _hashCode;
    }
}