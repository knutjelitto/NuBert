using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public interface IEbnfLexerRuleExpression : IEbnfNode
    {
        IEbnfLexerRuleTerm Term { get; }
    }

    public sealed class EbnfLexerRuleExpressionSimple : ValueEqualityBase<EbnfLexerRuleExpressionSimple>, IEbnfLexerRuleExpression
    {
        public EbnfLexerRuleExpressionSimple(IEbnfLexerRuleTerm term)
        {
            Term = term;
        }

        public IEbnfLexerRuleTerm Term { get; }

        protected override object ThisHashCode => Term;

        protected override bool ThisEquals(EbnfLexerRuleExpressionSimple other)
        {
            return other.Term.Equals(Term);
        }
    }

    public sealed class EbnfLexerRuleExpressionAlteration : ValueEqualityBase<EbnfLexerRuleExpressionAlteration>,
                                                            IEbnfLexerRuleExpression
    {
        public EbnfLexerRuleExpressionAlteration(IEbnfLexerRuleTerm term, IEbnfLexerRuleExpression expression)
        {
            Term = term;
            Expression = expression;
        }

        public IEbnfLexerRuleExpression Expression { get; }

        public IEbnfLexerRuleTerm Term { get; }

        protected override bool ThisEquals(EbnfLexerRuleExpressionAlteration other)
        {
            return Term.Equals(other.Term) &&
                   Expression.Equals(other.Expression);
        }

        protected override object ThisHashCode => (Term, Expression);
    }
}