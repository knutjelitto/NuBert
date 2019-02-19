using Pliant.Collections;
using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Charts
{
    public class EarleySet : IEarleySet
    {
        private static readonly NormalState[] EmptyNormalStates = { };
        private static readonly TransitionState[] EmptyTransitionStates = { };
        private UniqueList<NormalState> _predictions;
        private UniqueList<NormalState> _scans;
        private UniqueList<NormalState> _completions;
        private UniqueList<TransitionState> _transitions;

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

        public int Location { get; private set; }

        public EarleySet(int location)
        {
            Location = location;
        }

        public bool ContainsNormal(IDottedRule dottedRule, int origin)
        {
            var hashCode = NormalStateHashCodeAlgorithm.Compute(dottedRule, origin);
            if (dottedRule.IsComplete)
            {
                return CompletionsContainsHash(hashCode);
            }

            var currentSymbol = dottedRule.PostDotSymbol;
            if (currentSymbol is NonTerminal)
            {
                return PredictionsContainsHash(hashCode);
            }

            return ScansContainsHash(hashCode);
        }

        private bool CompletionsContainsHash(int hashCode)
        {
            if (this._completions == null)
            {
                return false;
            }

            return this._completions.ContainsHash(hashCode);
        }

        private bool PredictionsContainsHash(int hashCode)
        {
            if (this._predictions == null)
            {
                return false;
            }

            return this._predictions.ContainsHash(hashCode);
        }

        private bool ScansContainsHash(int hashCode)
        {
            if (this._scans == null)
            {
                return false;
            }

            return this._scans.ContainsHash(hashCode);
        }

        public bool Enqueue(State state)
        {
            if (state.StateType == StateType.Transitive)
            {
                return EnqueueTransition(state as TransitionState);
            }

            return EnqueueNormal(state, state as NormalState);
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

        private bool AddUniqueCompletion(NormalState normalState)
        {
            if (this._completions == null)
            {
                this._completions = new UniqueList<NormalState>();
            }

            return this._completions.AddUnique(normalState);
        }

        private bool AddUniqueScan(NormalState normalState)
        {
            if (this._scans == null)
            {
                this._scans = new UniqueList<NormalState>();
            }

            return this._scans.AddUnique(normalState);
        }

        private bool AddUniquePrediction(NormalState normalState)
        {
            if (this._predictions == null)
            {
                this._predictions = new UniqueList<NormalState>();
            }

            return this._predictions.AddUnique(normalState);
        }

        private bool EnqueueTransition(TransitionState transitionState)
        {
            if (this._transitions == null)
            {
                this._transitions = new UniqueList<TransitionState>();
            }

            return this._transitions.AddUnique(transitionState);
        }

        public TransitionState FindTransitionState(ISymbol searchSymbol)
        {
            for (var t = 0; t < Transitions.Count; t++)
            {
                var transitionState = Transitions[t] as TransitionState;
                if (transitionState.Recognized.Equals(searchSymbol))
                {
                    return transitionState;
                }
            }
            return null;
        }

        public NormalState FindSourceState(ISymbol searchSymbol)
        {
            var sourceItemCount = 0;
            NormalState sourceItem = null;

            for (var s = 0; s < Predictions.Count; s++)
            {
                var state = Predictions[s];
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
    }
}