using System.Collections.Generic;

namespace Pliant.Grammars
{
    public interface IGrammar
    {
        IReadOnlyList<IProduction> Productions { get; }

        NonTerminal Start { get; }

        IReadOnlyList<LexerRule> Ignores { get; }

        IReadOnlyList<LexerRule> Trivia { get; }

        IReadOnlyDottedRuleRegistry DottedRules { get; }

        IReadOnlyList<LexerRule> LexerRules { get; }

        int GetLexerRuleIndex(LexerRule lexerRule);

        IReadOnlyList<IProduction> RulesFor(NonTerminal nonTerminal);

        IReadOnlyList<IProduction> RulesContainingSymbol(NonTerminal nonTerminal);

        IReadOnlyList<IProduction> StartProductions();

        bool IsNullable(NonTerminal nonTerminal);

        bool IsTransativeNullable(NonTerminal nonTerminal);

        bool IsRightRecursive(ISymbol symbol);
    }
}