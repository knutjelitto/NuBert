using System.Collections.Generic;
using Pliant.Collections;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public class EarleySet
    {
        public EarleySet()
        {
            this.completions = new StateList<CompletedState>();
            this.predictions = new StateList<PredictionState>();
            this.scans = new StateList<ScanState>();
            this.transitions = new UniqueList<TransitionState>();
        }

        public IReadOnlyList<CompletedState> Completions => this.completions;

        public IReadOnlyList<PredictionState> Predictions => this.predictions;

        public IReadOnlyList<ScanState> Scans => this.scans;

        public IReadOnlyList<TransitionState> Transitions => this.transitions;

        public bool Add(CompletedState state)
        {
            return this.completions.AddUnique(state);
        }

        public bool Add(PredictionState state)
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

        public bool Contains(DottedRule dottedRule, int origin)
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

        public bool FindUniqueSourceState(Symbol searchSymbol, out PredictionState sourceItem)
        {
            var sourceItemCount = 0;
            sourceItem = null;

            foreach (var prediction in Predictions)
            {
                if (prediction.IsSource(searchSymbol))
                {
                    var moreThanOneSourceItemExists = sourceItemCount > 0;
                    if (moreThanOneSourceItemExists)
                    {
                        return false;
                    }

                    sourceItemCount++;
                    sourceItem = prediction;
                }
            }

            return sourceItemCount == 1;
        }

        public bool FindTransitionState(NonTerminal searchSymbol, out TransitionState transitionState)
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

        private readonly StateList<CompletedState> completions;
        private readonly StateList<PredictionState> predictions;
        private readonly StateList<ScanState> scans;
        private readonly UniqueList<TransitionState> transitions;
    }
}