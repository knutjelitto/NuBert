using Pliant.Utilities;

namespace Pliant.RegularExpressions
{
    public sealed class Regex : ValueEqualityBase<Regex>, IRegexNode
    {
        public Regex(bool startsWith, RegexExpression expression, bool endsWith)
            : base((startsWith, expression, endsWith))
        {
            StartsWith = startsWith;
            EndsWith = endsWith;
            Expression = expression;
        }

        public bool StartsWith { get; }
        public RegexExpression Expression { get; }
        public bool EndsWith { get; }

        public override bool ThisEquals(Regex other)
        {
            return StartsWith.Equals(other.StartsWith) &&
                   Expression.Equals(other.Expression) &&
                   EndsWith.Equals(other.EndsWith);
        }

        public override string ToString()
        {
            return $"{(StartsWith ? "^" : string.Empty)}{Expression}{(EndsWith ? "$" : string.Empty)}";
        }
    }
}