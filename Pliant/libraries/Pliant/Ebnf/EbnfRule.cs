namespace Pliant.Ebnf
{
    public class EbnfRule : EbnfNode
    {
        public EbnfRule(EbnfQualifiedIdentifier qualifiedEbnfQualifiedIdentifier, EbnfExpression expression)
        {
            QualifiedEbnfQualifiedIdentifier = qualifiedEbnfQualifiedIdentifier;
            Expression = expression;
        }

        public EbnfQualifiedIdentifier QualifiedEbnfQualifiedIdentifier { get; }
        public EbnfExpression Expression { get; }

        public override int GetHashCode()
        {
            return (QualifiedIdentifier: QualifiedEbnfQualifiedIdentifier, Expression).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfRule rule && 
                   rule.QualifiedEbnfQualifiedIdentifier.Equals(QualifiedEbnfQualifiedIdentifier) && 
                   rule.Expression.Equals(Expression);
        }

        public override string ToString()
        {
            return $"{QualifiedEbnfQualifiedIdentifier} = {Expression}";
        }
    }
}