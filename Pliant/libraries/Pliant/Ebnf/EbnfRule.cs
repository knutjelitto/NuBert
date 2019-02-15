using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public class EbnfRule : EbnfNode
    {
        private readonly int _hashCode;

        public EbnfQualifiedIdentifier QualifiedIdentifier { get; private set; }
        public EbnfExpression Expression { get; private set; }

        public EbnfRule(EbnfQualifiedIdentifier qualifiedIdentifier, EbnfExpression expression)
        {
            QualifiedIdentifier = qualifiedIdentifier;
            Expression = expression;
            this._hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfRule;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                QualifiedIdentifier.GetHashCode(),
                Expression.GetHashCode(),
                NodeType.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var rule = obj as EbnfRule;
            if (rule == null)
            {
                return false;
            }

            return rule.NodeType == EbnfNodeType.EbnfRule
                && rule.QualifiedIdentifier.Equals(QualifiedIdentifier)
                && rule.Expression.Equals(Expression);
        }

        public override string ToString()
        {
            return $"{QualifiedIdentifier} = {Expression}";
        }
    }
}