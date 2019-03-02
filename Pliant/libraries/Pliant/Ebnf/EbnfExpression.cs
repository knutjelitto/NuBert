using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public interface IEbnfExpression : IEbnfNode
    {
        IEbnfTerm Term { get;  }
    }

    public sealed class EbnfExpressionSimple : ValueEqualityBase<EbnfExpressionSimple>, IEbnfExpression
    {
        public IEbnfTerm Term { get; }

        public EbnfExpressionSimple(IEbnfTerm term)
            : base(term.GetHashCode())
        {
            Term = term;
        }

        public override bool ThisEquals(EbnfExpressionSimple other)
        {
            return Term.Equals(other.Term);
        }

        public override string ToString()
        {
            return Term.ToString();
        }
    }

    public sealed class EbnfExpressionAlteration : ValueEqualityBase<EbnfExpressionAlteration>, IEbnfExpression
    {
        public EbnfExpressionAlteration(IEbnfTerm term, IEbnfExpression expression)
            : base((term, expression).GetHashCode())
        {
            Term = term;
            Expression = expression;
        }

        public IEbnfTerm Term { get; }
        public IEbnfExpression Expression { get; }

        public override bool ThisEquals(EbnfExpressionAlteration other)
        {
            return other.Term.Equals(Term) &&
                   other.Expression.Equals(Expression);
        }

        public override string ToString()
        {
            return $"{Term} | {Expression}";
        }
    }
}