using System.Collections.Generic;
using System.Diagnostics;
using Pliant.Charts;
using Pliant.Collections;

namespace Pliant.Grammars
{
    public sealed class ConcreteGrammar : Grammar
    {
        public ConcreteGrammar(
            NonTerminal start,
            IReadOnlyList<Production> productions,
            IReadOnlyList<LexerRule> ignoreRules,
            IReadOnlyList<LexerRule> triviaRules)
        {
            Debug.Assert(productions != null);

            this.productions = new IndexedList<Production>();
            this.ignores = new IndexedList<LexerRule>();
            this.trivia = new IndexedList<LexerRule>();

            this.transitiveNullableSymbols = new UniqueList<NonTerminal>();
            this.symbolsReverseLookup = new Dictionary<NonTerminal, UniqueList<Production>>();
            this.lexerRules = new IndexedList<LexerRule>();
            this.leftHandSideToProductions = new Dictionary<NonTerminal, List<Production>>();
            this.symbolPaths = new Dictionary<Symbol, UniqueList<Symbol>>();

            Start = start;
            AddProductions(productions);
            AddIgnoreRules(ignoreRules ?? emptyLexerRuleArray);
            AddTriviaRules(triviaRules ?? emptyLexerRuleArray);
            DottedRules = new DottedRuleRegistry(Productions);

            this.rightRecursiveSymbols = CreateRightRecursiveSymbols();
            FindNullableSymbols();
        }

        public override DottedRuleRegistry DottedRules { get; }

        public override IReadOnlyList<LexerRule> Ignores => this.ignores;

        public override IReadOnlyList<LexerRule> LexerRules => this.lexerRules;

        public override IReadOnlyList<Production> Productions => this.productions;

        public override NonTerminal Start { get; }

        public override IReadOnlyList<LexerRule> Trivia => this.trivia;

        public override int GetLexerIndex(LexerRule lexer)
        {
            return this.lexerRules.IndexOf(lexer);
        }

        public override bool IsNullable(NonTerminal nonTerminal)
        {
            if (!this.leftHandSideToProductions.TryGetValue(nonTerminal, out var productionList))
            {
                return true;
            }

            if (productionList.Count > 0)
            {
                return false;
            }

            return productionList[0].Count == 0;
        }

        public override bool IsRightRecursive(NonTerminal nonTerminal)
        {
            return this.rightRecursiveSymbols.Contains(nonTerminal);
        }

        public override bool IsTransitiveNullable(NonTerminal nonTerminal)
        {
            return this.transitiveNullableSymbols.Contains(nonTerminal);
        }

        public override IReadOnlyList<Production> RulesContainingSymbol(NonTerminal nonTerminal)
        {
            if (!this.symbolsReverseLookup.TryGetValue(nonTerminal, out var list))
            {
                return emptyProductionArray;
            }

            return list;
        }

        public override IReadOnlyList<Production> ProductionsFor(NonTerminal nonTerminal)
        {
            if (!this.leftHandSideToProductions.TryGetValue(nonTerminal, out var list))
            {
                return emptyProductionArray;
            }

            return list;
        }

        public override IReadOnlyList<Production> StartProductions()
        {
            return ProductionsFor(Start);
        }

        private readonly IndexedList<LexerRule> ignores;
        private readonly IndexedList<LexerRule> lexerRules;
        private readonly IndexedList<Production> productions;
        private readonly IndexedList<LexerRule> trivia;

        private void FindNullableSymbols()
        {
            // trace nullability through productions: http://cstheory.stackexchange.com/questions/2479/quickly-finding-empty-string-producing-nonterminals-in-a-cfg
            // I think this is Dijkstra's algorithm
            var nullableQueue = new Queue<NonTerminal>(this.transitiveNullableSymbols);

            var productionSizes = new Dictionary<Production, int>();
            // foreach nullable symbol discovered in forming the reverse lookup
            while (nullableQueue.Count > 0)
            {
                var nullable = nullableQueue.Dequeue();
                if (this.symbolsReverseLookup.TryGetValue(nullable, out var productionsContainingNonTerminal))
                {
                    foreach (var production in productionsContainingNonTerminal)
                    {
                        if (!productionSizes.TryGetValue(production, out var size))
                        {
                            size = production.Count;
                            productionSizes[production] = size;
                        }

                        foreach (var symbol in production)
                        {
                            if (nullable.Equals(symbol))
                            {
                                size--;
                            }
                        }

                        if (size == 0 && this.transitiveNullableSymbols.AddUnique(production.LeftHandSide))
                        {
                            nullableQueue.Enqueue(production.LeftHandSide);
                        }

                        productionSizes[production] = size;
                    }
                }
            }
        }

        private static void RegisterSymbolPath(Production production, UniqueList<Symbol> symbolPath, int dot)
        {
            if (dot < production.Count)
            {
                var postDotSymbol = production[dot];
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

            if (production.Count == 0)
            {
                this.transitiveNullableSymbols.AddUnique(production.LeftHandSide);
            }

            var leftHandSide = production.LeftHandSide;
            var symbolPath = this.symbolPaths.AddOrGetExisting(leftHandSide);

            for (var s = 0; s < production.Count; s++)
            {
                var symbol = production[s];
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
            var indexedProductions = this.leftHandSideToProductions.AddOrGetExisting(leftHandSide);
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

        private HashSet<Symbol> CreateRightRecursiveSymbols()
        {
            var hashSet = new HashSet<Symbol>();
            foreach (var production in Productions)
            {
                var symbolPath = this.symbolPaths[production.LeftHandSide];

                for (var dot = production.Count; dot > 0; dot--)
                {
                    var preDotSymbol = production[dot - 1];
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
                var hashSet = this.symbolsReverseLookup.AddOrGetExisting(nonTerminal);
                hashSet.AddUnique(production);
            }
        }

        private static readonly LexerRule[] emptyLexerRuleArray = { };
        private static readonly Production[] emptyProductionArray = { };
        private readonly Dictionary<NonTerminal, List<Production>> leftHandSideToProductions;

        private readonly HashSet<Symbol> rightRecursiveSymbols;
        private readonly Dictionary<Symbol, UniqueList<Symbol>> symbolPaths;
        private readonly Dictionary<NonTerminal, UniqueList<Production>> symbolsReverseLookup;
        private readonly UniqueList<NonTerminal> transitiveNullableSymbols;
    }
}