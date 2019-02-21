using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public sealed class Production
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

        public bool IsEmpty => RightHandSide.Count == 0;
        public NonTerminal LeftHandSide { get; }
        public IReadOnlyList<Symbol> RightHandSide { get; }

        public override bool Equals(object obj)
        {
            return obj is Production other &&
                   LeftHandSide.Equals(other.LeftHandSide) &&
                   RightHandSide.SequenceEqual(other.RightHandSide);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0} ->", LeftHandSide.Value);

            foreach (var symbol in RightHandSide)
            {
                stringBuilder.AppendFormat(" {0}", symbol);
            }

            return stringBuilder.ToString();
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(LeftHandSide.GetHashCode(), HashCode.Compute(RightHandSide));
        }

        // PERF: Cache Costly Hash Code Computation
        private readonly int _hashCode;
    }
}