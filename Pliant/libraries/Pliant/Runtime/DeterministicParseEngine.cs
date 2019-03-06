using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pliant.Charts;
using Pliant.Dotted;
using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Runtime
{
    public class DeterministicParseEngine : IParseEngine
    {
        public DeterministicParseEngine(Grammar grammar)
            : this(new PreComputedGrammar(grammar))
        {
        }

        public DeterministicParseEngine(PreComputedGrammar preComputedGrammar)
        {
            this.precomputedGrammar = preComputedGrammar;
            Initialize();
        }

        public Grammar Grammar => this.precomputedGrammar.Grammar;

        public int Location { get; private set; }

        public IReadOnlyList<LexerRule> GetExpectedLexerRules()
        {
            var frameSets = this.chart.Sets;
            var frameSetCount = frameSets.Count;

            if (frameSetCount == 0)
            {
                return EmptyLexerRules;
            }

            var hashCode = 0;
            var count = 0;

            if (this.expectedLexerRuleIndicies == null)
            {
                this.expectedLexerRuleIndicies = new BitArray(Grammar.LexerRules.Count);
            }
            else
            {
                this.expectedLexerRuleIndicies.SetAll(false);
            }

            var frameSet = frameSets[frameSets.Count - 1];
            for (var i = 0; i < frameSet.States.Count; i++)
            {
                var stateFrame = frameSet.States[i];
                for (var j = 0; j < stateFrame.DottedRuleSet.ScanKeys.Count; j++)
                {
                    var lexerRule = stateFrame.DottedRuleSet.ScanKeys[j];
                    var index = Grammar.GetLexerIndex(lexerRule);
                    if (index < 0)
                    {
                        continue;
                    }

                    if (this.expectedLexerRuleIndicies[index])
                    {
                        continue;
                    }

                    this.expectedLexerRuleIndicies[index] = true;
                    hashCode = HashCode.ComputeIncrementalHash(lexerRule.GetHashCode(), hashCode, count == 0);
                    count++;
                }
            }

            if (this.expectedLexerRuleCache == null)
            {
                this.expectedLexerRuleCache = new Dictionary<int, LexerRule[]>();
            }

            // if the hash is found in the cached lexer rule lists, return the cached array
            if (this.expectedLexerRuleCache.TryGetValue(hashCode, out var cachedLexerRules))
            {
                return cachedLexerRules;
            }

            // compute the new lexer rule array and add it to the cache
            var array = new LexerRule[count];
            var returnItemIndex = 0;
            for (var i = 0; i < Grammar.LexerRules.Count; i++)
            {
                if (this.expectedLexerRuleIndicies[i])
                {
                    array[returnItemIndex] = Grammar.LexerRules[i];
                    returnItemIndex++;
                }
            }

            this.expectedLexerRuleCache.Add(hashCode, array);

            return array;
        }

        public IInternalForestNode GetParseForestRootNode()
        {
            throw new NotImplementedException();
        }

        public bool IsAccepted()
        {
            var anyEarleySets = this.chart.Sets.Count > 0;
            if (!anyEarleySets)
            {
                return false;
            }

            var lastDeterministicSetIndex = this.chart.Sets.Count - 1;
            var lastDeterministicSet = this.chart.Sets[lastDeterministicSetIndex];

            return AnyDeterministicSetAccepted(lastDeterministicSet);
        }

        public bool Pulse(IToken token)
        {
            Scan(Location, token);
            var tokenRecognized = this.chart.Sets.Count > Location + 1;
            if (!tokenRecognized)
            {
                return false;
            }

            Location++;
            Reduce(Location);
            return true;
        }

        public bool Pulse(IReadOnlyList<IToken> tokens)
        {
            for (var i = 0; i < tokens.Count; i++)
            {
                Scan(Location, tokens[i]);
            }

            var tokenRecognized = this.chart.Sets.Count > Location + 1;
            if (!tokenRecognized)
            {
                return false;
            }

            Location++;
            Reduce(Location);
            return true;
        }


        private bool AnyDeterministicSetAccepted(DeterministicSet lastDeterministicSet)
        {
            var lastDeterministicSetStateCount = lastDeterministicSet.States.Count;
            for (var i = 0; i < lastDeterministicSetStateCount; i++)
            {
                var deterministicState = lastDeterministicSet.States[i];
                var originIsFirstEarleySet = deterministicState.Origin == 0;
                if (!originIsFirstEarleySet)
                {
                    continue;
                }

                if (AnyPreComputedStateAccepted(deterministicState.DottedRuleSet.Data))
                {
                    return true;
                }
            }

            return false;
        }

        private bool AnyPreComputedStateAccepted(IEnumerable<DottedRule> states)
        {
            foreach (var preComputedState in states)
            {
                if (!preComputedState.IsComplete)
                {
                    continue;
                }

                if (!IsStartState(preComputedState))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private void Enqueue(int location, DeterministicState deterministicState)
        {
            if (this.chart.Enqueue(location, deterministicState) && deterministicState.DottedRuleSet.NullTransition != null)
            {
                var nullTransitionDeterministicState = new DeterministicState(
                    deterministicState.DottedRuleSet.NullTransition,
                    location);

                this.chart.Enqueue(location, nullTransitionDeterministicState);
            }
        }

        private void Initialize()
        {
            Location = 0;
            this.chart = new DeterministicChart();
            var kernelDottedRuleSet = this.precomputedGrammar.Start;
            Enqueue(Location, new DeterministicState(kernelDottedRuleSet, 0));
            Reduce(Location);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsStartState(DottedRule state)
        {
            return state.Production.LeftHandSide.Is(Grammar.Start);
        }

        private void Reduce(int i)
        {
            var set = this.chart.Sets[i];
            for (var f = 0; f < set.States.Count; f++)
            {
                var state = set.States[f];
                var parent = state.Origin;
                var frame = state.DottedRuleSet;

                if (parent == i)
                {
                    continue;
                }

                ReduceDottedRuleSet(i, parent, frame);
            }
        }

        private void ReduceDottedRuleSet(int i, int parent, DottedRuleSet dottedRuleSet)
        {
            var parentSet = this.chart.Sets[parent];
            var parentSetDeterministicStates = parentSet.States;
            var parentSetDeterministicStateCount = parentSetDeterministicStates.Count;

            foreach (var preComputedState in dottedRuleSet.Data)
            {
                var production = preComputedState.Production;

                if (!preComputedState.IsComplete)
                {
                    continue;
                }

                for (var p = 0; p < parentSetDeterministicStateCount; p++)
                {
                    var pState = parentSetDeterministicStates[p];
                    var pParent = pState.Origin;

                    if (!pState.DottedRuleSet.Reductions.TryGetValue(production.LeftHandSide, out var target))
                    {
                        continue;
                    }

                    if (!this.chart.Enqueue(i, new DeterministicState(target, pParent)))
                    {
                        continue;
                    }

                    if (target.NullTransition == null)
                    {
                        continue;
                    }

                    this.chart.Enqueue(i, new DeterministicState(target.NullTransition, i));
                }
            }
        }

        private void Scan(int location, IToken token)
        {
            var set = this.chart.Sets[location];
            var states = set.States;
            var stateCount = states.Count;

            for (var f = 0; f < stateCount; f++)
            {
                var deterministicState = states[f];
                var parentOrigin = deterministicState.Origin;
                var dottedRuleSet = deterministicState.DottedRuleSet;

                ScanDottedRuleSet(location, token, parentOrigin, dottedRuleSet);
            }
        }

        private void ScanDottedRuleSet(int location, IToken token, int parent, DottedRuleSet dottedRuleSet)
        {
            //PERF: This could perhaps be improved with an int array and direct index lookup based on "token.TokenType.Id"?...
            if (!dottedRuleSet.TokenTransitions.TryGetValue(token.TokenClass, out var target))
            {
                return;
            }

            if (!this.chart.Enqueue(location + 1, new DeterministicState(target, parent)))
            {
                return;
            }

            if (target.NullTransition == null)
            {
                return;
            }

            this.chart.Enqueue(location + 1, new DeterministicState(target.NullTransition, location + 1));
        }

        private static readonly LexerRule[] EmptyLexerRules = { };
        private DeterministicChart chart;

        private Dictionary<int, LexerRule[]> expectedLexerRuleCache;
        private BitArray expectedLexerRuleIndicies;
        private readonly PreComputedGrammar precomputedGrammar;
    }
}