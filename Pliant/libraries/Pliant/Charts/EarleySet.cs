using System;
using System.Collections.Generic;
using Pliant.Collections;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public class EarleySet : IEarleySet
    {
        public EarleySet(int location)
        {
            Location = location;
        }

        public IReadOnlyList<NormalState> Completions
        {
            get
            {
                if (this._completions == null)
                {
                    return EmptyNormalStates;
                }

                return this._completions;
            }
        }

        public int Location { get; }

        public IReadOnlyList<NormalState> Predictions
        {
            get
            {
                if (this._predictions == null)
                {
                    return EmptyNormalStates;
                }

                return this._predictions;
            }
        }

        public IReadOnlyList<NormalState> Scans
        {
            get
            {
                if (this._scans == null)
                {
                    return EmptyNormalStates;
                }

                return this._scans;
            }
        }

        public IReadOnlyList<TransitionState> Transitions
        {
            get
            {
                if (this._transitions == null)
                {
                    return EmptyTransitionStates;
                }

                return this._transitions;
            }
        }

        public bool ContainsNormal(DottedRule dottedRule, int origin)
        {
            if (dottedRule.IsComplete)
            {
                return CompletionContains(dottedRule, origin);
            }

            var currentSymbol = dottedRule.PostDotSymbol;
            if (currentSymbol is NonTerminal)
            {
                return PredictionsContains(dottedRule, origin);
            }

            return ScansContains(dottedRule, origin);
        }

        public bool Enqueue(State state)
        {
            if (state is TransitionState transition)
            {
                return EnqueueTransition(transition);
            }

            if (state is NormalState normal)
            {
                return EnqueueNormal(state, normal);
            }

            throw new NotImplementedException();
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

        private bool AddUniqueCompletion(NormalState normalState)
        {
            if (this._completions == null)
            {
                this._completions = new NormalStateList();
            }

            return this._completions.AddUnique(normalState);
        }

        private bool AddUniquePrediction(NormalState normalState)
        {
            if (this._predictions == null)
            {
                this._predictions = new NormalStateList();
            }

            return this._predictions.AddUnique(normalState);
        }

        private bool AddUniqueScan(NormalState normalState)
        {
            if (this._scans == null)
            {
                this._scans = new NormalStateList();
            }

            return this._scans.AddUnique(normalState);
        }

        private bool CompletionContains(DottedRule rule, int origin)
        {
            if (this._completions == null)
            {
                return false;
            }

            return this._completions.Contains(rule, origin);
        }

        private bool EnqueueNormal(State state, NormalState normalState)
        {
            var dottedRule = state.DottedRule;
            if (!dottedRule.IsComplete)
            {
                var currentSymbol = dottedRule.PostDotSymbol;
                if (currentSymbol is NonTerminal)
                {
                    return AddUniquePrediction(normalState);
                }

                return AddUniqueScan(normalState);
            }

            return AddUniqueCompletion(normalState);
        }

        private bool EnqueueTransition(TransitionState transitionState)
        {
            if (this._transitions == null)
            {
                this._transitions = new UniqueList<TransitionState>();
            }

            return this._transitions.AddUnique(transitionState);
        }

        private bool PredictionsContains(DottedRule rule, int origin)
        {
            if (this._predictions == null)
            {
                return false;
            }

            return this._predictions.Contains(rule, origin);
        }

        private bool ScansContains(DottedRule rule, int origin)
        {
            if (this._scans == null)
            {
                return false;
            }

            return this._scans.Contains(rule, origin);
        }

        private static readonly NormalState[] EmptyNormalStates = { };
        private static readonly TransitionState[] EmptyTransitionStates = { };
        private NormalStateList _completions;
        private NormalStateList _predictions;
        private NormalStateList _scans;
        private UniqueList<TransitionState> _transitions;
    }
}