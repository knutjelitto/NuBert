using Pliant.Grammars;

namespace Pliant.Builders.Expressions
{
    public abstract class BaseExpression
    {
        public static RuleExpression operator +(BaseExpression lhs, BaseExpression rhs)
        {
            return AddWithAnd(lhs, rhs);
        }

        public static RuleExpression operator +(string lhs, BaseExpression rhs)
        {
            return AddWithAnd(new StringLiteralLexerRule(lhs), rhs);
        }

        public static RuleExpression operator +(BaseExpression lhs, string rhs)
        {
            return AddWithAnd(lhs, new StringLiteralLexerRule(rhs));
        }

        public static RuleExpression operator +(char lhs, BaseExpression rhs)
        {
            return AddWithAnd(new TerminalLexerRule(lhs), rhs);
        }

        public static RuleExpression operator +(BaseExpression lhs, char rhs)
        {
            return AddWithAnd(lhs, new TerminalLexerRule(rhs));
        }

        public static RuleExpression operator +(BaseExpression lhs, LexerRule rhs)
        {
            return AddWithAnd(lhs, rhs);
        }

        public static RuleExpression operator +(LexerRule lhs, BaseExpression rhs)
        {
            return AddWithAnd(lhs, rhs);
        }

        public static RuleExpression operator +(Terminal lhs, BaseExpression rhs)
        {
            return AddWithAnd(lhs, rhs);
        }
        
        public static RuleExpression operator +(BaseExpression lhs, Terminal rhs)
        {
            return AddWithAnd(lhs, rhs);
        }

        public static RuleExpression operator |(BaseExpression lhs, BaseExpression rhs)
        {
            return AddWithOr(lhs, rhs);
        }

        public static RuleExpression operator |(string lhs, BaseExpression rhs)
        {
            return AddWithOr(new StringLiteralLexerRule(lhs), rhs);
        }

        public static RuleExpression operator |(BaseExpression lhs, string rhs)
        {
            return AddWithOr(lhs, new StringLiteralLexerRule(rhs));
        }

        public static RuleExpression operator |(char lhs, BaseExpression rhs)
        {
            return AddWithOr(new TerminalLexerRule(lhs), rhs);
        }

        public static RuleExpression operator |(BaseExpression lhs, char rhs)
        {
            return AddWithOr(lhs, new TerminalLexerRule(rhs));
        }

        public static RuleExpression operator |(BaseExpression lhs, LexerRule rhs)
        {
            return AddWithOr(lhs, rhs);
        }

        public static RuleExpression operator |(LexerRule lhs, BaseExpression rhs)
        {
            return AddWithOr(lhs, rhs);
        }
        
        public static RuleExpression operator |(Terminal lhs, BaseExpression rhs)
        {
            return AddWithOr(lhs, rhs);
        }

        public static RuleExpression operator |(BaseExpression lhs, Terminal rhs)
        {
            return AddWithOr(lhs, rhs);
        }

        private static RuleExpression AddWithAnd(LexerRule lhs, BaseExpression rhs)
        {
            return AddWithAnd(new SymbolExpression(new LexerRuleModel(lhs)), rhs);
        }

        private static RuleExpression AddWithAnd(BaseExpression lhs, LexerRule rhs)
        {
            return AddWithAnd(lhs, new SymbolExpression(new LexerRuleModel(rhs)));
        }

        private static RuleExpression AddWithAnd(Terminal lhs, BaseExpression rhs)
        {
            return AddWithAnd(
                new SymbolExpression(
                    new LexerRuleModel(
                        new TerminalLexerRule(lhs, lhs.ToString()))), 
                rhs);
        }

        private static RuleExpression AddWithAnd(BaseExpression lhs, Terminal rhs)
        {
            return AddWithAnd(
                lhs, 
                new SymbolExpression(
                    new LexerRuleModel(
                        new TerminalLexerRule(rhs, rhs.ToString()))));
        }

        private static RuleExpression AddWithAnd(BaseExpression lhs, BaseExpression rhs)
        {
            var expression = lhs as RuleExpression ?? new RuleExpression(lhs);
            expression.Alterations[expression.Alterations.Count - 1].Add(rhs);
            return expression;
        }
        
        private static RuleExpression AddWithOr(LexerRule lhs, BaseExpression rhs)
        {
            return AddWithOr(new SymbolExpression(new LexerRuleModel(lhs)), rhs);
        }

        private static RuleExpression AddWithOr(BaseExpression lhs, LexerRule rhs)
        {
            return AddWithOr(lhs, new SymbolExpression(new LexerRuleModel(rhs)));
        }

        private static RuleExpression AddWithOr(Terminal lhs, BaseExpression rhs)
        {
            return AddWithOr(
                new SymbolExpression(
                    new LexerRuleModel(
                        new TerminalLexerRule(lhs, lhs.ToString()))),
                rhs);
        }

        private static RuleExpression AddWithOr(BaseExpression lhs, Terminal rhs)
        {
            return AddWithOr(
                lhs,
                new SymbolExpression(
                    new LexerRuleModel(
                        new TerminalLexerRule(rhs, rhs.ToString()))));
        }

        private static RuleExpression AddWithOr(BaseExpression lhs, BaseExpression rhs)
        {
            var lhsExpression = lhs as RuleExpression ?? new RuleExpression(lhs);
            var rhsExpression = rhs as RuleExpression ?? new RuleExpression(rhs);
            foreach (var symbolList in rhsExpression.Alterations)
            {
                lhsExpression.Alterations.Add(symbolList);
            }

            return lhsExpression;
        }
    }
}
