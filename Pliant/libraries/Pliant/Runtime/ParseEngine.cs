using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Pliant.Charts;
using Pliant.Diagnostics;
using Pliant.Dotted;
using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Runtime
{
    public class ParseEngine : IParseEngine
    {
        public ParseEngine(Grammar grammar)
            : this(grammar, new ParseEngineOptions())
        {
        }

        public ParseEngine(Grammar grammar, ParseEngineOptions options)
        {
            Grammar = grammar;
            Options = options;
            NodeSet = new ForestNodeSet();
            StateFactory = new StateFactory(DottedRules);
            Initialize();
        }

        public Chart Chart { get; private set; }

        public Grammar Grammar { get; }

        public int Location { get; private set; }

        public IReadOnlyList<Lexer> GetExpectedLexerRules()
        {
            var currentEarleySet = Chart.LastSet();
            var scanStates = currentEarleySet.Scans;

            if (scanStates.Count == 0)
            {
                return emptyLexerRules;
            }

            var numbers = scanStates
                          .Select(state => state.DottedRule.PostDotSymbol)
                          .OfType<Lexer>()
                          .Select(lexerRule => Grammar.GetLexerIndex(lexerRule))
                          .Where(index => index >= 0);

            var indices = new Indices(numbers);


            // if the hash is found in the cached lexer rule lists, return the cached array
            if (this.expectedLexerRuleCache.TryGetValue(indices, out var cachedLexerRules))
            {
                return cachedLexerRules;
            }

            // compute the new lexer rule array and add it to the cache
            var array = indices.GetLexerRules(Grammar.LexerRules);

            this.expectedLexerRuleCache.Add(indices, array);

            return array;
        }

        public IInternalForestNode GetParseForestRootNode()
        {
            var lastSet = Chart.LastSet();

            // PERF: Avoid Linq expressions due to lambda allocation
            foreach (var completion in lastSet.Completions)
            {
                if (completion.Origin == 0 && completion.DottedRule.Production.LeftHandSide.Is(Grammar.Start))
                {
                    return completion.ParseNode as IInternalForestNode;
                }
            }

            throw new Exception("Unable to parse expression.");
        }

        public bool IsAccepted()
        {
            var lastSet = Chart.LastSet();

            // PERF: Avoid LINQ Any due to lambda allocation
            foreach (var completion in lastSet.Completions)
            {
                if (completion.Origin == 0 && completion.DottedRule.Production.LeftHandSide.Is(Grammar.Start))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Pulse(IToken token)
        {
            ScanPass(Location, token);

            return Recognize();
        }

        public bool Pulse(IReadOnlyList<IToken> tokens)
        {
            foreach (var token in tokens)
            {
                ScanPass(Location, token);
            }

            return Recognize();
        }

        private bool Recognize()
        {

            var tokenRecognized = Chart.EarleySets.Count > Location + 1;

            if (!tokenRecognized)
            {
                return false;
            }

            Location++;
            ReductionPass(Location);

            NodeSet.Clear();
            return true;
        }

        private DottedRuleRegistry DottedRules => Grammar.DottedRules;

        private ParseEngineOptions Options { get; }

        private StateFactory StateFactory { get; }

        private static string GetOriginStateOperationString(string operation, int origin, State state)
        {
            return $"{origin.ToString().PadRight(9)}{state.ToString().PadRight(100)}{operation}";
        }

        private void Complete(NormalState completed, int location)
        {
            if (completed.ParseNode == null)
            {
                completed.ParseNode = CreateNullParseNode(completed.DottedRule.Production.LeftHandSide, location);
            }

            var earleySet = Chart.EarleySets[completed.Origin];
            var searchSymbol = completed.DottedRule.Production.LeftHandSide;

            if (Options.OptimizeRightRecursion)
            {
                OptimizeReductionPath(searchSymbol, completed.Origin);
            }

            if (earleySet.FindTransitionState(searchSymbol, out var transitionState))
            {
                LeoComplete(transitionState, completed, location);
            }
            else
            {
                EarleyComplete(completed, location);
            }
        }

        private IForestNode CreateNullParseNode(Symbol symbol, int location)
        {
            var symbolNode = NodeSet.AddOrGetExistingSymbolNode(symbol, location, location);
            var token = new Token(location, string.Empty, emptyTokenType);
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
            if (nextDottedRule.Dot > 1)
            {
                var preDotPrecursorSymbol = nextDottedRule.Production[nextDottedRule.Dot - 2];
                anyPreDotRuleNull = IsSymbolTransitiveNullable(preDotPrecursorSymbol);
            }

            var anyPostDotRuleNull = IsSymbolTransitiveNullable(nextDottedRule.PostDotSymbol);
            if (anyPreDotRuleNull && !anyPostDotRuleNull)
            {
                return v;
            }

            IInternalForestNode internalNode;
            if (anyPostDotRuleNull)
            {
                internalNode = NodeSet.AddOrGetExistingSymbolNode(nextDottedRule.Production.LeftHandSide, origin, location);
            }
            else
            {
                internalNode = NodeSet.AddOrGetExistingIntermediateNode(nextDottedRule, origin, location);
            }

            // if w = null and y doesn't have a family of children (v)
            if (w == null)
            {
                internalNode.AddUniqueFamily(v);
            }

            // if w != null and y doesn't have a family of children (w, v)            
            else
            {
                internalNode.AddUniqueFamily(v, w);
            }

            return internalNode;
        }

        private VirtualForestNode CreateVirtualParseNode(State completed, int location, TransitionState rootTransitionState)
        {
            if (!NodeSet.TryGetExistingVirtualNode(
                    location,
                    rootTransitionState,
                    out var virtualParseNode))
            {
                virtualParseNode = new VirtualForestNode(rootTransitionState, location, completed.ParseNode);
                NodeSet.AddNewVirtualNode(virtualParseNode);
            }
            else
            {
                virtualParseNode.AddUniquePath(
                    new VirtualForestNodePath(rootTransitionState, completed.ParseNode));
            }

            return virtualParseNode;
        }

        private void EarleyComplete(NormalState completed, int location)
        {
            var sourceEarleySet = Chart.EarleySets[completed.Origin];

            // Predictions may grow
            for (var p = 0; p < sourceEarleySet.Predictions.Count; p++)
            {
                var prediction = sourceEarleySet.Predictions[p];
                if (!prediction.IsSource(completed.DottedRule.Production.LeftHandSide))
                {
                    continue;
                }

                var dottedRule = DottedRules.GetNext(prediction.DottedRule);
                var origin = prediction.Origin;

                // this will not create a node if the state already exists
                var parseNode = CreateParseNode(
                    dottedRule,
                    origin,
                    prediction.ParseNode,
                    completed.ParseNode,
                    location);

                if (Chart.Contains(location, dottedRule, origin))
                {
                    continue;
                }

                var nextState = StateFactory.NewState(dottedRule, origin, parseNode);

                if (Chart.Enqueue(location, nextState))
                {
                    Log(completeLogName, location, nextState);
                }
            }
        }

        private void Initialize()
        {
            Location = 0;
            Chart = new Chart();
            this.expectedLexerRuleCache = new Dictionary<Indices, Lexer[]>();

            foreach (var startProduction in Grammar.StartProductions())
            {
                var startState = StateFactory.NewState(startProduction, 0, 0);

                if (Chart.Enqueue(0, startState))
                {
                    Log(startLogName, 0, startState);
                }
            }

            ReductionPass(Location);
        }

        /// <summary>
        ///     Implements a check for leo quasi complete items
        /// </summary>
        /// <param name="state">the state to check for quasi completeness</param>
        /// <returns>true if quasi complete, false otherwise</returns>
        private bool IsNextStateQuasiComplete(State state)
        {
            var production = state.DottedRule.Production;

            var symbolCount = production.Count;
            if (symbolCount == 0)
            {
                return true;
            }

            var nextDot = state.DottedRule.Dot + 1;
            var isComplete = nextDot == production.Count;
            if (isComplete)
            {
                return true;
            }

            // if all subsequent symbols are nullable
            for (var i = nextDot; i < production.Count; i++)
            {
                var nextSymbol = production[nextDot];
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
                if (Grammar.Start.Is(production.LeftHandSide) &&
                    Grammar.Start.Is(nextSymbol))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsSymbolNullable(Symbol symbol)
        {
            return symbol == null || symbol is NonTerminal nonTerminal && Grammar.IsNullable(nonTerminal);
        }

        private bool IsSymbolTransitiveNullable(Symbol symbol)
        {
            return symbol == null || symbol is NonTerminal nonTerminal && Grammar.IsTransitiveNullable(nonTerminal);
        }

        private void LeoComplete(TransitionState transitionState, State completed, int k)
        {
            var earleySet = Chart.EarleySets[transitionState.Index];
            if (!earleySet.FindTransitionState(transitionState.DottedRule.PreDotSymbol, out var rootTransitionState))
            {
                rootTransitionState = transitionState;
            }

            var virtualParseNode = CreateVirtualParseNode(completed, k, rootTransitionState);

            var dottedRule = transitionState.DottedRule;
            var topmostItem = StateFactory.NewState(dottedRule, transitionState.Origin, virtualParseNode);

            if (Chart.Enqueue(k, topmostItem))
            {
                Log(completeLogName, k, topmostItem);
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

        private void OptimizeReductionPath(Symbol searchSymbol, int origin)
        {
            State t_rule = null;
            TransitionState previousTransitionState = null;

            var visited = ObjectPoolExtensions.Allocate(SharedPools.Default<HashSet<State>>());
            OptimizeReductionPathRecursive(searchSymbol, origin, ref t_rule, ref previousTransitionState, visited);
            SharedPools.Default<HashSet<State>>().ClearAndFree(visited);
        }

        private void OptimizeReductionPathRecursive(
            Symbol searchSymbol,
            int origin,
            ref State t_rule,
            ref TransitionState previousTransitionState,
            HashSet<State> visited)
        {
            var earleySet = Chart.EarleySets[origin];

            // if Ii contains a transitive item of the for [B -> b., A, k]
            if (earleySet.FindTransitionState(searchSymbol, out var transitionState))
            {
                // then t_rule := B-> b.; t_pos = k;
                previousTransitionState = transitionState;
                t_rule = transitionState;
                return;
            }

            // else if Ii contains exactly one item of the form [B -> a.Ab, k]
            if (!earleySet.FindUniqueSourceState(searchSymbol, out var sourceState) ||
                !visited.Add(sourceState) ||
                // and [B-> aA.b, k] is quasi complete (if b nullable)
                !IsNextStateQuasiComplete(sourceState))
            {
                return;
            }

            // then t_rule := [B->aAb.]; t_pos=k;
            t_rule = StateFactory.NextState(sourceState);

            if (sourceState.Origin != origin)
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

            TransitionState currentTransitionState;
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
                    origin);
            }

            if (Chart.Enqueue(origin, currentTransitionState))
            {
                Log(transitionLogName, origin, currentTransitionState);
            }

            previousTransitionState = currentTransitionState;
        }

        private void Predict(NormalState evidence, int location)
        {
            var dottedRule = evidence.DottedRule;
            var nonTerminal = dottedRule.PostDotSymbol as NonTerminal;
            var rulesForNonTerminal = Grammar.ProductionsFor(nonTerminal);

            foreach (var production in rulesForNonTerminal)
            {
                PredictProduction(location, production);
            }

            var isNullable = Grammar.IsTransitiveNullable(nonTerminal);
            if (isNullable)
            {
                PredictAycockHorspool(evidence, location);
            }
        }

        private void PredictAycockHorspool(NormalState evidence, int location)
        {
            var nullParseNode = CreateNullParseNode(evidence.DottedRule.PostDotSymbol, location);
            var dottedRule = DottedRules.GetNext(evidence.DottedRule);

            //var evidenceParseNode = evidence.ParseNode as IInternalForestNode;
            IForestNode parseNode;
            if (evidence.ParseNode is IInternalForestNode evidenceParseNode &&
                evidenceParseNode.Children.Count > 0
                && evidenceParseNode.Children[0].Children.Count > 0)
            {
                parseNode = CreateParseNode(
                    dottedRule,
                    evidence.Origin,
                    evidenceParseNode,
                    nullParseNode,
                    location);
            }
            else
            {
                parseNode = CreateParseNode(
                    dottedRule,
                    evidence.Origin,
                    null,
                    nullParseNode,
                    location);
            }

            if (Chart.Contains(location, dottedRule, evidence.Origin))
            {
                return;
            }

            var aycockHorspoolState = StateFactory.NewState(dottedRule, evidence.Origin, parseNode);
            if (Chart.Enqueue(location, aycockHorspoolState))
            {
                Log(predictionLogName, location, aycockHorspoolState);
            }
        }

        private void PredictProduction(int location, Production production)
        {
            var dottedRule = DottedRules.Get(production, 0);
            if (Chart.Contains(location, dottedRule, 0))
            {
                return;
            }

            // TODO: Pre-Compute Leo Items. If item is 1 step from being complete, add a transition item
            var predictedState = StateFactory.NewState(dottedRule, location);
            if (Chart.Enqueue(location, predictedState))
            {
                Log(predictionLogName, location, predictedState);
            }
        }

        private void ReductionPass(int position)
        {
            var earleySet = Chart.EarleySets[position];
            var resume = true;

            var p = 0;
            var c = 0;

            while (resume)
            {
                // is there a new completion?
                if (c < earleySet.Completions.Count)
                {
                    var completion = earleySet.Completions[c];
                    Complete(completion, position);
                    c++;
                }
                // is there a new prediction?
                else if (p < earleySet.Predictions.Count)
                {
                    var predictions = earleySet.Predictions;

                    var evidence = predictions[p];
                    Predict(evidence, position);

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

            if (currentSymbol is Lexer lexer && token.TokenType.Equals(lexer.TokenType))
            {
                var dottedRule = DottedRules.GetNext(scan.DottedRule);
                if (Chart.Contains(j + 1, dottedRule, i))
                {
                    return;
                }

                var tokenNode = NodeSet.AddOrGetExistingTokenNode(token);
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
            foreach (var scanState in earleySet.Scans)
            {
                Scan(scanState, location, token);
            }
        }

        private static readonly Lexer[] emptyLexerRules = { };

        private static readonly TokenType emptyTokenType = new TokenType(string.Empty);

        private Dictionary<Indices, Lexer[]> expectedLexerRuleCache;

        private ForestNodeSet NodeSet { get; }

        private const string predictionLogName = "Predict";
        private const string startLogName = "StartState";
        private const string completeLogName = "Complete";
        private const string transitionLogName = "Transition";

        private class Indices
        {
            private readonly SortedSet<int> indices;
            private readonly int hashCode;

            public Indices(IEnumerable<int> indices)
            {
                this.indices = new SortedSet<int>(indices);
                this.hashCode = 0;
                foreach (var index in this.indices)
                {
                    this.hashCode = HashCode.ComputeIncrementalHash(index, this.hashCode, this.hashCode == 0);
                }
            }

            public Lexer[] GetLexerRules(IReadOnlyList<Lexer> rules)
            {
                return this.indices.Select(index => rules[index]).ToArray();
            }

            public override bool Equals(object obj)
            {
                // ReSharper disable once PossibleNullReferenceException
                return this.indices.SequenceEqual(((Indices) obj).indices);
            }

            public override int GetHashCode()
            {
                return this.hashCode;
            }
        }
    }
}