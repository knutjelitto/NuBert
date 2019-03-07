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

        public static implicit operator TerminalExpression(Terminal terminal)
        {
            return new TerminalExpression(new SymbolExpression(new LexerRuleModel(new TerminalLexer(terminal))));
        }
    }
}