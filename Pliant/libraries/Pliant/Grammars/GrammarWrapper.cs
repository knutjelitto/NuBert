using System.Collections.Generic;
using Pliant.Charts;

namespace Pliant.Grammars
{
    public abstract class GrammarWrapper : Grammar
    {
        protected GrammarWrapper(Grammar innerGrammar)
        {
            this.innerGrammar = innerGrammar;
        }

        public override IReadOnlyList<Production> Productions => this.innerGrammar.Productions;

        public override NonTerminal Start => this.innerGrammar.Start;

        public override IReadOnlyList<LexerRule> Ignores => this.innerGrammar.Ignores;

        public override IReadOnlyList<LexerRule> Trivia => this.innerGrammar.Trivia;

        public override IReadOnlyList<LexerRule> LexerRules => this.innerGrammar.LexerRules;

        public override DottedRuleRegistry DottedRules => this.innerGrammar.DottedRules;

        public override int GetLexerIndex(LexerRule lexer)
        {
            return this.innerGrammar.GetLexerIndex(lexer);
        }

        public override IReadOnlyList<Production> ProductionsFor(NonTerminal nonTerminal)
        {
            return this.innerGrammar.ProductionsFor(nonTerminal);
        }

        public override IReadOnlyList<Production> StartProductions()
        {
            return this.innerGrammar.StartProductions();
        }

        public override bool IsNullable(NonTerminal nonTerminal)
        {
            return this.innerGrammar.IsNullable(nonTerminal);
        }

        public override bool IsTransitiveNullable(NonTerminal nonTerminal)
        {
            return this.innerGrammar.IsTransitiveNullable(nonTerminal);
        }

        public override IReadOnlyList<Production> RulesContainingSymbol(NonTerminal nonTerminal)
        {
            return this.innerGrammar.RulesContainingSymbol(nonTerminal);
        }

        public override bool IsRightRecursive(NonTerminal nonTerminal)
        {
            return this.innerGrammar.IsRightRecursive(nonTerminal);
        }

        private readonly Grammar innerGrammar;
    }
}