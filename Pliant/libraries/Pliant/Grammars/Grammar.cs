using System.Collections.Generic;
using System.Diagnostics;
using Pliant.Collections;
using Pliant.Dotted;

namespace Pliant.Grammars
{
    public class Grammar : IGrammar
    {
        public Grammar(
            NonTerminal start,
            IReadOnlyList<Production> productions,
            IReadOnlyList<Lexer> ignoreRules,
            IReadOnlyList<Lexer> triviaRules)
        {
            Debug.Assert(productions != null);

            this.productions = new IndexedList<Production>();
            this.ignores = new IndexedList<Lexer>();
            this.trivia = new IndexedList<Lexer>();

            this._transitiveNullableSymbols = new UniqueList<NonTerminal>();
            this._symbolsReverseLookup = new Dictionary<NonTerminal, UniqueList<Production>>();
            this.lexerRules = new IndexedList<Lexer>();
            this._leftHandSideToProductions = new Dictionary<NonTerminal, List<Production>>();
            this._symbolPaths = new Dictionary<Symbol, UniqueList<Symbol>>();

            Start = start;
            AddProductions(productions);
            AddIgnoreRules(ignoreRules ?? EmptyLexerRuleArray);
            AddTriviaRules(triviaRules ?? EmptyLexerRuleArray);
            DottedRules = new DottedRuleRegistry(Productions);

            this._rightRecursiveSymbols = CreateRightRecursiveSymbols(this._symbolPaths);
            FindNullableSymbols(this._symbolsReverseLookup, this._transitiveNullableSymbols);
        }

        public DottedRuleRegistry DottedRules { get; }

        public IReadOnlyList<Lexer> Ignores => this.ignores;

        public IReadOnlyList<Lexer> LexerRules => this.lexerRules;

        public IReadOnlyList<Production> Productions => this.productions;

        public NonTerminal Start { get; }

        public IReadOnlyList<Lexer> Trivia => this.trivia;

        public int GetLexerRuleIndex(Lexer lexerRule)
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

            return productionList[0].Count == 0;
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

        public IReadOnlyList<Production> ProductionsFor(NonTerminal nonTerminal)
        {
            if (!this._leftHandSideToProductions.TryGetValue(nonTerminal, out var list))
            {
                return EmptyProductionArray;
            }

            return list;
        }

        public IReadOnlyList<Production> StartProductions()
        {
            return ProductionsFor(Start);
        }

        private readonly IndexedList<Lexer> ignores;
        private readonly IndexedList<Lexer> lexerRules;
        private readonly IndexedList<Production> productions;
        private readonly IndexedList<Lexer> trivia;

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

                        if (size == 0 && nullables.AddUnique(production.LeftHandSide))
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

        private void AddIgnoreRules(IEnumerable<Lexer> ignoreRules)
        {
            foreach (var ignoreRule in ignoreRules)
            {
                this.ignores.Add(ignoreRule);
                this.lexerRules.Add(ignoreRule);
            }
        }

        private void AddLexerRule(Lexer lexerRule)
        {
            this.lexerRules.Add(lexerRule);
        }

        private void AddProduction(Production production)
        {
            this.productions.Add(production);
            AddProductionToLeftHandSideLookup(production);

            if (production.Count == 0)
            {
                this._transitiveNullableSymbols.AddUnique(production.LeftHandSide);
            }

            var leftHandSide = production.LeftHandSide;
            var symbolPath = this._symbolPaths.AddOrGetExisting(leftHandSide);

            for (var s = 0; s < production.Count; s++)
            {
                var symbol = production[s];
                if (symbol is Lexer lexerRule)
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

        private void AddTriviaRules(IEnumerable<Lexer> triviaRules)
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
                var symbolPath = symbolPaths[production.LeftHandSide];

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
                var hashSet = this._symbolsReverseLookup.AddOrGetExisting(nonTerminal);
                hashSet.AddUnique(production);
            }
        }

        private static readonly Lexer[] EmptyLexerRuleArray = { };
        private static readonly Production[] EmptyProductionArray = { };
        private readonly Dictionary<NonTerminal, List<Production>> _leftHandSideToProductions;

        private readonly HashSet<Symbol> _rightRecursiveSymbols;
        private readonly Dictionary<Symbol, UniqueList<Symbol>> _symbolPaths;
        private readonly Dictionary<NonTerminal, UniqueList<Production>> _symbolsReverseLookup;
        private readonly UniqueList<NonTerminal> _transitiveNullableSymbols;
    }
}