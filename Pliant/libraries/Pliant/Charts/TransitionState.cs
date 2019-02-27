using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    public sealed class TransitionState : State
    {
        public TransitionState(
            Symbol recognized,
            State transition,
            NormalState reduction,
            int index)
            : base(transition.DottedRule, transition.Origin)
        {
            Reduction = reduction;
            Recognized = recognized;
            Index = index;
            this._hashCode = ComputeHashCode();
        }

        public int Index { get; }

        public TransitionState NextTransition { get; set; }
        public Symbol Recognized { get; }

        public NormalState Reduction { get; }

        public override bool Equals(object obj)
        {
            return obj is TransitionState transitionState && 
                   GetHashCode() == transitionState.GetHashCode() && 
                   Recognized.Equals(transitionState.Recognized) && 
                   Index == transitionState.Index;
        }

        public override int GetHashCode()
        {
            return this._hashCode;
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

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                DottedRule.Dot.GetHashCode(),
                Origin.GetHashCode(),
                DottedRule.Production.GetHashCode(),
                Recognized.GetHashCode(),
                Reduction.GetHashCode(),
                Index.GetHashCode());
        }

        private readonly int _hashCode;
    }
}