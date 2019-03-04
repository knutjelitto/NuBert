using System;
using System.Collections.Generic;
using Pliant.Collections;
using Pliant.Dotted;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public class EarleySet
    {
        public EarleySet()
        {
            this.completions = new NormalStateList();
            this.predictions = new NormalStateList();
            this.scans = new NormalStateList();
            this.transitions = new UniqueList<TransitionState>();
        }

        public IReadOnlyList<NormalState> Completions => this.completions;

        public IReadOnlyList<NormalState> Predictions => this.predictions;

        public IReadOnlyList<NormalState> Scans => this.scans;

        public IReadOnlyList<TransitionState> Transitions => this.transitions;

        public bool ContainsNormal(DottedRule dottedRule, int origin)
        {
            if (dottedRule.IsComplete)
            {
                return CompletionContains(dottedRule, origin);
            }

            var currentSymbol = dottedRule.PostDotSymbol;
            return currentSymbol is NonTerminal
                       ? PredictionsContains(dottedRule, origin) 
                       : ScansContains(dottedRule, origin);
        }

        public bool Enqueue(State state)
        {
            switch (state)
            {
                case TransitionState transition:
                    return EnqueueTransition(transition);
                case NormalState normal:
                    return EnqueueNormal(normal);
            }

            throw new InvalidOperationException();
        }

        public NormalState FindSourceState(Symbol searchSymbol)
        {
            var sourceItemCount = 0;
            NormalState sourceItem = null;

            foreach (var state in Predictions)
            {
                if (state.IsSource(searchSymbol))
                {
                    var moreThanOneSourceItemExists = sourceItemCount > 0;
                    if (moreThanOneSourceItemExists)
                    {
                        return null;
                    }

                    sourceItemCount++;
                    sourceItem = state;
                }
            }

            return sourceItem;
        }

        public TransitionState FindTransitionState(Symbol searchSymbol)
        {
            foreach (var transition in Transitions)
            {
                var transitionState = transition;
                if (transitionState.Recognized.Equals(searchSymbol))
                {
                    return transitionState;
                }
            }

            return null;
        }

        private bool CompletionContains(DottedRule rule, int origin)
        {
            return this.completions.Contains(rule, origin);
        }

        private bool PredictionsContains(DottedRule rule, int origin)
        {
            return this.predictions.Contains(rule, origin);
        }

        private bool ScansContains(DottedRule rule, int origin)
        {
            return this.scans.Contains(rule, origin);
        }

        private bool EnqueueNormal(NormalState normalState)
        {
            var dottedRule = normalState.DottedRule;
            if (!dottedRule.IsComplete)
            {
                return dottedRule.PostDotSymbol is NonTerminal
                           ? this.predictions.AddUnique(normalState)
                           : this.scans.AddUnique(normalState);
            }

            return this.completions.AddUnique(normalState);
        }

        private bool EnqueueTransition(TransitionState transitionState)
        {
            return this.transitions.AddUnique(transitionState);
        }

        private readonly NormalStateList completions;
        private readonly NormalStateList predictions;
        private readonly NormalStateList scans;
        private readonly UniqueList<TransitionState> transitions;
    }
}