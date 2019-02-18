namespace Pliant.Ebnf
{
    public class EbnfRule : EbnfNode
    {
        public EbnfRule(EbnfQualifiedIdentifier qualifiedIdentifier, EbnfExpression expression)
        {
            QualifiedIdentifier = qualifiedIdentifier;
            Expression = expression;
        }

        public EbnfQualifiedIdentifier QualifiedIdentifier { get; }
        public EbnfExpression Expression { get; }

        public override int GetHashCode()
        {
            return (QualifiedIdentifier, Expression).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfRule rule && 
                   rule.QualifiedIdentifier.Equals(QualifiedIdentifier) && 
                   rule.Expression.Equals(Expression);
        }

        public override string ToString()
        {
            return $"{QualifiedIdentifier} = {Expression}";
        }
    }
}