using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    public sealed class TransitionState : State
    {
        public TransitionState(Symbol recognized, State transition, RuleState reduction, int index)
            : base(transition.DottedRule, transition.Origin, null)
        {
            Reduction = reduction;
            Recognized = recognized;
            Index = index;
            this.hashCode = (DottedRule, Origin, Recognized, Reduction, Index).GetHashCode();
        }

        public int Index { get; }
        public TransitionState NextTransition { get; set; }
        public Symbol Recognized { get; }
        public RuleState Reduction { get; }

        public override bool Equals(object obj)
        {
            return obj is TransitionState other &&
                   DottedRule.Equals(other.DottedRule) &&
                   Origin.Equals(other.Origin) &&
                   Recognized.Equals(other.Recognized) &&
                   Reduction.Equals(other.Reduction) &&
                   Index.Equals(other.Index);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public State GetTargetState()
        {
            var parameterTransitionStateHasNoParseNode = ParseNode == null;
            if (parameterTransitionStateHasNoParseNode)
            {
                return Reduction;
            }

            return this;
        }

        public override string ToString()
        {
            return $"{Recognized} : {Reduction}";
        }

        public override bool Enqueue(EarleySet set)
        {
            return set.Add(this);
        }

        private readonly int hashCode;
    }
}