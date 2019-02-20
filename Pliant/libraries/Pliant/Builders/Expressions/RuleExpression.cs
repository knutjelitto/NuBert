using System.Collections.Generic;
using Pliant.Grammars;

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
            return new RuleExpression(productionExpression);
        }

        public static implicit operator RuleExpression(string literal)
        {
            return new RuleExpression(
                new SymbolExpression(
                    new LexerRuleModel(
                        new StringLiteralLexerRule(literal))));
        }

        public static implicit operator RuleExpression(char literal)
        {
            return new RuleExpression(
                new SymbolExpression(
                    new LexerRuleModel(
                        new TerminalLexerRule(literal))));
        }

        public static implicit operator RuleExpression(LexerRule lexerRule)
        {
            return new RuleExpression(
                new SymbolExpression(
                    new LexerRuleModel(
                        lexerRule)));
        }

        public static implicit operator RuleExpression(Terminal baseTerminal)
        {
            return new RuleExpression(
                new SymbolExpression(
                    new LexerRuleModel(
                        new TerminalLexerRule(baseTerminal, baseTerminal.ToString()))));
        }

        public static implicit operator RuleExpression(ProductionReferenceExpression productionReference)
        {
            return new RuleExpression(productionReference);
        }
    }
}