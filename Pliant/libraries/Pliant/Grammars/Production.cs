using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public sealed class Production : IReadOnlyList<Symbol>
    {
        public Production(NonTerminal leftHandSide, IEnumerable<Symbol> rightHandSide)
        {
            LeftHandSide = leftHandSide;
            RightHandSide = rightHandSide.ToList();
            this._hashCode = ComputeHashCode();
        }

        public Production(NonTerminal leftHandSide, params Symbol[] rightHandSide)
            : this(leftHandSide, rightHandSide.AsEnumerable())
        {
        }

        public int Count => RightHandSide.Count;

        public NonTerminal LeftHandSide { get; }
        public IReadOnlyList<Symbol> RightHandSide { get; }

        public Symbol this[int index] => RightHandSide[index];

        public override bool Equals(object obj)
        {
            return obj is Production other &&
                   LeftHandSide.Equals(other.LeftHandSide) &&
                   RightHandSide.SequenceEqual(other.RightHandSide);
        }

        public IEnumerator<Symbol> GetEnumerator()
        {
            return RightHandSide.GetEnumerator();
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"{LeftHandSide} ->");

            foreach (var symbol in RightHandSide)
            {
                stringBuilder.Append($" {symbol}");
            }

            return stringBuilder.ToString();
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(LeftHandSide.GetHashCode(), HashCode.Compute(RightHandSide));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly int _hashCode;
    }
}