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

        public override RegexNodeType NodeType => RegexNodeType.Regex;

        public override bool Equals(object obj)
        {
            return obj is Regex other &&
                   other.StartsWith == StartsWith &&
                   other.EndsWith == EndsWith && 
                   other.Expression.Equals(Expression);
        }

        public override int GetHashCode()
        {
            return (StartsWith, EndsWith, Expression).GetHashCode();
        }

        public override string ToString()
        {
            return $"{(StartsWith ? "^" : string.Empty)}{Expression}{(EndsWith ? "$" : string.Empty)}";
        }
    }
}