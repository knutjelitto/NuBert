using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public class EbnfRule : ValueEqualityBase<EbnfRule>, IEbnfNode
    {
        public EbnfRule(EbnfQualifiedIdentifier identifier, IEbnfExpression expression)
            : base((identifier, expression))
        {
            Identifier = identifier;
            Expression = expression;
        }

        public EbnfQualifiedIdentifier Identifier { get; }
        public IEbnfExpression Expression { get; }

        public override bool ThisEquals(EbnfRule other)
        {
            return Identifier.Equals(other.Identifier) &&
                   Expression.Equals(other.Expression);
        }

        public override string ToString()
        {
            return $"{Identifier} = {Expression}";
        }
    }
}