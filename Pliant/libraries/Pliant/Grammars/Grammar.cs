using System.Collections.Generic;
using Pliant.Collections;

namespace Pliant.Grammars
{
    public class Grammar : IGrammar
    {
        public Grammar(
            NonTerminal start,
            IReadOnlyList<IProduction> productions,
            IReadOnlyList<LexerRule> ignoreRules,
            IReadOnlyList<LexerRule> triviaRules)
        {
            this._productions = new IndexedList<IProduction>();
            this._ignores = new IndexedList<LexerRule>();
            this._trivia = new IndexedList<LexerRule>();

            this._transativeNullableSymbols = new UniqueList<NonTerminal>();
            this._symbolsReverseLookup = new Dictionary<NonTerminal, UniqueList<IProduction>>();
            this._lexerRules = new IndexedList<LexerRule>();
            this._leftHandSideToProductions = new Dictionary<NonTerminal, List<IProduction>>();
            this._dottedRuleRegistry = new DottedRuleRegistry();
            this._symbolPaths = new Dictionary<ISymbol, UniqueList<ISymbol>>();

            Start = start;
            AddProductions(productions ?? EmptyProductionArray);
            AddIgnoreRules(ignoreRules ?? EmptyLexerRuleArray);
            AddTriviaRules(triviaRules ?? EmptyLexerRuleArray);

            this._rightRecursiveSymbols = CreateRightRecursiveSymbols(this._dottedRuleRegistry, this._symbolPaths);
            FindNullableSymbols(this._symbolsReverseLookup, this._transativeNullableSymbols);
        }

        public IReadOnlyDottedRuleRegistry DottedRules => this._dottedRuleRegistry;

        public IReadOnlyList<LexerRule> Ignores => this._ignores;

        public IReadOnlyList<LexerRule> LexerRules => this._lexerRules;

        public IReadOnlyList<IProduction> Productions => this._productions;

        public NonTerminal Start { get; }

        public IReadOnlyList<LexerRule> Trivia => this._trivia;

        public int GetLexerRuleIndex(LexerRule lexerRule)
        {
            return this._lexerRules.IndexOf(lexerRule);
        }

        public bool IsNullable(NonTerminal nonTerminal)
        {
            if (!this._leftHandSideToProductions.TryGetValue(nonTerminal, out var productionList))
            {
                return true;
            }

            if (productionList.Count > 0)
            {
                return false;
            }

            return productionList[0].RightHandSide.Count == 0;
        }

        public bool IsRightRecursive(ISymbol symbol)
        {
            return this._rightRecursiveSymbols.Contains(symbol);
        }

        public bool IsTransativeNullable(NonTerminal nonTerminal)
        {
            return this._transativeNullableSymbols.Contains(nonTerminal);
        }

        public IReadOnlyList<IProduction> RulesContainingSymbol(NonTerminal nonTerminal)
        {
            if (!this._symbolsReverseLookup.TryGetValue(nonTerminal, out var list))
            {
                return EmptyProductionArray;
            }

            return list;
        }

        public IReadOnlyList<IProduction> RulesFor(NonTerminal nonTerminal)
        {
            if (!this._leftHandSideToProductions.TryGetValue(nonTerminal, out var list))
            {
                return EmptyProductionArray;
            }

            return list;
        }

        public IReadOnlyList<IProduction> StartProductions()
        {
            return RulesFor(Start);
        }

        protected readonly IndexedList<LexerRule> _ignores;
        protected readonly IndexedList<LexerRule> _lexerRules;
        protected readonly IndexedList<IProduction> _productions;
        protected readonly IndexedList<LexerRule> _trivia;

        private static void FindNullableSymbols(
            Dictionary<NonTerminal, UniqueList<IProduction>> reverseLookup,
            UniqueList<NonTerminal> nullable)
        {
            // trace nullability through productions: http://cstheory.stackexchange.com/questions/2479/quickly-finding-empty-string-producing-nonterminals-in-a-cfg
            // I think this is Dijkstra's algorithm
            var nullableQueue = new Queue<NonTerminal>(nullable);

            var productionSizes = new Dictionary<IProduction, int>();
            // foreach nullable symbol discovered in forming the reverse lookup
            while (nullableQueue.Count > 0)
            {
                var nonTerminal = nullableQueue.Dequeue();
                if (reverseLookup.TryGetValue(nonTerminal, out var productionsContainingNonTerminal))
                {
                    for (var p = 0; p < productionsContainingNonTerminal.Count; p++)
                    {
                        var production = productionsContainingNonTerminal[p];
                        if (!productionSizes.TryGetValue(production, out var size))
                        {
                            size = production.RightHandSide.Count;
                            productionSizes[production] = size;
                        }

                        for (var s = 0; s < production.RightHandSide.Count; s++)
                        {
                            var symbol = production.RightHandSide[s];
                            if (symbol is NonTerminal && nonTerminal.Equals(symbol))
                            {
                                size--;
                            }
                        }

                        if (size == 0 && nullable.AddUnique(production.LeftHandSide))
                        {
                            nullableQueue.Enqueue(production.LeftHandSide);
                        }

                        productionSizes[production] = size;
                    }
                }
            }
        }

