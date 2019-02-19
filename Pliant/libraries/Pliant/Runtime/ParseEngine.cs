using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Pliant.Charts;
using Pliant.Diagnostics;
using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Runtime
{
    public class ParseEngine : IParseEngine
    {
        public ParseEngine(IGrammar grammar)
            : this(grammar, new ParseEngineOptions(true))
        {
        }

        public ParseEngine(IGrammar grammar, ParseEngineOptions options)
        {
            //this._dottedRuleRegistry = new DottedRuleRegistry().Seed(grammar);
            this._dottedRuleRegistry = grammar.DottedRules;
            StateFactory = new StateFactory(this._dottedRuleRegistry);
            Options = options;
            this._nodeSet = new ForestNodeSet();
            Grammar = grammar;
            Initialize();
        }

        public Chart Chart { get; private set; }

        public IGrammar Grammar { get; }

        public int Location { get; private set; }

        public ParseEngineOptions Options { get; }

        public StateFactory StateFactory { get; }

        public IReadOnlyList<LexerRule> GetExpectedLexerRules()
        {
            var earleySets = Chart.EarleySets;
            var currentIndex = earleySets.Count - 1;
            var currentEarleySet = earleySets[currentIndex];
            var scanStates = currentEarleySet.Scans;

            if (scanStates.Count == 0)
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

            // compute the lexer rule hash for caching the list of lexer rules
            // compute the unique lexer rule count 
            // set bits in the rule index bit array corresponding to the position of the lexer rule in the list of rules
            for (var s = 0; s < scanStates.Count; s++)
            {
                var scanState = scanStates[s];
                var postDotSymbol = scanState.DottedRule.PostDotSymbol;
                if (postDotSymbol != null && postDotSymbol is LexerRule lexerRule)
                {
                    var index = Grammar.GetLexerRuleIndex(lexerRule);

                    if (index < 0)
                    {
                        continue;
                    }

                    if (this._expectedLexerRuleIndicies[index])
                    {
                        continue;
                    }

                    count++;
                    this._expectedLexerRuleIndicies[index] = true;
                    hashCode = HashCode.ComputeIncrementalHash(lexerRule.GetHashCode(), hashCode, hashCode == 0);
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
            if (!IsAccepted())
            {
                throw new Exception("Unable to parse expression.");
            }

            var lastSet = Chart.EarleySets[Chart.Count - 1];
            var start = Grammar.Start;

            // PERF: Avoid Linq expressions due to lambda allocation
            for (var c = 0; c < lastSet.Completions.Count; c++)
            {
                var completion = lastSet.Completions[c];
                if (completion.DottedRule.Production.LeftHandSide.Equals(start) && completion.Origin == 0)
                {
                    return completion.ParseNode as IInternalForestNode;
                }
            }

            return null;
        }

        public bool IsAccepted()
        {
            var lastEarleySet = Chart.EarleySets[Chart.Count - 1];
            var startStateSymbol = Grammar.Start;

            // PERF: Avoid LINQ Any due to lambda allocation
            for (var c = 0; c < lastEarleySet.Completions.Count; c++)
            {
                var completion = lastEarleySet.Completions[c];
                if (completion.Origin == 0
                    && completion.DottedRule.Production.LeftHandSide.Value == startStateSymbol.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Pulse(IToken token)
        {
            ScanPass(Location, token);

            var tokenRecognized = Chart.EarleySets.Count > Location + 1;
            if (!tokenRecognized)
            {
                return false;
            }

            Location++;
            ReductionPass(Location);

            this._nodeSet.Clear();
            return true;
        }

        public bool Pulse(IReadOnlyList<IToken> tokens)
        {
            for (var i = 0; i < tokens.Count; i++)
            {
                ScanPass(Location, tokens[i]);
            }

            var tokenRecognized = Chart.EarleySets.Count > Location + 1;

            if (!tokenRecognized)
            {
                return false;
            }

            Location++;
            ReductionPass(Location);

            this._nodeSet.Clear();
            return true;
        }

        public void Reset()
        {
            Initialize();
        }

        private static string GetOriginStateOperationString(string operation, int origin, State state)
        {
            return $"{origin.ToString().PadRight(9)}{state.ToString().PadRight(100)}{operation}";
        }

        private void Complete(NormalState completed, int k)
        {
            if (completed.ParseNode == null)
            {
                completed.ParseNode = CreateNullParseNode(completed.DottedRule.Production.LeftHandSide, k);
            }

            var earleySet = Chart.EarleySets[completed.Origin];
            var searchSymbol = completed.DottedRule.Production.LeftHandSide;

            if (Options.OptimizeRightRecursion)
            {
                OptimizeReductionPath(searchSymbol, completed.Origin);
            }

            var transitionState = earleySet.FindTransitionState(searchSymbol);
            if (transitionState != null)
            {
                LeoComplete(transitionState, completed, k);
            }
            else
            {
                EarleyComplete(completed, k);
            }
        }

        private IForestNode CreateNullParseNode(Symbol symbol, int location)
        {
            var symbolNode = this._nodeSet.AddOrGetExistingSymbolNode(symbol, location, location);
            var token = new Token(string.Empty, location, EmptyTokenType);
            var nullNode = new TokenForestNode(token, location, location);
            symbolNode.AddUniqueFamily(nullNode);
            return symbolNode;
        }

        private IForestNode CreateParseNode(
            DottedRule nextDottedRule,
            int origin,
            IForestNode w,
            IForestNode v,
            int location)
        {
            Assert.IsNotNull(v, nameof(v));
            var anyPreDotRuleNull = true;
            if (nextDottedRule.Position > 1)
            {
                var predotPrecursorSymbol = nextDottedRule
                                            .Production
                                            .RightHandSide[nextDottedRule.Position - 2];
                anyPreDotRuleNull = IsSymbolTransativeNullable(predotPrecursorSymbol);
            }

            var anyPostDotRuleNull = IsSymbolTransativeNullable(nextDottedRule.PostDotSymbol);
            if (anyPreDotRuleNull && !anyPostDotRuleNull)
            {
                return v;
            }

            IInternalForestNode internalNode;
            if (anyPostDotRuleNull)
            {
                internalNode = this._nodeSet
                                   .AddOrGetExistingSymbolNode(nextDottedRule.Production.LeftHandSide, origin, location);
            }
            else
            {
                internalNode = this._nodeSet.AddOrGetExistingIntermediateNode(nextDottedRule, origin, location);
            }

            // if w = null and y doesn't have a family of children (v)
            if (w == null)
            {
                internalNode.AddUniqueFamily(v);
            }

            // if w != null and y doesn't have a family of children (w, v)            
            else
            {
                internalNode.AddUniqueFamily(w, v);
            }

            return internalNode;
        }

        private VirtualForestNode CreateVirtualParseNode(State completed, int k, TransitionState rootTransitionState)
        {
            if (!this._nodeSet.TryGetExistingVirtualNode(
                    k,
                    rootTransitionState,
                    out var virtualParseNode))
            {
                virtualParseNode = new VirtualForestNode(k, rootTransitionState, completed.ParseNode);
                this._nodeSet.AddNewVirtualNode(virtualParseNode);
            }
            else
            {
                virtualParseNode.AddUniquePath(
                    new VirtualForestNodePath(rootTransitionState, completed.ParseNode));
            }

            return virtualParseNode;
        }

        private void EarleyComplete(NormalState completed, int k)
        {
            var j = completed.Origin;
            var sourceEarleySet = Chart.EarleySets[j];

            for (var p = 0; p < sourceEarleySet.Predictions.Count; p++)
            {
                var prediction = sourceEarleySet.Predictions[p];
                if (!prediction.IsSource(completed.DottedRule.Production.LeftHandSide))
                {
                    continue;
                }

                var dottedRule = this._dottedRuleRegistry.GetNext(prediction.DottedRule);
                var origin = prediction.Origin;

                // this will not create a node if the state already exists
                var parseNode = CreateParseNode(
                    dottedRule,
                    origin,
                    prediction.ParseNode,
                    completed.ParseNode,
                    k);

                if (Chart.Contains(k, dottedRule, origin))
                {
                    continue;
                }

                var nextState = StateFactory.NewState(dottedRule, origin, parseNode);

                if (Chart.Enqueue(k, nextState))
                {
                    Log(CompleteLogName, k, nextState);
                }
            }
        }

        private void Initialize()
        {
            Location = 0;
            Chart = new Chart();
            this._expectedLexerRuleCache = null;
            this._expectedLexerRuleIndicies = null;
            var startProductions = Grammar.StartProductions();
            for (var s = 0; s < startProductions.Count; s++)
            {
                var startProduction = startProductions[s];
                var startState = StateFactory.NewState(startProduction, 0, 0);
                if (Chart.Enqueue(0, startState))
                {
                    Log(StartLogName, 0, startState);
                }
            }

            ReductionPass(Location);
        }

        /// <summary>
        /// Implements a check for leo quasi complete items
        /// </summary>
        /// <param name="state">the state to check for quasi completeness</param>
        /// <returns>true if quasi complete, false otherwise</returns>
        private bool IsNextStateQuasiComplete(State state)
        {
            var ruleCount = state.DottedRule.Production.RightHandSide.Count;
            if (ruleCount == 0)
            {
                return true;
            }

            var nextStatePosition = state.DottedRule.Position + 1;
            var isComplete = nextStatePosition == state.DottedRule.Production.RightHandSide.Count;
            if (isComplete)
            {
                return true;
            }

            // if all subsequent symbols are nullable
            for (var i = nextStatePosition; i < state.DottedRule.Production.RightHandSide.Count; i++)
            {
                var nextSymbol = state.DottedRule.Production.RightHandSide[nextStatePosition];
                var isSymbolNullable = IsSymbolNullable(nextSymbol);
                if (!isSymbolNullable)
                {
                    return false;
                }

                // From Page 4 of Leo's paper:
                //
                // "on a non-empty deterministic reduction path there always
                //  exists a topmost item if S =+> S is impossible.
                //  The easiest way to avoid problems in this respect is to augment
                //  the grammar with a new start symbol S'.
                //  this means adding the rule S'=>S as the start."
                //
                // to fix this, check if S can derive S. Basically if we are in the StartState state
                // and the StartState state is found and is nullable, exit with false
                if (state.DottedRule.Production.LeftHandSide == Grammar.Start &&
                    nextSymbol == Grammar.Start)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsSymbolNullable(Symbol symbol)
        {
            if (symbol == null)
            {
                return true;
            }

            if (symbol is NonTerminal nonTerminal)
            {
                return Grammar.IsNullable(nonTerminal);
            }

            return false;
        }

        private bool IsSymbolTransativeNullable(Symbol symbol)
        {
            if (symbol == null)
            {
                return true;
            }

            if (symbol is NonTerminal nonTerminal)
            {
                return Grammar.IsTransitiveNullable(nonTerminal);
            }

            return false;
        }

        private void LeoComplete(TransitionState transitionState, State completed, int k)
        {
            var earleySet = Chart.EarleySets[transitionState.Index];
            var rootTransitionState = earleySet.FindTransitionState(
                transitionState.DottedRule.PreDotSymbol);

            if (rootTransitionState == null)
            {
                rootTransitionState = transitionState;
            }

            var virtualParseNode = CreateVirtualParseNode(completed, k, rootTransitionState);

            var dottedRule = transitionState.DottedRule;
            var topmostItem = StateFactory.NewState(
                dottedRule,
                transitionState.Origin,
                virtualParseNode);

            if (Chart.Enqueue(k, topmostItem))
            {
                Log(CompleteLogName, k, topmostItem);
            }
        }

        private void Log(string operation, int origin, State state)
        {
            if (Options.LoggingEnabled)
            {
                Debug.WriteLine(GetOriginStateOperationString(operation, origin, state));
            }
        }

        private void LogScan(int origin, State state, IToken token)
        {
            if (Options.LoggingEnabled)
            {
                Debug.WriteLine($"{GetOriginStateOperationString("Scan", origin, state)} \"{token.Value}\"");
            }
        }

        private void OptimizeReductionPath(Symbol searchSymbol, int k)
        {
            State t_rule = null;
            TransitionState previousTransitionState = null;

            var visited = SharedPools.Default<HashSet<State>>().AllocateAndClear();
            OptimizeReductionPathRecursive(searchSymbol, k, ref t_rule, ref previousTransitionState, visited);
            SharedPools.Default<HashSet<State>>().ClearAndFree(visited);
        }

        private void OptimizeReductionPathRecursive(
            Symbol searchSymbol,
            int k,
            ref State t_rule,
            ref TransitionState previousTransitionState,
            HashSet<State> visited)
        {
            var earleySet = Chart.EarleySets[k];

            // if Ii contains a transitive item of the for [B -> b., A, k]
            var transitionState = earleySet.FindTransitionState(searchSymbol);
            if (transitionState != null)
            {
                // then t_rule := B-> b.; t_pos = k;
                previousTransitionState = transitionState;
                t_rule = transitionState;
                return;
            }

            // else if Ii contains exactly one item of the form [B -> a.Ab, k]
            var sourceState = earleySet.FindSourceState(searchSymbol);
            if (sourceState == null)
            {
                return;
            }

            if (!visited.Add(sourceState))
            {
                return;
            }

            // and [B-> aA.b, k] is quasi complete (is b null)
            if (!IsNextStateQuasiComplete(sourceState))
            {
                return;
            }

            // then t_rule := [B->aAb.]; t_pos=k;
            t_rule = StateFactory.NextState(sourceState);

            if (sourceState.Origin != k)
            {
                visited.Clear();
            }

            // T_Update(I0...Ik, B);
            OptimizeReductionPathRecursive(
                sourceState.DottedRule.Production.LeftHandSide,
                sourceState.Origin,
                ref t_rule,
                ref previousTransitionState,
                visited);

            if (t_rule == null)
            {
                return;
            }

            TransitionState currentTransitionState = null;
            if (previousTransitionState != null)
            {
                currentTransitionState = new TransitionState(
                    searchSymbol,
                    t_rule,
                    sourceState,
                    previousTransitionState.Index);

                previousTransitionState.NextTransition = currentTransitionState;
            }
            else
            {
                currentTransitionState = new TransitionState(
                    searchSymbol,
                    t_rule,
                    sourceState,
                    k);
            }

            if (Chart.Enqueue(k, currentTransitionState))
            {
                Log(TransitionLogName, k, currentTransitionState);
            }

            previousTransitionState = currentTransitionState;
        }

        private void Predict(NormalState evidence, int j)
        {
            var dottedRule = evidence.DottedRule;
            var nonTerminal = dottedRule.PostDotSymbol as NonTerminal;
            var rulesForNonTerminal = Grammar.RulesFor(nonTerminal);

            // PERF: Avoid boxing enumerable
            for (var p = 0; p < rulesForNonTerminal.Count; p++)
            {
                var production = rulesForNonTerminal[p];
                PredictProduction(j, production);
            }

            var isNullable = Grammar.IsTransitiveNullable(nonTerminal);
            if (isNullable)
            {
                PredictAycockHorspool(evidence, j);
            }
        }

        private void PredictAycockHorspool(NormalState evidence, int j)
        {
            var nullParseNode = CreateNullParseNode(evidence.DottedRule.PostDotSymbol, j);
            var dottedRule = this._dottedRuleRegistry.GetNext(evidence.DottedRule);

            var evidenceParseNode = evidence.ParseNode as IInternalForestNode;
            IForestNode parseNode = null;
            if (evidenceParseNode == null)
            {
                parseNode = CreateParseNode(
                    dottedRule,
                    evidence.Origin,
                    null,
                    nullParseNode,
                    j);
            }
            else if (evidenceParseNode.Children.Count > 0
                     && evidenceParseNode.Children[0].Children.Count > 0)
            {
                parseNode = CreateParseNode(
                    dottedRule,
                    evidence.Origin,
                    evidenceParseNode,
                    nullParseNode,
                    j);
            }

            if (Chart.Contains(j, dottedRule, evidence.Origin))
            {
                return;
            }

            var aycockHorspoolState = StateFactory.NewState(dottedRule, evidence.Origin, parseNode);
            if (Chart.Enqueue(j, aycockHorspoolState))
            {
                Log(PredictionLogName, j, aycockHorspoolState);
            }
        }

        private void PredictProduction(int j, Production production)
        {
            var dottedRule = this._dottedRuleRegistry.Get(production, 0);
            if (Chart.Contains(j, dottedRule, 0))
            {
                return;
            }

            // TODO: Pre-Compute Leo Items. If item is 1 step from being complete, add a transition item
            var predictedState = StateFactory.NewState(dottedRule, j);
            if (Chart.Enqueue(j, predictedState))
            {
                Log(PredictionLogName, j, predictedState);
            }
        }

        private void ReductionPass(int location)
        {
            var earleySet = Chart.EarleySets[location];
            var resume = true;

            var p = 0;
            var c = 0;

            while (resume)
            {
                // is there a new completion?
                if (c < earleySet.Completions.Count)
                {
                    var completion = earleySet.Completions[c];
                    Complete(completion, location);
                    c++;
                }
                // is there a new prediction?
                else if (p < earleySet.Predictions.Count)
                {
                    var predictions = earleySet.Predictions;

                    var evidence = predictions[p];
                    Predict(evidence, location);

                    p++;
                }
                else
                {
                    resume = false;
                }
            }
        }

        private void Scan(NormalState scan, int j, IToken token)
        {
            var i = scan.Origin;
            var currentSymbol = scan.DottedRule.PostDotSymbol;

            if (currentSymbol is LexerRule lexerRule && token.TokenType.Equals(lexerRule.TokenType))
            {
                var dottedRule = this._dottedRuleRegistry.GetNext(scan.DottedRule);
                if (Chart.Contains(j + 1, dottedRule, i))
                {
                    return;
                }

                var tokenNode = this._nodeSet.AddOrGetExistingTokenNode(token);
                var parseNode = CreateParseNode(
                    dottedRule,
                    scan.Origin,
                    scan.ParseNode,
                    tokenNode,
                    j + 1);
                var nextState = StateFactory.NewState(dottedRule, scan.Origin, parseNode);

                if (Chart.Enqueue(j + 1, nextState))
                {
                    LogScan(j + 1, nextState, token);
                }
            }
        }

        private void ScanPass(int location, IToken token)
        {
            var earleySet = Chart.EarleySets[location];
            for (var s = 0; s < earleySet.Scans.Count; s++)
            {
                var scanState = earleySet.Scans[s];
                Scan(scanState, location, token);
            }
        }

        private static readonly LexerRule[] EmptyLexerRules = { };

        private static readonly TokenType EmptyTokenType = new TokenType(string.Empty);

        private readonly DottedRuleRegistry _dottedRuleRegistry;

        private Dictionary<int, LexerRule[]> _expectedLexerRuleCache;
        private BitArray _expectedLexerRuleIndicies;
        private readonly ForestNodeSet _nodeSet;

        private const string PredictionLogName = "Predict";
        private const string StartLogName = "StartState";
        private const string CompleteLogName = "Complete";
        private const string TransitionLogName = "Transition";
    }
}