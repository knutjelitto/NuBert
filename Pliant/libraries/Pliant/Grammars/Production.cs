using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public sealed class Production : ValueEqualityBase<Production>, IReadOnlyList<Symbol>
    {
        private Production(NonTerminal leftHandSide, Symbol[] rightHandSide)
        {
            LeftHandSide = leftHandSide;
            this.rightHandSide = rightHandSide;
        }

        public int Count => this.rightHandSide.Length;

        public NonTerminal LeftHandSide { get; }

        public Symbol this[int index] => this.rightHandSide[index];

        protected override object ThisHashCode => (LeftHandSide, HashCode.Compute(this.rightHandSide));

        public static Production From(NonTerminal leftHandSide, params Symbol[] rightHandSide)
        {
            return new Production(leftHandSide, rightHandSide);
        }

        public IEnumerator<Symbol> GetEnumerator()
        {
            return this.rightHandSide.AsEnumerable().GetEnumerator();
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"{LeftHandSide} ->");

            foreach (var symbol in this.rightHandSide)
            {
                stringBuilder.Append($" {symbol}");
            }

            return stringBuilder.ToString();
        }

        protected override bool ThisEquals(Production other)
        {
            return LeftHandSide.Equals(other.LeftHandSide) && this.rightHandSide.SequenceEqual(other.rightHandSide);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly Symbol[] rightHandSide;
    }
}