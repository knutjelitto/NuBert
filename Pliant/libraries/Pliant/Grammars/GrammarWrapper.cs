using System.Collections.Generic;
using Pliant.Dotted;

namespace Pliant.Grammars
{
    public abstract class GrammarWrapper : IGrammar
    {
        protected GrammarWrapper(IGrammar innerGrammar)
        {
            this.innerGrammar = innerGrammar;
        }

        public IReadOnlyList<Production> Productions => this.innerGrammar.Productions;

        public NonTerminal Start => this.innerGrammar.Start;

        public IReadOnlyList<Lexer> Ignores => this.innerGrammar.Ignores;

        public IReadOnlyList<Lexer> Trivia => this.innerGrammar.Trivia;

        public IReadOnlyList<Lexer> LexerRules => this.innerGrammar.LexerRules;

        public DottedRuleRegistry DottedRules => this.innerGrammar.DottedRules;

        public int GetLexerIndex(Lexer lexer)
        {
            return this.innerGrammar.GetLexerIndex(lexer);
        }

        public IReadOnlyList<Production> ProductionsFor(NonTerminal nonTerminal)
        {
            return this.innerGrammar.ProductionsFor(nonTerminal);
        }

        public IReadOnlyList<Production> StartProductions()
        {
            return this.innerGrammar.StartProductions();
        }

        public bool IsNullable(NonTerminal nonTerminal)
        {
            return this.innerGrammar.IsNullable(nonTerminal);
        }

        public bool IsTransitiveNullable(NonTerminal nonTerminal)
        {
            return this.innerGrammar.IsTransitiveNullable(nonTerminal);
        }

        public IReadOnlyList<Production> RulesContainingSymbol(NonTerminal nonTerminal)
        {
            return this.innerGrammar.RulesContainingSymbol(nonTerminal);
        }

        public bool IsRightRecursive(NonTerminal nonTerminal)
        {
            return this.innerGrammar.IsRightRecursive(nonTerminal);
        }

        private readonly IGrammar innerGrammar;
    }
}