﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pliant.Charts;
using Pliant.Collections;
using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Runtime
{
    public class MarpaParseEngine : IParseEngine
    {
        public MarpaParseEngine(PreComputedGrammar preComputedGrammar)
        {
            this._preComputedGrammar = preComputedGrammar;
            Chart = new DeterministicChart();
            Initialize();
        }

        public MarpaParseEngine(IGrammar grammar)
            : this(new PreComputedGrammar(grammar))
        {
        }

        public DeterministicChart Chart { get; }

        public IGrammar Grammar => this._preComputedGrammar.Grammar;

        public int Location { get; private set; }

        public IReadOnlyList<LexerRule> GetExpectedLexerRules()
        {
            var frameSets = Chart.Sets;
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
                this._expectedLexerRuleCache = new Dictionary<int, LexerRule[]>();
            }

            // if the hash is found in the cached lexer rule lists, return the cached array
            if (this._expectedLexerRuleCache.TryGetValue(hashCode, out var cachedLexerRules))
            {
                return cachedLexerRules;
            }

            // compute the new lexer rule array and add it to the cache
            var array = new LexerRule[count];
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

        public IInternalForestNode GetParseForestRootNode()
        {
            throw new NotImplementedException();
        }

        public bool IsAccepted()
        {
            var anyEarleySets = Chart.Sets.Count > 0;
            if (!anyEarleySets)
            {
                return false;
            }

            var lastDeterministicSetIndex = Chart.Sets.Count - 1;
            var lastDeterministicSet = Chart.Sets[lastDeterministicSetIndex];

            return AnyDeterministicStateAccepted(lastDeterministicSet);
        }

        public bool Pulse(IToken token)
        {
            ScanPass(Location, token);
            var tokenRecognized = Chart.Sets.Count > Location + 1;
            if (!tokenRecognized)
            {
                return false;
            }

            Location++;
            ReductionPass(Location);
            return true;
        }

        public bool Pulse(IReadOnlyList<IToken> tokens)
        {
            for (var i = 0; i < tokens.Count; i++)
            {
                ScanPass(Location, tokens[i]);
            }

            var tokenRecognized = Chart.Sets.Count > Location + 1;
            if (!tokenRecognized)
            {
                return false;
            }

            Location++;
            ReductionPass(Location);
            return true;
        }

        public void Reset()
        {
            Initialize();
        }

        private static DottedRuleAssortment Goto(DottedRuleAssortment fromAH)
        {
            return fromAH.NullTransition;
        }

        private static DottedRuleAssortment Goto(DottedRuleAssortment fromAH, Symbol symbol)
        {
            return fromAH.Reductions.GetOrReturnNull(symbol);
        }

        private static DottedRuleAssortment Goto(DottedRuleAssortment fromAH, IToken token)
        {
            return fromAH.TokenTransitions.GetOrReturnNull(token.TokenType);
        }

        private void AddEimPair(int iLoc, DottedRuleAssortment confirmedAH, int origLoc)
        {
            var confirmedEIM = new DeterministicState(confirmedAH, origLoc);
            var predictedAH = Goto(confirmedAH);
            Chart.Enqueue(iLoc, confirmedEIM);
            if (predictedAH == null)
            {
                return;
            }

            var predictedEIM = new DeterministicState(predictedAH, iLoc);
            Chart.Enqueue(iLoc, predictedEIM);
        }

        private bool AnyDeterministicStateAccepted(DeterministicSet lastFrameSet)
        {
            var lastDeterministicStateCount = lastFrameSet.States.Count;
            for (var i = 0; i < lastDeterministicStateCount; i++)
            {
                var deterministicState = lastFrameSet.States[i];
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

        private CachedDottedRuleSetTransition CreateTopCachedItem(
            DeterministicState stateFrame,
            Symbol postDotSymbol)
        {
            var origin = stateFrame.Origin;
            CachedDottedRuleSetTransition topCacheItem = null;
            // search for the top item in the leo chain
            while (true)
            {
                var originFrameSet = Chart.Sets[stateFrame.Origin];
                var nextCachedItem = originFrameSet.FindCachedDottedRuleSetTransition(postDotSymbol);
                if (nextCachedItem == null)
                {
                    break;
                }

                topCacheItem = nextCachedItem;
                if (origin == nextCachedItem.Origin)
                {
                    break;
                }

                origin = topCacheItem.Origin;
            }

            return new CachedDottedRuleSetTransition(
                postDotSymbol,
                stateFrame.DottedRuleSet,
                topCacheItem == null ? stateFrame.Origin : origin);
        }

        private void EarleyReductionOperation(int iLoc, DeterministicState fromEim, Symbol transSym)
        {
            var fromAH = fromEim.DottedRuleSet;
            var originLoc = fromEim.Origin;

            var toAH = Goto(fromAH, transSym);
            if (toAH == null)
            {
                return;
            }

            AddEimPair(iLoc, toAH, originLoc);
        }

        private void Initialize()
        {
            var start = this._preComputedGrammar.Start;
            AddEimPair(0, start, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsStartState(DottedRule state)
        {
            return state.Production.LeftHandSide.Is(this._preComputedGrammar.Grammar.Start);
        }

        private void LeoReductionOperation(int iLoc, CachedDottedRuleSetTransition fromLim)
        {
            var fromAH = fromLim.DottedRuleSet;
            var transSym = fromLim.Symbol;
            var originLoc = fromLim.Origin;

            var toAH = Goto(fromAH, transSym);
            if (toAH == null)
            {
                return;
            }

            AddEimPair(iLoc, toAH, originLoc);
        }

        private void MemoizeTransitions(int iLoc)
        {
            var frameSet = Chart.Sets[iLoc];
            // leo eligibility needs to be cached before creating the cached transition
            // if the size of the list is != 1, do not enter the cached frame transition
            var cachedTransitionsPool = SharedPools.Default<Dictionary<Symbol, CachedDottedRuleSetTransition>>();
            var cachedTransitions = ObjectPoolExtensions.Allocate(cachedTransitionsPool);
            var cachedCountPool = SharedPools.Default<Dictionary<Symbol, int>>();
            var cachedCount = ObjectPoolExtensions.Allocate(cachedCountPool);

            for (var i = 0; i < frameSet.States.Count; i++)
            {
                var stateFrame = frameSet.States[i];
                var frame = stateFrame.DottedRuleSet;

                foreach (var preComputedState in frame.Data)
                {
                    if (preComputedState.IsComplete)
                    {
                        continue;
                    }

                    var postDotSymbol = preComputedState.PostDotSymbol;
                    if (postDotSymbol is NonTerminal)
                    {
                        if (!this._preComputedGrammar.Grammar.IsRightRecursive(preComputedState.Production.LeftHandSide))
                        {
                            continue;
                        }

                        // to determine if the item is leo unique, cache it here
                        if (!cachedCount.TryGetValue(postDotSymbol, out var count))
                        {
                            cachedCount[postDotSymbol] = 1;
                            cachedTransitions[postDotSymbol] = CreateTopCachedItem(stateFrame, postDotSymbol);
                        }
                        else
                        {
                            cachedCount[postDotSymbol] = count + 1;
                        }
                    }
                }
            }

            // add all memoized leo items to the frameSet
            foreach (var symbol in cachedCount.Keys)
            {
                var count = cachedCount[symbol];
                if (count != 1)
                {
                    continue;
                }

                frameSet.AddCachedTransition(cachedTransitions[symbol]);
            }

            cachedTransitionsPool.ClearAndFree(cachedTransitions);
            cachedCountPool.ClearAndFree(cachedCount);
        }

        private void ReduceOneLeftHandSide(int iLoc, int origLoc, NonTerminal lhsSym)
        {
            var frameSet = Chart.Sets[origLoc];
            var transitionItem = frameSet.FindCachedDottedRuleSetTransition(lhsSym);
            if (transitionItem != null)
            {
                LeoReductionOperation(iLoc, transitionItem);
            }
            else
            {
                for (var i = 0; i < frameSet.States.Count; i++)
                {
                    var stateFrame = frameSet.States[i];
                    EarleyReductionOperation(iLoc, stateFrame, lhsSym);
                }
            }
        }

        private void ReductionPass(int iLoc)
        {
            var iES = Chart.Sets[iLoc];
            var processed = ObjectPoolExtensions.Allocate(SharedPools.Default<HashSet<Symbol>>());
            for (var i = 0; i < iES.States.Count; i++)
            {
                var workEIM = iES.States[i];
                var workAH = workEIM.DottedRuleSet;
                var origLoc = workEIM.Origin;

                foreach (var dottedRule in workAH.Data)
                {
                    if (!dottedRule.IsComplete)
                    {
                        continue;
                    }

                    var lhsSym = dottedRule.Production.LeftHandSide;
                    if (!processed.Add(lhsSym))
                    {
                        continue;
                    }

                    ReduceOneLeftHandSide(iLoc, origLoc, lhsSym);
                }

                processed.Clear();
            }

            SharedPools.Default<HashSet<Symbol>>().ClearAndFree(processed);
            MemoizeTransitions(iLoc);
        }

        private void ScanPass(int iLoc, IToken token)
        {
            var iES = Chart.Sets[iLoc];
            for (var i = 0; i < iES.States.Count; i++)
            {
                var workEIM = iES.States[i];
                var fromAH = workEIM.DottedRuleSet;
                var origLoc = workEIM.Origin;

                var toAH = Goto(fromAH, token);
                if (toAH == null)
                {
                    continue;
                }

                AddEimPair(iLoc + 1, toAH, origLoc);
            }
        }

        private static readonly LexerRule[] EmptyLexerRules = { };

        private Dictionary<int, LexerRule[]> _expectedLexerRuleCache;
        private BitArray _expectedLexerRuleIndicies;
        private readonly PreComputedGrammar _preComputedGrammar;
    }
}