using Pliant.Grammars;
using Pliant.LexerRules;
using Pliant.Terminals;

namespace Pliant.Builders.Expressions
{
    public class Expr : RuleExpression
    {
        private Expr(BaseExpression baseExpression) 
            : base(baseExpression)
        {
        }

        public static Expr Epsilon => null;

        public static explicit operator Expr(string value)
        {
            return new Expr(new SymbolExpression(new LexerRuleModel(new StringLiteralLexer(value))));
        }

        public static explicit operator Expr(char value)
        {

            return new Expr(new SymbolExpression(new LexerRuleModel(new TerminalLexer(value))));
        }

        public static explicit operator Expr(Lexer value)
        {
            return new Expr(new SymbolExpression(new LexerRuleModel(value)));
        }

        public static explicit operator Expr(Terminal value)
        {
            return new Expr(new SymbolExpression(new LexerRuleModel(new TerminalLexer(value))));
        }

        public static explicit operator Expr(GrammarReferenceExpression grammarReference)
        {
            return new Expr(grammarReference);
        }


        public static explicit operator Expr(ProductionExpression productionExpression)
        {
            return new Expr(productionExpression);
        }
    }
}
