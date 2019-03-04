using Pliant.Dotted;
using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    public sealed class NormalState : State
    {
        public NormalState(DottedRule dottedRule, int origin)
            : base(dottedRule, origin)
        {
            this._hashCode = ComputeHashCode();
        }

        public NormalState(DottedRule dottedRule, int origin, IForestNode parseNode)
            : this(dottedRule, origin)
        {
            ParseNode = parseNode;
        }

        public override bool Equals(object obj)
        {
            return obj is NormalState other && GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public bool IsSource(ISymbol searchSymbol)
        {
            var dottedRule = DottedRule;
            return !dottedRule.IsComplete && dottedRule.PostDotSymbol.Is(searchSymbol);
        }

        public override string ToString()
        {
            return $"{DottedRule}\t\t({Origin})";
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                DottedRule.Dot.GetHashCode(),
                Origin.GetHashCode(),
                DottedRule.Production.GetHashCode());
        }

        private readonly int _hashCode;
    }
}