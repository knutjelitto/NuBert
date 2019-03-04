﻿using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    public sealed class TransitionState : State
    {
        public TransitionState(
            Symbol recognized,
            State transition,
            StateBase reduction,
            int index)
            : base(transition.DottedRule, transition.Origin, null)
        {
            Reduction = reduction;
            Recognized = recognized;
            Index = index;
            this.hashCode = ComputeHashCode();
        }

        public int Index { get; }

        public TransitionState NextTransition { get; set; }

        public Symbol Recognized { get; }
        public StateBase Reduction { get; }

        public override bool Equals(object obj)
        {
            return obj is TransitionState transitionState &&
                   GetHashCode() == transitionState.GetHashCode() &&
                   Recognized.Equals(transitionState.Recognized) &&
                   Index == transitionState.Index;
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

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                Origin.GetHashCode(),
                DottedRule.Dot.GetHashCode(),
                DottedRule.Production.GetHashCode(),
                Recognized.GetHashCode(),
                Reduction.GetHashCode(),
                Index.GetHashCode());
        }

        public override bool Enqueue(EarleySet set)
        {
            return set.Add(this);
        }

        private readonly int hashCode;
    }
}