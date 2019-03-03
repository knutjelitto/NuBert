using System.Collections.Generic;
using Pliant.Collections;
using Pliant.Dotted;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class PreComputedGrammar
    {
        public PreComputedGrammar(IGrammar grammar)
        {
            this.dottedRuleSetQueue = new ProcessOnceQueue<DottedRuleSet>();
            this.dottedRuleSets = new Dictionary<DottedRuleSet, DottedRuleSet>();

            Grammar = grammar;

            var startStates = Initialize(Grammar);
            Start = AddNewOrGetExistingDottedRuleSet(startStates);
            ProcessDottedRuleSetQueue();
        }

        public IGrammar Grammar { get; }

        public DottedRuleSet Start { get; }

        private static Symbol GetPostDotSymbol(DottedRule state)
        {
            return state.Production[state.Dot];
        }

        private static bool IsComplete(DottedRule state)
        {
            return state.IsComplete;
        }

        private DottedRuleSet AddNewOrGetExistingDottedRuleSet(HashSet<DottedRule> states)
        {
            var dottedRuleSet = new DottedRuleSet(states);
            if (this.dottedRuleSets.TryGetValue(dottedRuleSet, out var outDottedRuleSet))
            {
                return outDottedRuleSet;
            }

            outDottedRuleSet = dottedRuleSet;
            this.dottedRuleSets[dottedRuleSet] = outDottedRuleSet;
            this.dottedRuleSetQueue.Enqueue(outDottedRuleSet);
            return outDottedRuleSet;
        }

        private HashSet<DottedRule> GetConfirmedStates(HashSet<DottedRule> states)
        {
            var pool = SharedPools.Default<Queue<DottedRule>>();

            var queue = ObjectPoolExtensions.Allocate(pool);
            var closure = new HashSet<DottedRule>();

            foreach (var state in states)
            {
                if (closure.Add(state))
                {
                    queue.Enqueue(state);
                }
            }

            while (queue.Count > 0)
            {
                var state = queue.Dequeue();
                if (IsComplete(state))
                {
                    continue;
                }

                var production = state.Production;
                for (var s = state.Dot; s < state.Production.Count; s++)
                {
                    var postDotSymbol = production[s];
                    if (postDotSymbol is NonTerminal nonTerminalPostDotSymbol)
                    {
                        if (!Grammar.IsTransitiveNullable(nonTerminalPostDotSymbol))
                        {
                            break;
                        }

                        var preComputedState = GetPreComputedState(production, s + 1);
                        if (closure.Add(preComputedState))
                        {
                            queue.Enqueue(preComputedState);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            pool.ClearAndFree(queue);
            return closure;
        }

        private DottedRule GetPreComputedState(Production production, int position)
        {
            return Grammar.DottedRules.Get(production, position);
        }

        private HashSet<DottedRule> GetPredictedStates(DottedRuleSet frame)
        {
            var pool = SharedPools.Default<Queue<DottedRule>>();

            var queue = ObjectPoolExtensions.Allocate(pool);
            var closure = new HashSet<DottedRule>();

            foreach (var state in frame.Data)
            {
                if (!IsComplete(state))
                {
                    queue.Enqueue(state);
                }
            }

            while (queue.Count > 0)
            {
                var state = queue.Dequeue();
                if (IsComplete(state))
                {
                    continue;
                }

                var postDotSymbol = GetPostDotSymbol(state);
                if (postDotSymbol is NonTerminal nonTerminalPostDotSymbol)
                {
                    if (Grammar.IsTransitiveNullable(nonTerminalPostDotSymbol))
                    {
                        var preComputedState = GetPreComputedState(state.Production, state.Dot + 1);
                        if (!frame.Contains(preComputedState))
                        {
                            if (closure.Add(preComputedState))
                            {
                                if (!IsComplete(preComputedState))
                                {
                                    queue.Enqueue(preComputedState);
                                }
                            }
                        }
                    }

                    var predictions = Grammar.ProductionsFor(nonTerminalPostDotSymbol);
                    for (var p = 0; p < predictions.Count; p++)
                    {
                        var prediction = predictions[p];
                        var preComputedState = GetPreComputedState(prediction, 0);
                        if (frame.Contains(preComputedState))
                        {
                            continue;
                        }

                        if (!closure.Add(preComputedState))
                        {
                            continue;
                        }

                        if (!IsComplete(preComputedState))
                        {
                            queue.Enqueue(preComputedState);
                        }
                    }
                }
            }

            pool.ClearAndFree(queue);
            return closure;
        }

        private HashSet<DottedRule> Initialize(IGrammar grammar)
        {
            var pool = SharedPools.Default<HashSet<DottedRule>>();

            var startStates = ObjectPoolExtensions.Allocate(pool);

            foreach (var production in grammar.StartProductions())
            {
                var state = GetPreComputedState(production, 0);
                startStates.Add(state);
            }

            var confirmedStates = GetConfirmedStates(startStates);

            pool.ClearAndFree(startStates);
            return confirmedStates;
        }

        private void ProcessDottedRuleSetQueue()
        {
            while (this.dottedRuleSetQueue.Count > 0)
            {
                // assume the closure has already been captured
                var frame = this.dottedRuleSetQueue.Dequeue();
                ProcessSymbolTransitions(frame);

                // capture the predictions for the frame
                var predictedStates = GetPredictedStates(frame);

                // if no predictions, continue
                if (predictedStates.Count == 0)
                {
                    continue;
                }

                // assign the null transition
                // only process symbols on the null frame if it is new
                if (!TryGetOrCreateDottedRuleSet(predictedStates, out var nullDottedRuleSet))
                {
                    ProcessSymbolTransitions(nullDottedRuleSet);
                }

                frame.NullTransition = nullDottedRuleSet;
            }
        }

        private void ProcessSymbolTransitions(DottedRuleSet frame)
        {
            var pool = SharedPools.Default<Dictionary<Symbol, HashSet<DottedRule>>>();
            var transitions = ObjectPoolExtensions.Allocate(pool);

            foreach (var nfaState in frame.Data)
            {
                if (IsComplete(nfaState))
                {
                    continue;
                }

                var postDotSymbol = GetPostDotSymbol(nfaState);
                var targetStates = transitions.AddOrGetExisting(postDotSymbol);
                var nextRule = GetPreComputedState(nfaState.Production, nfaState.Dot + 1);

                targetStates.Add(nextRule);
            }

            foreach (var symbol in transitions.Keys)
            {
                var confirmedStates = GetConfirmedStates(transitions[symbol]);
                var valueDottedRuleSet = AddNewOrGetExistingDottedRuleSet(confirmedStates);
                frame.AddTransition(symbol, valueDottedRuleSet);
            }

            pool.ClearAndFree(transitions);
        }

        private bool TryGetOrCreateDottedRuleSet(HashSet<DottedRule> states, out DottedRuleSet outDottedRuleSet)
        {
            var dottedRuleSet = new DottedRuleSet(states);
            if (this.dottedRuleSets.TryGetValue(dottedRuleSet, out outDottedRuleSet))
            {
                return true;
            }

            outDottedRuleSet = dottedRuleSet;
            this.dottedRuleSets[dottedRuleSet] = outDottedRuleSet;
            this.dottedRuleSetQueue.Enqueue(outDottedRuleSet);
            return false;
        }

        private readonly ProcessOnceQueue<DottedRuleSet> dottedRuleSetQueue;

        private readonly Dictionary<DottedRuleSet, DottedRuleSet> dottedRuleSets;
    }
}