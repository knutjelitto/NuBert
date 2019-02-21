namespace Pliant.Ebnf
{
    public abstract class EbnfExpression : EbnfNode
    {
        protected EbnfExpression(EbnfTerm term)
        {
            Term = term;
        }

        public EbnfTerm Term { get; }
    }

    public sealed class EbnfExpressionSimple : EbnfExpression
    {
        public EbnfExpressionSimple(EbnfTerm term)
            : base(term)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfExpressionSimple other && 
                   Term.Equals(other.Term);
        }

        public override int GetHashCode()
        {
            return Term.GetHashCode();
        }

        public override string ToString()
        {
            return Term.ToString();
        }
    }

    public sealed class EbnfExpressionAlteration : EbnfExpression
    {
        public EbnfExpressionAlteration(EbnfTerm term, EbnfExpression expression)
            : base(term)
        {
            Expression = expression;
        }

        public EbnfExpression Expression { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfExpressionAlteration other && 
                   other.Term.Equals(Term) && 
                   other.Expression.Equals(Expression);
        }

        public override int GetHashCode()
        {
            return (Term, Expression).GetHashCode();
        }

        public override string ToString()
        {
            return $"{Term} | {Expression}";
        }
    }
}