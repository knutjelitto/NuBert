namespace Pliant.Ebnf
{
    public class EbnfLexerRule : EbnfNode
    {
        public EbnfLexerRule(EbnfQualifiedIdentifier identifier, EbnfLexerRuleExpression expression)
        {
            Identifier = identifier;
            Expression = expression;
        }

        public EbnfQualifiedIdentifier Identifier { get; }
        public EbnfLexerRuleExpression Expression { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfLexerRule other &&
                   other.Identifier.Equals(Identifier) &&
                   other.Expression.Equals(Expression);
        }

        public override int GetHashCode()
        {
            return (QualifiedIdentifier: Identifier, Expression).GetHashCode();
        }
    }
}