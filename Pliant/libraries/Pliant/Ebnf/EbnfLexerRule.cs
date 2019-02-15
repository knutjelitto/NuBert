using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public class EbnfLexerRule : EbnfNode
    {
        public EbnfQualifiedIdentifier QualifiedIdentifier { get; private set; }
        public EbnfLexerRuleExpression Expression { get; private set; }

        private readonly int _hashCode;

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfLexerRule;

        public EbnfLexerRule(EbnfQualifiedIdentifier qualifiedIdentifier, EbnfLexerRuleExpression expression)
        {
            QualifiedIdentifier = qualifiedIdentifier;
            Expression = expression;
            this._hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var ebnfLexerRule = obj as EbnfLexerRule;
            if (ebnfLexerRule == null)
            {
                return false;
            }

            return ebnfLexerRule.QualifiedIdentifier.Equals(QualifiedIdentifier)
                && ebnfLexerRule.Expression.Equals(Expression);
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                QualifiedIdentifier.GetHashCode(),
                Expression.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }
    }
}