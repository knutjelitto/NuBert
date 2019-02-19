using System.Collections.Generic;

namespace Pliant.Grammars
{
    public interface IGrammar
    {
        IReadOnlyList<Production> Productions { get; }

        NonTerminal Start { get; }

        IReadOnlyList<LexerRule> Ignores { get; }

        IReadOnlyList<LexerRule> Trivia { get; }

        DottedRuleRegistry DottedRules { get; }

        IReadOnlyList<LexerRule> LexerRules { get; }

        int GetLexerRuleIndex(LexerRule lexerRule);

        IReadOnlyList<Production> RulesFor(NonTerminal nonTerminal);

        IReadOnlyList<Production> RulesContainingSymbol(NonTerminal nonTerminal);

        IReadOnlyList<Production> StartProductions();

        bool IsNullable(NonTerminal nonTerminal);

        bool IsTransitiveNullable(NonTerminal nonTerminal);

        bool IsRightRecursive(Symbol symbol);
    }
}