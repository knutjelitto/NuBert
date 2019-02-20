using System.Collections.Generic;
using Pliant.Collections;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public class DeterministicSet
    {
        public DeterministicSet(int location)
        {
            this._states = new UniqueList<DeterministicState>();
            this._transitions = new Dictionary<Symbol, CachedDottedRuleSetTransition>();
            Location = location;
        }

        public IReadOnlyDictionary<Symbol, CachedDottedRuleSetTransition> CachedTransitions => this._transitions;

        public int Location { get; }

        public IReadOnlyList<DeterministicState> States => this._states;

        public void AddCachedTransition(CachedDottedRuleSetTransition cachedDottedRuleSetTransition)
        {
            this._transitions.Add(cachedDottedRuleSetTransition.Symbol, cachedDottedRuleSetTransition);
        }

        public CachedDottedRuleSetTransition FindCachedDottedRuleSetTransition(Symbol searchSymbol)
        {
            if (this._transitions.TryGetValue(searchSymbol, out var transition))
            {
                return transition;
            }

            return null;
        }

        public bool IsLeoUnique(Symbol symbol)
        {
            return !CachedTransitions.ContainsKey(symbol);
        }

        internal bool Enqueue(DeterministicState frame)
        {
            var hasEnqueued = this._states.AddUnique(frame);
            return hasEnqueued;
        }

        private readonly UniqueList<DeterministicState> _states;
        private readonly Dictionary<Symbol, CachedDottedRuleSetTransition> _transitions;
    }
}