using Pliant.Dotted;
using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    public class NormalState : State
    {
        public NormalState(DottedRule dottedRule, int origin, IForestNode parseNode = null)
            : base(dottedRule, origin, parseNode)
        {
            this._hashCode = (DottedRule, Origin).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is NormalState other && 
                   DottedRule.Equals(other.DottedRule) &&
                   Origin.Equals(other.Origin);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public bool IsSource(Symbol searchSymbol)
        {
            var dottedRule = DottedRule;
            return !dottedRule.IsComplete && dottedRule.PostDotSymbol.Is(searchSymbol);
        }

        public override string ToString()
        {
            return $"{DottedRule}\t\t({Origin})";
        }

        private readonly int _hashCode;
    }
}