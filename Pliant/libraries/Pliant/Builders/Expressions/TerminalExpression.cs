using Pliant.Grammars;
using Pliant.LexerRules;
using Pliant.Terminals;

namespace Pliant.Builders.Expressions
{
    public class TerminalExpression : RuleExpression
    {
        public TerminalExpression(BaseExpression expr)
            : base(expr)
        {
        }

        public static implicit operator TerminalExpression(AtomTerminal terminal)
        {
            return new TerminalExpression(new SymbolExpression(new LexerRuleModel(new TerminalLexerRule(terminal))));
        }
    }
}