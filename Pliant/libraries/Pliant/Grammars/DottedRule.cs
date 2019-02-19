using System.Text;
using Pliant.Diagnostics;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public sealed class DottedRule
    {
        public int Id { get; }

        public DottedRule(int id, Production production, int position)
        {
            Id = id;
            Assert.IsNotNull(production, nameof(production));
            Assert.IsGreaterThanEqualToZero(position, nameof(position));

            Production = production;
            Position = position;
            PostDotSymbol = GetPostDotSymbol(production, position);
            PreDotSymbol = GetPreDotSymbol(production, position);
            IsComplete = IsCompleted(position, production);

            //this._hashCode = ComputeHashCode(Production, Position);
            this._hashCode = Id.GetHashCode();
        }

        public bool IsComplete { get; }

        public int Position { get; }

        public Symbol PostDotSymbol { get; }

        public Symbol PreDotSymbol { get; }

        public Production Production { get; }

        public override bool Equals(object obj)
        {
            return obj is DottedRule other && Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id;
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
                    p == Position ? dot : " ",
                    Production.RightHandSide[p]);
            }

            if (Position == Production.RightHandSide.Count)
            {
                stringBuilder.Append(dot);
            }

            return stringBuilder.ToString();
        }

        private static int ComputeHashCode(Production production, int position)
        {
            return HashCode.Compute(production.GetHashCode(), position.GetHashCode());
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

        private readonly int _hashCode;
    }
}