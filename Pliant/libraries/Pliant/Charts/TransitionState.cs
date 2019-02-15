using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    public class TransitionState : StateBase, ITransitionState
    {
        public ISymbol Recognized { get; private set; }

        public INormalState Reduction { get; private set; }

        public int Index { get; private set; }

        public ITransitionState NextTransition { get; set; }
                
        public TransitionState(
            ISymbol recognized,
            IState transition,
            INormalState reduction,
            int index)
            : base(transition.DottedRule, transition.Origin)
        {
            Reduction = reduction;
            Recognized = recognized;
            Index = index;
            this._hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var transitionState = obj as TransitionState;
            if (transitionState == null)
            {
                return false;
            }

            return GetHashCode() == transitionState.GetHashCode()
                && Recognized.Equals(transitionState.Recognized)
                && Index == transitionState.Index;
        }
        
        private readonly int _hashCode;

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                DottedRule.Position.GetHashCode(),
                Origin.GetHashCode(),
                DottedRule.Production.GetHashCode(),
                Recognized.GetHashCode(),
                Reduction.GetHashCode(),
                Index.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return $"{Recognized} : {Reduction}";
        }

        public override StateType StateType => StateType.Transitive;

        public IState GetTargetState()
        {
            var parameterTransitionStateHasNoParseNode = ParseNode == null;
            if (parameterTransitionStateHasNoParseNode)
            {
                return Reduction;
            }

            return this;
        }
    }
}