        private static void RegisterSymbolPath(IProduction production, UniqueList<ISymbol> symbolPath, int s)
        {
            if (s < production.RightHandSide.Count)
            {
                var postDotSymbol = production.RightHandSide[s];
                symbolPath.AddUnique(postDotSymbol);
            }
        }

        private void AddIgnoreRules(IReadOnlyList<LexerRule> ignoreRules)
        {
            for (var i = 0; i < ignoreRules.Count; i++)
            {
                var ignoreRule = ignoreRules[i];
                this._ignores.Add(ignoreRule);
                this._lexerRules.Add(ignoreRule);
            }
        }

        private void AddLexerRule(LexerRule lexerRule)
        {
            this._lexerRules.Add(lexerRule);
        }

        private void AddProduction(IProduction production)
        {
            this._productions.Add(production);
            AddProductionToLeftHandSideLookup(production);

            if (production.IsEmpty)
            {
                this._transativeNullableSymbols.Add(production.LeftHandSide);
            }

            var leftHandSide = production.LeftHandSide;
            var symbolPath = this._symbolPaths.AddOrGetExisting(leftHandSide);

            for (var s = 0; s < production.RightHandSide.Count; s++)
            {
                var symbol = production.RightHandSide[s];
                if (symbol is LexerRule lexerRule)
                {
                    AddLexerRule(lexerRule);
                }

                RegisterDottedRule(production, s);
                RegisterSymbolPath(production, symbolPath, s);
                RegisterSymbolInReverseLookup(production, symbol);
            }

            RegisterDottedRule(production, production.RightHandSide.Count);
        }

        private void AddProductions(IReadOnlyList<IProduction> productions)
        {
            for (var p = 0; p < productions.Count; p++)
            {
                var production = productions[p];
                AddProduction(production);
            }
        }

        private void AddProductionToLeftHandSideLookup(IProduction production)
        {
            var leftHandSide = production.LeftHandSide;
            var indexedProductions = this._leftHandSideToProductions.AddOrGetExisting(leftHandSide);
            indexedProductions.Add(production);
        }

        private void AddTriviaRules(IReadOnlyList<LexerRule> triviaRules)
        {
            for (var i = 0; i < triviaRules.Count; i++)
            {
                var triviaRule = triviaRules[i];
                this._trivia.Add(triviaRule);
                this._lexerRules.Add(triviaRule);
            }
        }

        private HashSet<ISymbol> CreateRightRecursiveSymbols(
            IDottedRuleRegistry dottedRuleRegistry,
            Dictionary<ISymbol, UniqueList<ISymbol>> symbolPaths)
        {
            var hashSet = new HashSet<ISymbol>();
            for (var p = 0; p < this._productions.Count; p++)
            {
                var production = this._productions[p];
                var position = production.RightHandSide.Count;
                var completed = dottedRuleRegistry.Get(production, position);
                var symbolPath = symbolPaths[production.LeftHandSide];

                for (var s = position; s > 0; s--)
                {
                    var preDotSymbol = production.RightHandSide[s - 1];
                    if (preDotSymbol is NonTerminal)
                    {
                        var preDotNonTerminal = preDotSymbol as NonTerminal;
                        if (symbolPath.Contains(preDotNonTerminal))
                        {
                            hashSet.Add(production.LeftHandSide);
                            break;
                        }

                        if (!IsTransativeNullable(preDotNonTerminal))
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return hashSet;
        }

        private void RegisterDottedRule(IProduction production, int s)
        {
            var dottedRule = new DottedRule(production, s);
            this._dottedRuleRegistry.Register(dottedRule);
        }

        private void RegisterSymbolInReverseLookup(IProduction production, ISymbol symbol)
        {
            if (symbol is NonTerminal nonTerminal)
            {
                var hashSet = this._symbolsReverseLookup.AddOrGetExisting(nonTerminal);
                hashSet.Add(production);
            }
        }

        private static readonly LexerRule[] EmptyLexerRuleArray = { };
        private static readonly DottedRule[] EmptyPredictionArray = { };
        private static readonly IProduction[] EmptyProductionArray = { };
        private readonly IDottedRuleRegistry _dottedRuleRegistry;
        private readonly Dictionary<NonTerminal, List<IProduction>> _leftHandSideToProductions;

        private readonly HashSet<ISymbol> _rightRecursiveSymbols;
        private readonly Dictionary<ISymbol, UniqueList<ISymbol>> _symbolPaths;
        private readonly Dictionary<NonTerminal, UniqueList<IProduction>> _symbolsReverseLookup;
        private readonly UniqueList<NonTerminal> _transativeNullableSymbols;
    }
}