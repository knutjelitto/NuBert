using System.Collections.Generic;
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
        }

        public IReadOnlyList<CompletedState> Completions => this.completions;

        public IReadOnlyList<PredictionState> Predictions => this.predictions;

        public IReadOnlyList<ScanState> Scans => this.scans;

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
    }
}