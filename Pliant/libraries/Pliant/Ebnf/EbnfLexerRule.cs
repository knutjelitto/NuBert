namespace Pliant.Ebnf
{
    public class EbnfLexerRule : EbnfNode
    {
        public EbnfLexerRule(EbnfQualifiedIdentifier qualifiedIdentifier, EbnfLexerRuleExpression expression)
        {
            QualifiedIdentifier = qualifiedIdentifier;
            Expression = expression;
        }

        public EbnfLexerRuleExpression Expression { get; }

        public EbnfQualifiedIdentifier QualifiedIdentifier { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfLexerRule other &&
                   other.QualifiedIdentifier.Equals(QualifiedIdentifier) &&
                   other.Expression.Equals(Expression);
        }

        public override int GetHashCode()
        {
            return (QualifiedIdentifier, Expression).GetHashCode();
        }
    }
}