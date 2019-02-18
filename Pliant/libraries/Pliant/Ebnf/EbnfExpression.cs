namespace Pliant.Ebnf
{
    public class EbnfExpressionEmpty : EbnfNode
    {
        public override bool Equals(object obj)
        {
            return obj is EbnfExpressionEmpty;
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }

    public class EbnfExpression : EbnfExpressionEmpty
    {
        public EbnfExpression(EbnfTerm term)
        {
            Term = term;
        }

        public EbnfTerm Term { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfExpression other && 
                   other.Term.Equals(Term);
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

    public class EbnfExpressionAlteration : EbnfExpression
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