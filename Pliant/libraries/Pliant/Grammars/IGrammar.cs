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
}