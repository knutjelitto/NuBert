using Pliant.Collections;
using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Charts
{
    public class DeterministicSet
    {
        private readonly UniqueList<DeterministicState> _states;
        private readonly Dictionary<ISymbol, CachedDottedRuleSetTransition> _transitions;

        public IReadOnlyList<DeterministicState> States => this._states;

        public IReadOnlyDictionary<ISymbol, CachedDottedRuleSetTransition> CachedTransitions => this._transitions;

        public int Location { get; private set; }

        public DeterministicSet(int location)
        {
            this._states = new UniqueList<DeterministicState>();
            this._transitions = new Dictionary<ISymbol, CachedDottedRuleSetTransition>();
            Location = location;
        }

        internal bool Enqueue(DeterministicState frame)
        {
            var hasEnqueued = this._states.AddUnique(frame);
            return hasEnqueued;
        }

        public bool IsLeoUnique(ISymbol symbol)
        {
            return !CachedTransitions.ContainsKey(symbol);
        }

        public CachedDottedRuleSetTransition FindCachedDottedRuleSetTransition(ISymbol searchSymbol)
        {
            if (this._transitions.TryGetValue(searchSymbol, out var transition))
            {
                return transition;
            }

            return null;
        }

        public void AddCachedTransition(CachedDottedRuleSetTransition cachedDottedRuleSetTransition)
        {
            this._transitions.Add(cachedDottedRuleSetTransition.Symbol, cachedDottedRuleSetTransition);
        }
    }
}
