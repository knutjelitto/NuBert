using System;
using System.Collections.Generic;
using System.Text;
using Pliant.Grammars;
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
