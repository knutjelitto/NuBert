using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Pliant.Charts;
using Pliant.Diagnostics;
using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Tokens;
using Pliant.Tree;

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

        public IEnumerable<LexerRule> GetExpectedLexerRules()
        {
            return Chart.Current.Scans
                        .Select(state => state.DottedRule.PostDotSymbol)
                        .OfType<LexerRule>();
        }

        public IInternalTreeNode GetParseTree(IForestDisambiguationAlgorithm disambiguate = null)
        {
            return new InternalTreeNode(GetParseForestRootNode(), disambiguate ?? new SelectFirstChildDisambiguationAlgorithm());
        }

        public ISymbolForestNode GetParseForestRootNode()
        {
            // PERF: Avoid Linq expressions due to lambda allocation
            foreach (var completion in Chart.Current.Completions)
            {
                if (completion.Origin == 0 && completion.LeftHandSide.Is(Grammar.Start))
                {
                    Debug.Assert(completion.ParseNode is ISymbolForestNode);
                    return (ISymbolForestNode) completion.ParseNode;
                }
            }

            throw new Exception("Unable to parse expression.");
        }

        public bool IsAccepted()
        {
            // PERF: Avoid LINQ Any due to lambda allocation
            foreach (var completion in Chart.Current.Completions)
            {
                if (completion.Origin == 0 && completion.LeftHandSide.Is(Grammar.Start))
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
            var tokenRecognized = Chart.Count > Location + 1;

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

        private static string GetOriginStateOperationString(string operation, int origin, EarleyItem state)
        {
            return $"{origin.ToString().PadRight(9)}{state.ToString().PadRight(100)}{operation}";
        }

        private void Complete(CompletedState completed, int location)
        {
            if (completed.ParseNode == null)
            {
                completed.SetParseNode(CreateNullParseNode(completed.LeftHandSide, location));
            }

            EarleyComplete(completed, location);
        }

        private IForestNode CreateNullParseNode(NonTerminal symbol, int location)
        {
            var symbolNode = NodeSet.AddOrGetExistingSymbolNode(symbol, location, location);
            var nullNode = new SymbolForestNode(symbol, location, location); //new EpsilonForestNode(location);
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

        private void EarleyComplete(CompletedState completed, int location)
        {
            var earleySet = Chart[completed.Origin];

            // Predictions may grow
            var p = 0;
            for (;p < earleySet.Predictions.Count;++p)
            {
                var prediction = earleySet.Predictions[p];
                if (!prediction.IsSource(completed.LeftHandSide))
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

#if false
        /// <summary>
        ///     Implements a check for leo quasi complete items
        /// </summary>
        /// <param name="state">the state to check for quasi completeness</param>
        /// <returns>true if quasi complete, false otherwise</returns>
        private bool IsNextStateQuasiComplete(PredictionState state)
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
                if (Grammar.Start.Is(production) && Grammar.Start.Is(nextSymbol))
                {
                    return false;
                }
            }

            return true;
        }
#endif

#if false
        private bool IsSymbolNullable(Symbol symbol)
        {
            return symbol == null || symbol is NonTerminal nonTerminal && Grammar.IsNullable(nonTerminal);
        }
#endif

        private bool IsSymbolTransitiveNullable(Symbol symbol)
        {
            return symbol == null || symbol is NonTerminal nonTerminal && Grammar.IsTransitiveNullable(nonTerminal);
        }

#if false
        private void LeoComplete(TransitionState transitionState, CompletedState completed, int location)
        {
            var earleySet = Chart[transitionState.Index];
            if (!earleySet.FindTransitionState((NonTerminal)transitionState.DottedRule.PreDotSymbol, out var rootTransitionState))
            {
                rootTransitionState = transitionState;
            }

            var virtualParseNode = CreateVirtualParseNode(completed, location, rootTransitionState);

            var dottedRule = transitionState.DottedRule;

            var topmostItem = StateFactory.NewState(dottedRule, transitionState.Origin, virtualParseNode);

            if (Chart.Enqueue(location, topmostItem))
            {
                Log(completeLogName, location, topmostItem);
            }
        }
#endif

        private void Log(string operation, int origin, EarleyItem state)
        {
            if (Options.LoggingEnabled)
            {
                Debug.WriteLine(GetOriginStateOperationString(operation, origin, state));
            }
        }

        private void LogScan(int origin, EarleyItem state, IToken token)
        {
            if (Options.LoggingEnabled)
            {
                Debug.WriteLine($"{GetOriginStateOperationString("Scan", origin, state)} \"{token.Value}\"");
            }
        }

        private void Predict(PredictionState evidence, int location)
        {
            var nonTerminal = evidence.DottedRule.PostDotSymbol as NonTerminal;
            Debug.Assert(nonTerminal != null);
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

        private void PredictAycockHorspool(EarleyItem evidence, int location)
        {
            var nullParseNode = CreateNullParseNode(evidence.DottedRule.PostDotSymbol as NonTerminal, location);
            var dottedRule = DottedRules.GetNext(evidence.DottedRule);

            //var evidenceParseNode = evidence.ParseNode as IInternalForestNode;
            IForestNode parseNode;
            if (evidence.ParseNode is IInternalForestNode evidenceParseNode && evidenceParseNode.Children.Count > 0)
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

        private void ReductionPass(int location)
        {
            var earleySet = Chart[location];
            var resume = true;

            var p = 0;
            var c = 0;

            while (resume)
            {
                // is there a new completion?
                if (c < earleySet.Completions.Count)
                {
                    var completion = earleySet.Completions[c++];
                    Complete(completion, location);
                }
                // is there a new prediction?
                else if (p < earleySet.Predictions.Count)
                {
                    var evidence = earleySet.Predictions[p++];
                    Predict(evidence, location);
                }
                else
                {
                    resume = false;
                }
            }
        }

        private void Scan(ScanState scan, int location, IToken token)
        {
            var currentSymbol = scan.DottedRule.PostDotSymbol;

            if (currentSymbol is LexerRule lexerRule && token.TokenName.Equals(lexerRule.TokenName))
            {
                var dottedRule = DottedRules.GetNext(scan.DottedRule);
                if (Chart.Contains(location + 1, dottedRule, scan.Origin))
                {
                    return;
                }

                var tokenNode = NodeSet.AddOrGetExistingTokenNode(token);
                var parseNode = CreateParseNode(
                    dottedRule,
                    scan.Origin,
                    scan.ParseNode,
                    tokenNode,
                    location + 1);
                var nextState = StateFactory.NewState(dottedRule, scan.Origin, parseNode);

                if (Chart.Enqueue(location + 1, nextState))
                {
                    LogScan(location + 1, nextState, token);
                }
            }
        }

        private void ScanPass(int location, IToken token)
        {
            var earleySet = Chart[location];

            foreach (var scanState in earleySet.Scans)
            {
                Scan(scanState, location, token);
            }
        }

        private ForestNodeSet NodeSet { get; }

        private const string predictionLogName = "Predict";
        private const string startLogName = "StartState";
        private const string completeLogName = "Complete";
    }
}