using System.Collections.Generic;
using Pliant.Dotted;

namespace Pliant.Grammars
{
    public interface IGrammar
    {
        IReadOnlyList<Production> Productions { get; }

        NonTerminal Start { get; }

        IReadOnlyList<Lexer> Ignores { get; }

        IReadOnlyList<Lexer> Trivia { get; }

        DottedRuleRegistry DottedRules { get; }

        IReadOnlyList<Lexer> LexerRules { get; }

        int GetLexerIndex(Lexer lexer);

        IReadOnlyList<Production> ProductionsFor(NonTerminal nonTerminal);

        IReadOnlyList<Production> RulesContainingSymbol(NonTerminal nonTerminal);

        IReadOnlyList<Production> StartProductions();

        bool IsNullable(NonTerminal nonTerminal);

        bool IsTransitiveNullable(NonTerminal nonTerminal);

        bool IsRightRecursive(NonTerminal nonTerminal);
    }

    public abstract class Grammar : IGrammar
    {
        public abstract IReadOnlyList<Production> Productions { get; }
        public abstract NonTerminal Start { get; }
        public abstract IReadOnlyList<Lexer> Ignores { get; }
        public abstract IReadOnlyList<Lexer> Trivia { get; }
        public abstract DottedRuleRegistry DottedRules { get; }
        public abstract IReadOnlyList<Lexer> LexerRules { get; }
        public abstract int GetLexerIndex(Lexer lexer);
        public abstract IReadOnlyList<Production> ProductionsFor(NonTerminal nonTerminal);
        public abstract IReadOnlyList<Production> RulesContainingSymbol(NonTerminal nonTerminal);
        public abstract IReadOnlyList<Production> StartProductions();
        public abstract bool IsNullable(NonTerminal nonTerminal);
        public abstract bool IsTransitiveNullable(NonTerminal nonTerminal);
        public abstract bool IsRightRecursive(NonTerminal nonTerminal);
    }
}