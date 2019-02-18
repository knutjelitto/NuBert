using System.Collections.Generic;
using System.Text;
using Pliant.Diagnostics;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class Production
    {
        public Production(NonTerminal leftHandSide, List<Symbol> rightHandSide)
        {
            Assert.IsNotNull(leftHandSide, nameof(leftHandSide));
            Assert.IsNotNull(rightHandSide, nameof(rightHandSide));
            LeftHandSide = leftHandSide;
            this._rightHandSide = new List<Symbol>(new List<Symbol>(rightHandSide));
            this._hashCode = ComputeHashCode();
        }

        public Production(NonTerminal leftHandSide, params Symbol[] rightHandSide)
        {
            Assert.IsNotNull(leftHandSide, nameof(leftHandSide));
            Assert.IsNotNull(rightHandSide, nameof(rightHandSide));
            LeftHandSide = leftHandSide;
            this._rightHandSide = new List<Symbol>(new List<Symbol>(rightHandSide));
            this._hashCode = ComputeHashCode();
        }

        public bool IsEmpty => this._rightHandSide.Count == 0;
        public NonTerminal LeftHandSide { get; }

        public IReadOnlyList<Symbol> RightHandSide => this._rightHandSide;

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var production = obj as Production;
            if (production == null)
            {
                return false;
            }

            if (!LeftHandSide.Equals(production.LeftHandSide))
            {
                return false;
            }

            var rightHandSideCount = RightHandSide.Count;
            if (rightHandSideCount != production.RightHandSide.Count)
            {
                return false;
            }

            for (var i = 0; i < rightHandSideCount; i++)
            {
                if (!RightHandSide[i].Equals(production.RightHandSide[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0} ->", LeftHandSide.Value);

            for (var p = 0; p < RightHandSide.Count; p++)
            {
                var symbol = RightHandSide[p];
                stringBuilder.AppendFormat(" {0}", symbol);
            }

            return stringBuilder.ToString();
        }

        private int ComputeHashCode()
        {
            var hash = HashCode.ComputeIncrementalHash(LeftHandSide.GetHashCode(), 0, true);

            for (var s = 0; s < RightHandSide.Count; s++)
            {
                var symbol = RightHandSide[s];
                hash = HashCode.ComputeIncrementalHash(symbol.GetHashCode(), hash);
            }

            return hash;
        }

        // PERF: Cache Costly Hash Code Computation
        private readonly int _hashCode;

        private readonly List<Symbol> _rightHandSide;
    }
}