namespace Pliant.RegularExpressions
{
    public abstract class RegexExpression : RegexNode
    {
        public override RegexNodeType NodeType => RegexNodeType.RegexExpression;
    }

    public sealed class RegexExpressionTerm : RegexExpression
    {
        public RegexExpressionTerm(RegexTerm term)
        {
            Term = term;
        }

        public RegexTerm Term { get; }

        public override RegexNodeType NodeType => RegexNodeType.RegexExpressionTerm;

        public override bool Equals(object obj)
        {
            return obj is RegexExpressionTerm other && 
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

    public sealed class RegexExpressionAlteration : RegexExpression
    {
        public RegexExpressionAlteration(RegexTerm term, RegexExpression expression)
        {
            Term = term;
            Expression = expression;
        }

        public RegexTerm Term { get; }
        public RegexExpression Expression { get; }

        public override RegexNodeType NodeType => RegexNodeType.RegexExpressionAlteration;

        public override int GetHashCode()
        {
            return (Term, Expression).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is RegexExpressionAlteration other && 
                   other.Expression.Equals(Expression);
        }

        public override string ToString()
        {
            return $"{Term}|{Expression}";
        }
    }
}