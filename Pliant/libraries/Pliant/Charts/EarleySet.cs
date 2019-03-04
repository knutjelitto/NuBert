using System;
using System.Collections.Generic;
using System.Diagnostics;
using Pliant.Collections;
using Pliant.Dotted;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public class EarleySet
    {
        public EarleySet()
        {
            this.completions = new StateList<CompletionState>();
            this.predictions = new StateList<PredictState>();
            this.scans = new NormalStateList();
            this.transitions = new UniqueList<TransitionState>();
        }

        public IReadOnlyList<NormalState> Completions => this.completions;

        public IReadOnlyList<NormalState> Predictions => this.predictions;

        public IReadOnlyList<NormalState> Scans => this.scans;

        public IReadOnlyList<TransitionState> Transitions => this.transitions;

        public bool Add(CompletionState state)
        {
            return this.completions.AddUnique(state);
        }

        public bool Add(PredictState state)
        {
            return this.predictions.AddUnique(state);
        }

        public bool Add(ScanState state)
        {
            return this.scans.AddUnique(state);
        }

        public bool Add(TransitionState state)
        {
            return this.transitions.AddUnique(state);
        }

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

        public bool Enqueue(TransitionState state)
        {
            return state.Enqueue(this);
        }

        public bool FindUniqueSourceState(Symbol searchSymbol, out NormalState sourceItem)
        {
            var sourceItemCount = 0;
            sourceItem = null;

            foreach (var state in Predictions)
            {
                if (state.IsSource(searchSymbol))
                {
                    var moreThanOneSourceItemExists = sourceItemCount > 0;
                    if (moreThanOneSourceItemExists)
                    {
                        return false;
                    }

                    sourceItemCount++;
                    sourceItem = state;
                }
            }

            return sourceItemCount == 1;
        }

        public bool FindTransitionState(Symbol searchSymbol, out TransitionState transitionState)
        {
            foreach (var transition in Transitions)
            {
                if (transition.Recognized.Equals(searchSymbol))
                {
                    transitionState = transition;
                    return true;
                }
            }

            transitionState = null;
            return false;
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
                if (dottedRule.PostDotSymbol is NonTerminal)
                {
                    return this.predictions.AddUnique(normalState as PredictState);
                }

                return this.scans.AddUnique(normalState as ScanState);
            }

            Debug.Assert(normalState is CompletionState);
            return this.completions.AddUnique(normalState as CompletionState);
        }

        private bool EnqueueTransition(TransitionState transitionState)
        {
            return this.transitions.AddUnique(transitionState);
        }

        private readonly StateList<CompletionState> completions;
        private readonly StateList<PredictState> predictions;
        private readonly NormalStateList scans;
        private readonly UniqueList<TransitionState> transitions;
    }
}