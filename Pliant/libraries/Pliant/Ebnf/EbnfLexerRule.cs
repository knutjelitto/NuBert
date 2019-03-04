using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public class EbnfLexerRule : ValueEqualityBase<EbnfLexerRule>, IEbnfNode
    {
        public EbnfLexerRule(EbnfQualifiedIdentifier identifier, IEbnfLexerRuleExpression expression)
        {
            Identifier = identifier;
            Expression = expression;
        }

        public EbnfQualifiedIdentifier Identifier { get; }
        public IEbnfLexerRuleExpression Expression { get; }

        protected override bool ThisEquals(EbnfLexerRule other)
        {
            return other.Identifier.Equals(Identifier) &&
                   other.Expression.Equals(Expression);
        }

        protected override object ThisHashCode => (Identifier, Expression);
    }
}