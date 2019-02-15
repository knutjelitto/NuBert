using Pliant.Collections;
using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Charts
{
    public class EarleySet : IEarleySet
    {
        private static readonly INormalState[] EmptyNormalStates = { };
        private static readonly ITransitionState[] EmptyTransitionStates = { };
        private UniqueList<INormalState> _predictions;
        private UniqueList<INormalState> _scans;
        private UniqueList<INormalState> _completions;
        private UniqueList<ITransitionState> _transitions;

        public IReadOnlyList<INormalState> Predictions
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

        public IReadOnlyList<INormalState> Scans
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

        public IReadOnlyList<INormalState> Completions
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

        public IReadOnlyList<ITransitionState> Transitions
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

        public bool Contains(StateType stateType, IDottedRule dottedRule, int origin)
        {
            if (stateType != StateType.Normal)
            {
                return false;
            }

            var hashCode = NormalStateHashCodeAlgorithm.Compute(dottedRule, origin);
            if (dottedRule.IsComplete)
            {
                return CompletionsContainsHash(hashCode);
            }

            var currentSymbol = dottedRule.PostDotSymbol;
            if (currentSymbol.SymbolType == SymbolType.NonTerminal)
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

        public bool Enqueue(IState state)
        {
            if (state.StateType == StateType.Transitive)
            {
                return EnqueueTransition(state as ITransitionState);
            }

            return EnqueueNormal(state, state as INormalState);
        }

        private bool EnqueueNormal(IState state, INormalState normalState)
        {
            var dottedRule = state.DottedRule;
            if (!dottedRule.IsComplete)
            {
                var currentSymbol = dottedRule.PostDotSymbol;
                if (currentSymbol.SymbolType == SymbolType.NonTerminal)
                {
                    return AddUniquePrediction(normalState);
                }

                return AddUniqueScan(normalState);
            }

            return AddUniqueCompletion(normalState);
        }

        private bool AddUniqueCompletion(INormalState normalState)
        {
            if (this._completions == null)
            {
                this._completions = new UniqueList<INormalState>();
            }

            return this._completions.AddUnique(normalState);
        }

        private bool AddUniqueScan(INormalState normalState)
        {
            if (this._scans == null)
            {
                this._scans = new UniqueList<INormalState>();
            }

            return this._scans.AddUnique(normalState);
        }

        private bool AddUniquePrediction(INormalState normalState)
        {
            if (this._predictions == null)
            {
                this._predictions = new UniqueList<INormalState>();
            }

            return this._predictions.AddUnique(normalState);
        }

        private bool EnqueueTransition(ITransitionState transitionState)
        {
            if (this._transitions == null)
            {
                this._transitions = new UniqueList<ITransitionState>();
            }

            return this._transitions.AddUnique(transitionState);
        }

        public ITransitionState FindTransitionState(ISymbol searchSymbol)
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

        public INormalState FindSourceState(ISymbol searchSymbol)
        {
            var sourceItemCount = 0;
            INormalState sourceItem = null;

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