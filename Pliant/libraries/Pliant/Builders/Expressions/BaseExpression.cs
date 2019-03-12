using Pliant.Grammars;
using Pliant.Terminals;

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
            return AddWithAnd((Expr)lhs, rhs);
        }

        public static RuleExpression operator +(BaseExpression lhs, string rhs)
        {
            return AddWithAnd(lhs, (Expr)rhs);
        }

        public static RuleExpression operator +(char lhs, BaseExpression rhs)
        {
            return AddWithAnd((Expr)lhs, rhs);
        }

        public static RuleExpression operator +(BaseExpression lhs, char rhs)
        {
            return AddWithAnd(lhs, (Expr)rhs);
        }

        public static RuleExpression operator +(LexerRule lhs, BaseExpression rhs)
        {
            return AddWithAnd((Expr)lhs, rhs);
        }

        public static RuleExpression operator +(BaseExpression lhs, LexerRule rhs)
        {
            return AddWithAnd(lhs, (Expr)rhs);
        }

        public static RuleExpression operator +(AtomTerminal lhs, BaseExpression rhs)
        {
            return AddWithAnd((Expr)lhs, rhs);
        }

        public static RuleExpression operator +(BaseExpression lhs, AtomTerminal rhs)
        {
            return AddWithAnd(lhs, (Expr)rhs);
        }

        public static RuleExpression operator |(BaseExpression lhs, BaseExpression rhs)
        {
            return AddWithOr(lhs, rhs);
        }

        public static RuleExpression operator |(string lhs, BaseExpression rhs)
        {
            return AddWithOr((Expr)lhs, rhs);
        }

        public static RuleExpression operator |(BaseExpression lhs, string rhs)
        {
            return AddWithOr(lhs, (Expr)rhs);
        }

        public static RuleExpression operator |(char lhs, BaseExpression rhs)
        {
            return AddWithOr((Expr)lhs, rhs);
        }

        public static RuleExpression operator |(BaseExpression lhs, char rhs)
        {
            return AddWithOr(lhs, (Expr)rhs);
        }

        public static RuleExpression operator |(LexerRule lhs, BaseExpression rhs)
        {
            return AddWithOr((Expr)lhs, rhs);
        }

        public static RuleExpression operator |(BaseExpression lhs, LexerRule rhs)
        {
            return AddWithOr(lhs, (Expr)rhs);
        }

        public static RuleExpression operator |(AtomTerminal lhs, BaseExpression rhs)
        {
            return AddWithOr((Expr)lhs, rhs);
        }

        public static RuleExpression operator |(BaseExpression lhs, AtomTerminal rhs)
        {
            return AddWithOr(lhs, (Expr)rhs);
        }

        private static RuleExpression AddWithAnd(BaseExpression lhs, BaseExpression rhs)
        {
            var expression = lhs as RuleExpression ?? new RuleExpression(lhs);
            expression.Alterations[expression.Alterations.Count - 1].Add(rhs);
            return expression;
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
