using System.Collections.Generic;
using Pliant.Charts;

namespace Pliant.Grammars
{
    public abstract class Grammar
    {
        public abstract IReadOnlyList<Production> Productions { get; }
        public abstract NonTerminal Start { get; }
        public abstract IReadOnlyList<LexerRule> Ignores { get; }
        public abstract IReadOnlyList<LexerRule> Trivia { get; }
        public abstract DottedRuleRegistry DottedRules { get; }
        public abstract IReadOnlyList<LexerRule> LexerRules { get; }
        public abstract int GetLexerIndex(LexerRule lexer);
        public abstract IReadOnlyList<Production> ProductionsFor(NonTerminal nonTerminal);
        public abstract IReadOnlyList<Production> RulesContainingSymbol(NonTerminal nonTerminal);
        public abstract IReadOnlyList<Production> StartProductions();
        public abstract bool IsNullable(NonTerminal nonTerminal);
        public abstract bool IsTransitiveNullable(NonTerminal nonTerminal);
        public abstract bool IsRightRecursive(NonTerminal nonTerminal);
    }
}