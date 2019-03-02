using System.Collections.Generic;
using Pliant.Dotted;

namespace Pliant.Grammars
{
    public abstract class GrammarWrapper : IGrammar
    {
        private readonly IGrammar _innerGrammar;

        protected GrammarWrapper(IGrammar innerGrammar)
        {
            this._innerGrammar = innerGrammar;
        }

        public IReadOnlyList<Production> Productions => this._innerGrammar.Productions;

        public NonTerminal Start => this._innerGrammar.Start;

        public IReadOnlyList<Lexer> Ignores => this._innerGrammar.Ignores;

        public IReadOnlyList<Lexer> Trivia => this._innerGrammar.Trivia;

        public IReadOnlyList<Lexer> LexerRules => this._innerGrammar.LexerRules;

        public DottedRuleRegistry DottedRules => this._innerGrammar.DottedRules;

        public int GetLexerRuleIndex(Lexer lexerRule)
        {
            return this._innerGrammar.GetLexerRuleIndex(lexerRule);
        }

        public IReadOnlyList<Production> ProductionsFor(NonTerminal nonTerminal)
        {
            return this._innerGrammar.ProductionsFor(nonTerminal);
        }

        public IReadOnlyList<Production> StartProductions()
        {
            return this._innerGrammar.StartProductions();
        }

        public bool IsNullable(NonTerminal nonTerminal)
        {
            return this._innerGrammar.IsNullable(nonTerminal);
        }

        public bool IsTransitiveNullable(NonTerminal nonTerminal)
        {
            return this._innerGrammar.IsTransitiveNullable(nonTerminal);
        }

        public IReadOnlyList<Production> RulesContainingSymbol(NonTerminal nonTerminal)
        {
            return this._innerGrammar.RulesContainingSymbol(nonTerminal);
        }

        public bool IsRightRecursive(Symbol symbol)
        {
            return this._innerGrammar.IsRightRecursive(symbol);
        }
    }
}
