using Pliant.Collections;
using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class Grammar : IGrammar
    {
        private static readonly IProduction[] EmptyProductionArray = { };
        private static readonly ILexerRule[] EmptyLexerRuleArray = { };
        private static readonly DottedRule[] EmptyPredictionArray = { };

        protected readonly IndexedList<ILexerRule> _ignores;
        protected readonly IndexedList<ILexerRule> _trivia;
        protected readonly IndexedList<ILexerRule> _lexerRules;
        protected readonly IndexedList<IProduction> _productions;
        
        private readonly HashSet<ISymbol> _rightRecursiveSymbols;
        private readonly Dictionary<INonTerminal, List<IProduction>> _leftHandSideToProductions;
        private readonly UniqueList<INonTerminal> _transativeNullableSymbols;
        private readonly Dictionary<INonTerminal, UniqueList<IProduction>> _symbolsReverseLookup;
        private readonly IDottedRuleRegistry _dottedRuleRegistry;
        private readonly Dictionary<ISymbol, UniqueList<ISymbol>> _symbolPaths;

        public INonTerminal Start { get; private set; }

        public IReadOnlyList<IProduction> Productions => this._productions;

        public IReadOnlyList<ILexerRule> LexerRules => this._lexerRules;

        public IReadOnlyList<ILexerRule> Ignores => this._ignores;

        public IReadOnlyList<ILexerRule> Trivia => this._trivia;

        public IReadOnlyDottedRuleRegistry DottedRules => this._dottedRuleRegistry;

        public Grammar(
            INonTerminal start,
            IReadOnlyList<IProduction> productions,
            IReadOnlyList<ILexerRule> ignoreRules,
            IReadOnlyList<ILexerRule> triviaRules)
        {
            this._productions = new IndexedList<IProduction>();
            this._ignores = new IndexedList<ILexerRule>();
            this._trivia = new IndexedList<ILexerRule>();

            this._transativeNullableSymbols = new UniqueList<INonTerminal>();
            this._symbolsReverseLookup = new Dictionary<INonTerminal, UniqueList<IProduction>>();
            this._lexerRules = new IndexedList<ILexerRule>();
            this._leftHandSideToProductions = new Dictionary<INonTerminal, List<IProduction>>();
            this._dottedRuleRegistry = new DottedRuleRegistry();
            this._symbolPaths = new Dictionary<ISymbol, UniqueList<ISymbol>>();
            
            Start = start;
            AddProductions(productions ?? EmptyProductionArray);
            AddIgnoreRules(ignoreRules ?? EmptyLexerRuleArray);
            AddTriviaRules(triviaRules ?? EmptyLexerRuleArray);

            this._rightRecursiveSymbols = CreateRightRecursiveSymbols(this._dottedRuleRegistry, this._symbolPaths);
            FindNullableSymbols(this._symbolsReverseLookup, this._transativeNullableSymbols);
        }

        public int GetLexerRuleIndex(ILexerRule lexerRule)
        {
            return this._lexerRules.IndexOf(lexerRule);
        }

        private void AddProductions(IReadOnlyList<IProduction> productions)
        {
            for (var p = 0; p < productions.Count; p++)
            {
                var production = productions[p];
                AddProduction(production);
            }
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
                if(symbol.SymbolType == SymbolType.LexerRule)
                {
                    AddLexerRule(symbol as ILexerRule);
                }

                RegisterDottedRule(production, s);
                RegisterSymbolPath(production, symbolPath, s);
                RegisterSymbolInReverseLookup(production, symbol);
            }
            RegisterDottedRule(production, production.RightHandSide.Count);
        }
        
        private void AddProductionToLeftHandSideLookup(IProduction production)
        {
            var leftHandSide = production.LeftHandSide;
            var indexedProductions = this._leftHandSideToProductions.AddOrGetExisting(leftHandSide);
            indexedProductions.Add(production);
        }
        
        private void AddLexerRule(ILexerRule lexerRule)
        {
            this._lexerRules.Add(lexerRule);
        }

        private void RegisterSymbolInReverseLookup(IProduction production, ISymbol symbol)
        {
            if (symbol.SymbolType == SymbolType.NonTerminal)
            {
                var nonTerminal = symbol as INonTerminal;
                var hashSet = this._symbolsReverseLookup.AddOrGetExisting(nonTerminal);
                hashSet.Add(production);
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

        private void RegisterDottedRule(IProduction production, int s)
        {
            var dottedRule = new DottedRule(production, s);
            this._dottedRuleRegistry.Register(dottedRule);
        }

        private void AddIgnoreRules(IReadOnlyList<ILexerRule> ignoreRules)
        {
            for (var i = 0; i < ignoreRules.Count; i++)
            {
                var ignoreRule = ignoreRules[i];
                this._ignores.Add(ignoreRule);
                this._lexerRules.Add(ignoreRule);
            }
        }

        private void AddTriviaRules(IReadOnlyList<ILexerRule> triviaRules)
        {
            for (var i = 0; i < triviaRules.Count; i++)
            {
                var triviaRule = triviaRules[i];
                this._trivia.Add(triviaRule);
                this._lexerRules.Add(triviaRule);
            }
        }

        private static void FindNullableSymbols(
            Dictionary<INonTerminal, UniqueList<IProduction>> reverseLookup,
            UniqueList<INonTerminal> nullable)
        {
            // trace nullability through productions: http://cstheory.stackexchange.com/questions/2479/quickly-finding-empty-string-producing-nonterminals-in-a-cfg
            // I think this is Dijkstra's algorithm
            var nullableQueue = new Queue<INonTerminal>(nullable);

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
                            if (symbol.SymbolType == SymbolType.NonTerminal
                                && nonTerminal.Equals(symbol))
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
                    if (preDotSymbol.SymbolType != SymbolType.NonTerminal)
                    {
                        break;
                    }

                    var preDotNonTerminal = preDotSymbol as INonTerminal;
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
            }
            return hashSet;
        }
        
        public bool IsTransativeNullable(INonTerminal nonTerminal)
        {
            return this._transativeNullableSymbols.Contains(nonTerminal);
        }

        public bool IsNullable(INonTerminal nonTerminal)
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

        public IReadOnlyList<IProduction> RulesContainingSymbol(INonTerminal nonTerminal)
        {
            if (!this._symbolsReverseLookup.TryGetValue(nonTerminal, out var list))
            {
                return EmptyProductionArray;
            }

            return list;
        }

        public IReadOnlyList<IProduction> RulesFor(INonTerminal nonTerminal)
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

        public bool IsRightRecursive(ISymbol symbol)
        {
            return this._rightRecursiveSymbols.Contains(symbol);
        }
    }
}
