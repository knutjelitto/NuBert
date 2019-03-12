using System.Collections.Generic;
using Pliant.Grammars;
using Pliant.Terminals;

namespace Pliant.Builders.Expressions
{
    public class RuleExpression : BaseExpression
    {
        public RuleExpression(BaseExpression baseExpression)
        {
            Alterations = new List<List<BaseExpression>>();
            AddWithAnd(baseExpression);
        }

        public List<List<BaseExpression>> Alterations { get; }

        private void AddWithAnd(BaseExpression baseExpression)
        {
            if (Alterations.Count == 0)
            {
                Alterations.Add(new List<BaseExpression>());
            }

            Alterations[Alterations.Count - 1].Add(baseExpression);
        }

        public static implicit operator RuleExpression(ProductionExpression productionExpression)
        {
            return (Expr) productionExpression;
        }

        public static implicit operator RuleExpression(GrammarReferenceExpression grammarReference)
        {
            return (Expr) grammarReference;
        }

        public static implicit operator RuleExpression(string literal)
        {
            return (Expr) literal;
        }

        public static implicit operator RuleExpression(char literal)
        {
            return (Expr) literal;
        }

        public static implicit operator RuleExpression(LexerRule lexerRule)
        {
            return (Expr) lexerRule;
        }

        public static implicit operator RuleExpression(AtomTerminal baseTerminal)
        {
            return (Expr) baseTerminal;
        }
    }
}