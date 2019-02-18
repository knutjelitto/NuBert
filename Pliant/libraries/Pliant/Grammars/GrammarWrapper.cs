using System.Collections.Generic;

namespace Pliant.Grammars
{
    public abstract class GrammarWrapper : IGrammar
    {
        private readonly IGrammar _innerGrammar;

        protected GrammarWrapper(IGrammar innerGrammar)
        {
            this._innerGrammar = innerGrammar;
        }

        public IReadOnlyList<IProduction> Productions => this._innerGrammar.Productions;

        public NonTerminal Start => this._innerGrammar.Start;

        public IReadOnlyList<LexerRule> Ignores => this._innerGrammar.Ignores;

        public IReadOnlyList<LexerRule> Trivia => this._innerGrammar.Trivia;

        public IReadOnlyList<LexerRule> LexerRules => this._innerGrammar.LexerRules;

        public IReadOnlyDottedRuleRegistry DottedRules => this._innerGrammar.DottedRules;

        public int GetLexerRuleIndex(LexerRule lexerRule)
        {
            return this._innerGrammar.GetLexerRuleIndex(lexerRule);
        }

        public IReadOnlyList<IProduction> RulesFor(NonTerminal nonTerminal)
        {
            return this._innerGrammar.RulesFor(nonTerminal);
        }

        public IReadOnlyList<IProduction> StartProductions()
        {
            return this._innerGrammar.StartProductions();
        }

        public bool IsNullable(NonTerminal nonTerminal)
        {
            return this._innerGrammar.IsNullable(nonTerminal);
        }

        public bool IsTransativeNullable(NonTerminal nonTerminal)
        {
            return this._innerGrammar.IsTransativeNullable(nonTerminal);
        }

        public IReadOnlyList<IProduction> RulesContainingSymbol(NonTerminal nonTerminal)
        {
            return this._innerGrammar.RulesContainingSymbol(nonTerminal);
        }

        public bool IsRightRecursive(ISymbol symbol)
        {
            return this._innerGrammar.IsRightRecursive(symbol);
        }
    }
}
