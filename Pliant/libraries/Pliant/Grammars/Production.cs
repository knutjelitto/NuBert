using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public sealed class Production : ValueEqualityBase<Production>, IReadOnlyList<ISymbol>
    {
        public Production(NonTerminal leftHandSide, IReadOnlyList<ISymbol> rightHandSide)
            : base((leftHandSide, HashCode.Compute(rightHandSide)))
        {
            LeftHandSide = leftHandSide;
            RightHandSide = rightHandSide.ToArray();
        }

        public Production(NonTerminal leftHandSide, params ISymbol[] rightHandSide)
            : this(leftHandSide, (IReadOnlyList<ISymbol>)rightHandSide)
        {
        }

        public int Count => RightHandSide.Count;

        public NonTerminal LeftHandSide { get; }
        public IReadOnlyList<ISymbol> RightHandSide { get; }

        public ISymbol this[int index] => RightHandSide[index];

        public override bool ThisEquals(Production other)
        {
            return LeftHandSide.Equals(other.LeftHandSide) &&
                   RightHandSide.SequenceEqual(other.RightHandSide);
        }

        public IEnumerator<ISymbol> GetEnumerator()
        {
            return RightHandSide.GetEnumerator();
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}