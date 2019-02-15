using System;
using Pliant.Tokens;
using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Forest;
using System.Collections.Generic;
using Pliant.Utilities;
using System.Runtime.CompilerServices;
using System.Collections;

namespace Pliant.Runtime
{
    public class DeterministicParseEngine : IParseEngine
    {
        private readonly PreComputedGrammar _precomputedGrammar;
        private DeterministicChart _chart;

        public int Location { get; private set; }

        public IGrammar Grammar => this._precomputedGrammar.Grammar;

        public DeterministicParseEngine(IGrammar grammar)
            : this(new PreComputedGrammar(grammar))
        {
        }

        public DeterministicParseEngine(PreComputedGrammar preComputedGrammar)
        {
            this._precomputedGrammar = preComputedGrammar;
            Initialize();
        }
        
        private void Initialize()
        {
            Location = 0;
            this._chart = new DeterministicChart();
            var kernelDottedRuleSet = this._precomputedGrammar.Start;
            Enqueue(Location, new DeterministicState(kernelDottedRuleSet, 0));
            Reduce(Location);
        }

        private bool Enqueue(int location, DeterministicState deterministicState)
        {
            if (!this._chart.Enqueue(location, deterministicState))
            {
                return false;
            }

            if (deterministicState.DottedRuleSet.NullTransition == null)
            {
                return true;
            }

            var nullTransitionDeterministicState = new DeterministicState(
                deterministicState.DottedRuleSet.NullTransition,
                location);

            return this._chart.Enqueue(location, nullTransitionDeterministicState);
        }

        public bool Pulse(IToken token)
        {
            Scan(Location, token);
            var tokenRecognized = this._chart.Sets.Count > Location + 1;
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
            for(var i=0;i<tokens.Count;i++)
            {
                Scan(Location, tokens[i]);
            }

            var tokenRecognized = this._chart.Sets.Count > Location + 1;
            if (!tokenRecognized)
            {
                return false;
            }

            Location++;
            Reduce(Location);
            return true;
        }

        public bool IsAccepted()
        {
            var anyEarleySets = this._chart.Sets.Count > 0;
            if (!anyEarleySets)
            {
                return false;
            }

            var lastDeterministicSetIndex = this._chart.Sets.Count - 1;
            var lastDeterministicSet = this._chart.Sets[lastDeterministicSetIndex];
            
            return AnyDeterministicSetAccepted(lastDeterministicSet);
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

        private bool AnyPreComputedStateAccepted(IReadOnlyList<IDottedRule> states)
        {
            for (var j = 0; j < states.Count; j++)
            {
                var preComputedState = states[j];
                if (!IsComplete(preComputedState))
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

        private void Reduce(int i)
        {
            var set = this._chart.Sets[i];
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
            var parentSet = this._chart.Sets[parent];
            var parentSetDeterministicStates = parentSet.States;
            var parentSetDeterministicStateCount = parentSetDeterministicStates.Count;

            for (var d = 0; d < dottedRuleSet.Data.Count; ++d)
            {
                var preComputedState = dottedRuleSet.Data[d];

                var production = preComputedState.Production;
                
                if (!preComputedState.IsComplete)
                {
                    continue;
                }

                var leftHandSide = production.LeftHandSide;

                for (var p = 0; p < parentSetDeterministicStateCount; p++)
                {
                    var pState = parentSetDeterministicStates[p];
                    var pParent = pState.Origin;

                    if (!pState.DottedRuleSet.Reductions.TryGetValue(leftHandSide, out var target))
                    {
                        continue;
                    }

                    if (!this._chart.Enqueue(i, new DeterministicState(target, pParent)))
                    {
                        continue;
                    }

                    if (target.NullTransition == null)
                    {
                        continue;
                    }

                    this._chart.Enqueue(i, new DeterministicState(target.NullTransition, i));
                }
            }
        }

        private void Scan(int location, IToken token)
        {
            var set = this._chart.Sets[location];
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
            if (!dottedRuleSet.TokenTransitions.TryGetValue(token.TokenType, out var target))
            {
                return;
            }

            if (!this._chart.Enqueue(location + 1, new DeterministicState(target, parent)))
            {
                return;
            }

            if (target.NullTransition == null)
            {
                return;
            }

            this._chart.Enqueue(location + 1, new DeterministicState(target.NullTransition, location + 1));
        }
        
        public void Reset()
        {
            this._chart.Clear();
        }

        public IInternalForestNode GetParseForestRootNode()
        {
            throw new NotImplementedException();
        }

        private Dictionary<int, ILexerRule[]> _expectedLexerRuleCache;
        private static readonly ILexerRule[] EmptyLexerRules = { };
        private BitArray _expectedLexerRuleIndicies;

        public IReadOnlyList<ILexerRule> GetExpectedLexerRules()
        {
            var frameSets = this._chart.Sets;
            var frameSetCount = frameSets.Count;

            if (frameSetCount == 0)
            {
                return EmptyLexerRules;
            }

            var hashCode = 0;
            var count = 0;

            if (this._expectedLexerRuleIndicies == null)
            {
                this._expectedLexerRuleIndicies = new BitArray(Grammar.LexerRules.Count);
            }
            else
            {
                this._expectedLexerRuleIndicies.SetAll(false);
            }

            var frameSet = frameSets[frameSets.Count - 1];
            for (var i = 0; i < frameSet.States.Count; i++)
            {
                var stateFrame = frameSet.States[i];
                for (var j = 0; j < stateFrame.DottedRuleSet.ScanKeys.Count; j++)
                {
                    var lexerRule = stateFrame.DottedRuleSet.ScanKeys[j];
                    var index = Grammar.GetLexerRuleIndex(lexerRule);
                    if (index < 0)
                    {
                        continue;
                    }

                    if (this._expectedLexerRuleIndicies[index])
                    {
                        continue;
                    }

                    this._expectedLexerRuleIndicies[index] = true;
                    hashCode = HashCode.ComputeIncrementalHash(lexerRule.GetHashCode(), hashCode, count == 0);
                    count++;
                }
            }

            if (this._expectedLexerRuleCache == null)
            {
                this._expectedLexerRuleCache = new Dictionary<int, ILexerRule[]>();
            }

            // if the hash is found in the cached lexer rule lists, return the cached array
            if (this._expectedLexerRuleCache.TryGetValue(hashCode, out var cachedLexerRules))
            {
                return cachedLexerRules;
            }

            // compute the new lexer rule array and add it to the cache
            var array = new ILexerRule[count];
            var returnItemIndex = 0;
            for (var i = 0; i < Grammar.LexerRules.Count; i++)
            {
                if (this._expectedLexerRuleIndicies[i])
                {
                    array[returnItemIndex] = Grammar.LexerRules[i];
                    returnItemIndex++;
                }
            }

            this._expectedLexerRuleCache.Add(hashCode, array);

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsStartState(IDottedRule state)
        {
            var start = Grammar.Start;
            return state.Production.LeftHandSide.Equals(start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsComplete(IDottedRule preComputedState)
        {
            return preComputedState.Position == preComputedState.Production.RightHandSide.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ISymbol GetPostDotSymbol(IDottedRule preComputedState)
        {
            return preComputedState.Production.RightHandSide[preComputedState.Position];
        }
    }
}
