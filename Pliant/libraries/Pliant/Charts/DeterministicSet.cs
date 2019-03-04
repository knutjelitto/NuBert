using System.Collections.Generic;
using Pliant.Collections;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public class DeterministicSet
    {
        public DeterministicSet(int location)
        {
            Location = location;
            this.states = new UniqueList<DeterministicState>();
            this.transitions = new Dictionary<Symbol, CachedDottedRuleSetTransition>();
        }

        public IReadOnlyDictionary<Symbol, CachedDottedRuleSetTransition> CachedTransitions => this.transitions;

        public int Location { get; }

        public IReadOnlyList<DeterministicState> States => this.states;

        public void AddCachedTransition(CachedDottedRuleSetTransition cachedDottedRuleSetTransition)
        {
            this.transitions.Add(cachedDottedRuleSetTransition.Symbol, cachedDottedRuleSetTransition);
        }

        public CachedDottedRuleSetTransition FindCachedDottedRuleSetTransition(Symbol searchSymbol)
        {
            if (this.transitions.TryGetValue(searchSymbol, out var transition))
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
            return this.states.AddUnique(frame);
        }

        private readonly UniqueList<DeterministicState> states;
        private readonly Dictionary<Symbol, CachedDottedRuleSetTransition> transitions;
    }
}