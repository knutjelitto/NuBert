using System.Collections.Generic;
using Pliant.Collections;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class PreComputedGrammar
    {
        public PreComputedGrammar(IGrammar grammar)
        {
            this._dottedRuleSetQueue = new ProcessOnceQueue<DottedRuleSet>();
            this._dottedRuleSets = new Dictionary<DottedRuleSet, DottedRuleSet>();

            Grammar = grammar;

            var startStates = Initialize(Grammar);
            Start = AddNewOrGetExistingDottedRuleSet(startStates);
            ProcessDottedRuleSetQueue();
        }

        public IGrammar Grammar { get; }

        public DottedRuleSet Start { get; }

        private static Symbol GetPostDotSymbol(DottedRule state)
        {
            return state.Production.RightHandSide[state.Dot];
        }

        private static bool IsComplete(DottedRule state)
        {
            return state.Dot == state.Production.RightHandSide.Count;
        }

        private DottedRuleSet AddNewOrGetExistingDottedRuleSet(DottedSet states)
        {
            var dottedRuleSet = new DottedRuleSet(states);
            if (this._dottedRuleSets.TryGetValue(dottedRuleSet, out var outDottedRuleSet))
            {
                return outDottedRuleSet;
            }

            outDottedRuleSet = dottedRuleSet;
            this._dottedRuleSets[dottedRuleSet] = outDottedRuleSet;
            this._dottedRuleSetQueue.Enqueue(outDottedRuleSet);
            return outDottedRuleSet;
        }

        private DottedSet GetConfirmedStates(DottedSet states)
        {
            var queue = new Queue<DottedRule>();
            var closure = new DottedSet();

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
                for (var s = state.Dot; s < state.Production.RightHandSide.Count; s++)
                {
                    var postDotSymbol = production.RightHandSide[s];
                    if (postDotSymbol is NonTerminal nonTerminalPostDotSymbol)
                    {
                        if (!Grammar.IsTransativeNullable(nonTerminalPostDotSymbol))
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

            return closure;
        }

        private DottedRule GetPreComputedState(Production production, int position)
        {
            return Grammar.DottedRules.Get(production, position);
        }

        private DottedSet GetPredictedStates(DottedRuleSet frame)
        {
            var queue = new Queue<DottedRule>();
            var closure = new DottedSet();

            for (var i = 0; i < frame.Data.Count; i++)
            {
                var state = frame.Data[i];
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
                    if (Grammar.IsTransativeNullable(nonTerminalPostDotSymbol))
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

                    var predictions = Grammar.RulesFor(nonTerminalPostDotSymbol);
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

            return closure;
        }

        private DottedSet Initialize(IGrammar grammar)
        {
            var startStates = new DottedSet();
            var startProductions = grammar.StartProductions();

            for (var p = 0; p < startProductions.Count; p++)
            {
                var production = startProductions[p];
                var state = GetPreComputedState(production, 0);
                startStates.Add(state);
            }

            var confirmedStates = GetConfirmedStates(startStates);

            return confirmedStates;
        }

        private void ProcessDottedRuleSetQueue()
        {
            while (this._dottedRuleSetQueue.Count > 0)
            {
                // assume the closure has already been captured
                var frame = this._dottedRuleSetQueue.Dequeue();
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
            var transitions = new Dictionary<Symbol, DottedSet>();

            for (var i = 0; i < frame.Data.Count; i++)
            {
                var nfaState = frame.Data[i];
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
        }

        private bool TryGetOrCreateDottedRuleSet(DottedSet states, out DottedRuleSet outDottedRuleSet)
        {
            var dottedRuleSet = new DottedRuleSet(states);
            if (this._dottedRuleSets.TryGetValue(dottedRuleSet, out outDottedRuleSet))
            {
                return true;
            }

            outDottedRuleSet = dottedRuleSet;
            this._dottedRuleSets[dottedRuleSet] = outDottedRuleSet;
            this._dottedRuleSetQueue.Enqueue(outDottedRuleSet);
            return false;
        }

        private readonly ProcessOnceQueue<DottedRuleSet> _dottedRuleSetQueue;

        private readonly Dictionary<DottedRuleSet, DottedRuleSet> _dottedRuleSets;
    }
}