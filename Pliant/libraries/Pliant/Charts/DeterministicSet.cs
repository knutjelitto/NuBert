using Pliant.Collections;
using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Charts
{
    public class DeterministicSet
    {
        private readonly UniqueList<DeterministicState> _states;
        private readonly Dictionary<Symbol, CachedDottedRuleSetTransition> _transitions;

        public IReadOnlyList<DeterministicState> States => this._states;

        public IReadOnlyDictionary<Symbol, CachedDottedRuleSetTransition> CachedTransitions => this._transitions;

        public int Location { get; private set; }

        public DeterministicSet(int location)
        {
            this._states = new UniqueList<DeterministicState>();
            this._transitions = new Dictionary<Symbol, CachedDottedRuleSetTransition>();
            Location = location;
        }

        internal bool Enqueue(DeterministicState frame)
        {
            var hasEnqueued = this._states.AddUnique(frame);
            return hasEnqueued;
        }

        public bool IsLeoUnique(Symbol symbol)
        {
            return !CachedTransitions.ContainsKey(symbol);
        }

        public CachedDottedRuleSetTransition FindCachedDottedRuleSetTransition(Symbol searchSymbol)
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
