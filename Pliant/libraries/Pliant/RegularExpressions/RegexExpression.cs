namespace Pliant.RegularExpressions
{
    public abstract class RegexExpression : IRegexNode
    {
    }

    public sealed class RegexExpressionTerm : RegexExpression
    {
        public RegexExpressionTerm(IRegexTerm term)
        {
            Term = term;
        }

        public IRegexTerm Term { get; }

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
        public RegexExpressionAlteration(IRegexTerm term, RegexExpression expression)
        {
            Term = term;
            Expression = expression;
        }

        public IRegexTerm Term { get; }
        public RegexExpression Expression { get; }

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