using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public class EbnfRule : ValueEqualityBase<EbnfRule>, IEbnfNode
    {
        public EbnfRule(EbnfQualifiedIdentifier identifier, IEbnfExpression expression)
        {
            Identifier = identifier;
            Expression = expression;
        }

        public EbnfQualifiedIdentifier Identifier { get; }
        public IEbnfExpression Expression { get; }

        protected override bool ThisEquals(EbnfRule other)
        {
            return Identifier.Equals(other.Identifier) &&
                   Expression.Equals(other.Expression);
        }

        protected override object ThisHashCode => (Identifier, Expression);

        public override string ToString()
        {
            return $"{Identifier} = {Expression}";
        }
    }
}