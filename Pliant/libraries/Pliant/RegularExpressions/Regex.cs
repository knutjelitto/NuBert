namespace Pliant.RegularExpressions
{
    public sealed class Regex : RegexNode
    {
        public Regex(
            bool startsWith,
            RegexExpression expression,
            bool endsWith)
        {
            StartsWith = startsWith;
            EndsWith = endsWith;
            Expression = expression;
        }

        public bool StartsWith { get; }
        public RegexExpression Expression { get; }
        public bool EndsWith { get; }

        public override bool Equals(object obj)
        {
            return obj is Regex other &&
                   StartsWith.Equals(other.StartsWith) &&
                   Expression.Equals(other.Expression) &&
                   EndsWith.Equals(other.EndsWith);
        }

        public override int GetHashCode()
        {
            return (StartsWith, Expression, EndsWith).GetHashCode();
        }

        public override string ToString()
        {
            return $"{(StartsWith ? "^" : string.Empty)}{Expression}{(EndsWith ? "$" : string.Empty)}";
        }
    }
}