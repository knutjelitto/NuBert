using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public sealed class Production : ValueEqualityBase<Production>, IReadOnlyList<Symbol>
    {
        public Production(NonTerminal leftHandSide, IReadOnlyList<Symbol> rightHandSide)
            : base((leftHandSide, HashCode.Compute(rightHandSide)))
        {
            LeftHandSide = leftHandSide;
            RightHandSide = rightHandSide.ToArray();
        }

        public Production(NonTerminal leftHandSide, params Symbol[] rightHandSide)
            : this(leftHandSide, (IReadOnlyList<Symbol>)rightHandSide)
        {
        }

        public int Count => RightHandSide.Count;

        public NonTerminal LeftHandSide { get; }
        public IReadOnlyList<Symbol> RightHandSide { get; }

        public Symbol this[int index] => RightHandSide[index];

        public override bool ThisEquals(Production other)
        {
            return LeftHandSide.Equals(other.LeftHandSide) &&
                   RightHandSide.SequenceEqual(other.RightHandSide);
        }

        public IEnumerator<Symbol> GetEnumerator()
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