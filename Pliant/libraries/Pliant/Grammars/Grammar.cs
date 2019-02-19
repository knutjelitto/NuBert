using System.Collections.Generic;
using System.Diagnostics;
using Pliant.Collections;

namespace Pliant.Grammars
{
    public class Grammar : IGrammar
    {
        public Grammar(
            NonTerminal start,
            IReadOnlyList<Production> productions,
            IReadOnlyList<LexerRule> ignoreRules,
            IReadOnlyList<LexerRule> triviaRules)
        {
            Debug.Assert(productions != null);

            this._productions = new IndexedList<Production>();
            this._ignores = new IndexedList<LexerRule>();
            this._trivia = new IndexedList<LexerRule>();

            this._transitiveNullableSymbols = new UniqueList<NonTerminal>();
            this._symbolsReverseLookup = new Dictionary<NonTerminal, UniqueList<Production>>();
            this._lexerRules = new IndexedList<LexerRule>();
            this._leftHandSideToProductions = new Dictionary<NonTerminal, List<Production>>();
            DottedRules = new DottedRuleRegistry();
            this._symbolPaths = new Dictionary<Symbol, UniqueList<Symbol>>();

            Start = start;
            AddProductions(productions);
            AddIgnoreRules(ignoreRules ?? EmptyLexerRuleArray);
            AddTriviaRules(triviaRules ?? EmptyLexerRuleArray);
            DottedRules.Seed(this);

            this._rightRecursiveSymbols = CreateRightRecursiveSymbols(this._symbolPaths);
            FindNullableSymbols(this._symbolsReverseLookup, this._transitiveNullableSymbols);
        }

        public DottedRuleRegistry DottedRules { get; }

        public IReadOnlyList<LexerRule> Ignores => this._ignores;

        public IReadOnlyList<LexerRule> LexerRules => this._lexerRules;

        public IReadOnlyList<Production> Productions => this._productions;

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

        public bool IsRightRecursive(Symbol symbol)
        {
            return this._rightRecursiveSymbols.Contains(symbol);
        }

        public bool IsTransitiveNullable(NonTerminal nonTerminal)
        {
            return this._transitiveNullableSymbols.Contains(nonTerminal);
        }

        public IReadOnlyList<Production> RulesContainingSymbol(NonTerminal nonTerminal)
        {
            if (!this._symbolsReverseLookup.TryGetValue(nonTerminal, out var list))
            {
                return EmptyProductionArray;
            }

            return list;
        }

        public IReadOnlyList<Production> RulesFor(NonTerminal nonTerminal)
        {
            if (!this._leftHandSideToProductions.TryGetValue(nonTerminal, out var list))
            {
                return EmptyProductionArray;
            }

            return list;
        }

        public IReadOnlyList<Production> StartProductions()
        {
            return RulesFor(Start);
        }

        protected readonly IndexedList<LexerRule> _ignores;
        protected readonly IndexedList<LexerRule> _lexerRules;
        protected readonly IndexedList<Production> _productions;
        protected readonly IndexedList<LexerRule> _trivia;

        private static void FindNullableSymbols(
            Dictionary<NonTerminal, UniqueList<Production>> reverseLookup,
            UniqueList<NonTerminal> nullable)
        {
            // trace nullability through productions: http://cstheory.stackexchange.com/questions/2479/quickly-finding-empty-string-producing-nonterminals-in-a-cfg
            // I think this is Dijkstra's algorithm
            var nullableQueue = new Queue<NonTerminal>(nullable);

            var productionSizes = new Dictionary<Production, int>();
            // foreach nullable symbol discovered in forming the reverse lookup
            while (nullableQueue.Count > 0)
            {
                var nonTerminal = nullableQueue.Dequeue();
                if (reverseLookup.TryGetValue(nonTerminal, out var productionsContainingNonTerminal))
                {
                    foreach (var production in productionsContainingNonTerminal)
                    {
                        if (!productionSizes.TryGetValue(production, out var size))
                        {
                            size = production.RightHandSide.Count;
                            productionSizes[production] = size;
                        }

                        foreach (var symbol in production.RightHandSide)
                        {
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

        private static void RegisterSymbolPath(Production production, UniqueList<Symbol> symbolPath, int s)
        {
            if (s < production.RightHandSide.Count)
            {
                var postDotSymbol = production.RightHandSide[s];
                symbolPath.AddUnique(postDotSymbol);
            }
        }

        private void AddIgnoreRules(IEnumerable<LexerRule> ignoreRules)
        {
            foreach (var ignoreRule in ignoreRules)
            {
                this._ignores.Add(ignoreRule);
                this._lexerRules.Add(ignoreRule);
            }
        }

        private void AddLexerRule(LexerRule lexerRule)
        {
            this._lexerRules.Add(lexerRule);
        }

        private void AddProduction(Production production)
        {
            this._productions.Add(production);
            AddProductionToLeftHandSideLookup(production);

            if (production.IsEmpty)
            {
                this._transitiveNullableSymbols.Add(production.LeftHandSide);
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

                RegisterSymbolPath(production, symbolPath, s);
                RegisterSymbolInReverseLookup(production, symbol);
            }
        }

        private void AddProductions(IEnumerable<Production> productions)
        {
            foreach (var production in productions)
            {
                AddProduction(production);
            }
        }

        private void AddProductionToLeftHandSideLookup(Production production)
        {
            var leftHandSide = production.LeftHandSide;
            var indexedProductions = this._leftHandSideToProductions.AddOrGetExisting(leftHandSide);
            indexedProductions.Add(production);
        }

        private void AddTriviaRules(IEnumerable<LexerRule> triviaRules)
        {
            foreach (var triviaRule in triviaRules)
            {
                this._trivia.Add(triviaRule);
                this._lexerRules.Add(triviaRule);
            }
        }

        private HashSet<Symbol> CreateRightRecursiveSymbols(Dictionary<Symbol, UniqueList<Symbol>> symbolPaths)
        {
            var hashSet = new HashSet<Symbol>();
            foreach (var production in Productions)
            {
                var position = production.RightHandSide.Count;
                var symbolPath = symbolPaths[production.LeftHandSide];

                for (var s = position; s > 0; s--)
                {
                    var preDotSymbol = production.RightHandSide[s - 1];
                    if (preDotSymbol is NonTerminal preDotNonTerminal)
                    {
                        if (symbolPath.Contains(preDotNonTerminal))
                        {
                            hashSet.Add(production.LeftHandSide);
                            break;
                        }

                        if (!IsTransitiveNullable(preDotNonTerminal))
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

        private void RegisterSymbolInReverseLookup(Production production, Symbol symbol)
        {
            if (symbol is NonTerminal nonTerminal)
            {
                var hashSet = this._symbolsReverseLookup.AddOrGetExisting(nonTerminal);
                hashSet.Add(production);
            }
        }

        private static readonly LexerRule[] EmptyLexerRuleArray = { };
        private static readonly Production[] EmptyProductionArray = { };
        private readonly Dictionary<NonTerminal, List<Production>> _leftHandSideToProductions;

        private readonly HashSet<Symbol> _rightRecursiveSymbols;
        private readonly Dictionary<Symbol, UniqueList<Symbol>> _symbolPaths;
        private readonly Dictionary<NonTerminal, UniqueList<Production>> _symbolsReverseLookup;
        private readonly UniqueList<NonTerminal> _transitiveNullableSymbols;
    }
}