using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public interface IEbnfLexerRuleExpression : IEbnfNode
    {
        IEbnfLexerRuleTerm Term { get; }
    }

    public sealed class EbnfLexerRuleExpressionSimple : ValueEqualityBase<EbnfLexerRuleExpressionSimple>, IEbnfLexerRuleExpression
    {
        public IEbnfLexerRuleTerm Term { get; }

        public EbnfLexerRuleExpressionSimple(IEbnfLexerRuleTerm term)
            : base(term.GetHashCode())
        {
            Term = term;
        }

        public override bool ThisEquals(EbnfLexerRuleExpressionSimple other)
        {
            return other.Term.Equals(Term);
        }
    }

    public sealed class EbnfLexerRuleExpressionAlteration : ValueEqualityBase<EbnfLexerRuleExpressionAlteration>, IEbnfLexerRuleExpression
    {
        public EbnfLexerRuleExpressionAlteration(IEbnfLexerRuleTerm term, IEbnfLexerRuleExpression expression)
            : base((term, expression).GetHashCode())
        {
            Term = term;
            Expression = expression;
        }

        public IEbnfLexerRuleTerm Term { get; }
        public IEbnfLexerRuleExpression Expression { get; }

        public override bool ThisEquals(EbnfLexerRuleExpressionAlteration other)
        {
            return Term.Equals(other.Term) &&
                   Expression.Equals(other.Expression);
        }
    }
}