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

            this.productions = new IndexedList<Production>();
            this.ignores = new IndexedList<LexerRule>();
            this.trivia = new IndexedList<LexerRule>();

            this._transitiveNullableSymbols = new UniqueList<NonTerminal>();
            this._symbolsReverseLookup = new Dictionary<NonTerminal, UniqueList<Production>>();
            this.lexerRules = new IndexedList<LexerRule>();
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

        public IReadOnlyList<LexerRule> Ignores => this.ignores;

        public IReadOnlyList<LexerRule> LexerRules => this.lexerRules;

        public IReadOnlyList<Production> Productions => this.productions;

        public NonTerminal Start { get; }

        public IReadOnlyList<LexerRule> Trivia => this.trivia;

        public int GetLexerRuleIndex(LexerRule lexerRule)
        {
            return this.lexerRules.IndexOf(lexerRule);
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

        private readonly IndexedList<LexerRule> ignores;
        private readonly IndexedList<LexerRule> lexerRules;
        private readonly IndexedList<Production> productions;
        private readonly IndexedList<LexerRule> trivia;

        private static void FindNullableSymbols(
            Dictionary<NonTerminal, UniqueList<Production>> reverseLookup,
            UniqueList<NonTerminal> nullables)
        {
            // trace nullability through productions: http://cstheory.stackexchange.com/questions/2479/quickly-finding-empty-string-producing-nonterminals-in-a-cfg
            // I think this is Dijkstra's algorithm
            var nullableQueue = new Queue<NonTerminal>(nullables);

            var productionSizes = new Dictionary<Production, int>();
            // foreach nullable symbol discovered in forming the reverse lookup
            while (nullableQueue.Count > 0)
            {
                var nullable = nullableQueue.Dequeue();
                if (reverseLookup.TryGetValue(nullable, out var productionsContainingNonTerminal))
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
                            if (nullable.Equals(symbol))
                            {
                                size--;
                            }
                        }

                        if (size == 0 && nullables.AddUnique(production.LeftHandSide))
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
                this.ignores.Add(ignoreRule);
                this.lexerRules.Add(ignoreRule);
            }
        }

        private void AddLexerRule(LexerRule lexerRule)
        {
            this.lexerRules.Add(lexerRule);
        }

        private void AddProduction(Production production)
        {
            this.productions.Add(production);
            AddProductionToLeftHandSideLookup(production);

            if (production.IsEmpty)
            {
                this._transitiveNullableSymbols.AddUnique(production.LeftHandSide);
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
                this.trivia.Add(triviaRule);
                this.lexerRules.Add(triviaRule);
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
                hashSet.AddUnique(production);
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