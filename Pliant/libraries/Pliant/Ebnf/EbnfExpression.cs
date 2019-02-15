using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public class EbnfExpressionEmpty: EbnfNode
    {
        private readonly int _hashCode;

        public EbnfExpressionEmpty()
        {
            this._hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfExpressionEmpty;

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var expression = obj as EbnfExpressionEmpty;
            if (expression == null)
            {
                return false;
            }

            return expression.NodeType == NodeType;
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode());
        }
    }

    public class EbnfExpression : EbnfExpressionEmpty
    {
        private readonly int _hashCode;

        public EbnfTerm Term { get; private set; }

        public EbnfExpression(EbnfTerm term)
        {
            Term = term;
            this._hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfExpression;

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var expression = obj as EbnfExpression;
            if (expression == null)
            {
                return false;
            }

            return expression.NodeType == NodeType
                && expression.Term.Equals(Term);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                Term.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return Term.ToString();
        }
    }

    public class EbnfExpressionAlteration : EbnfExpression
    {
        private readonly int _hashCode;

        public EbnfExpression Expression { get; private set; }

        public EbnfExpressionAlteration(
            EbnfTerm term,
            EbnfExpression expression)
            : base(term)
        {
            Expression = expression;
            this._hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfExpressionAlteration;

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var expression = obj as EbnfExpressionAlteration;
            if (expression == null)
            {
                return false;
            }

            return expression.NodeType == NodeType
                && expression.Term.Equals(Term)
                && expression.Expression.Equals(Expression);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                Term.GetHashCode(),
                Expression.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return $"{Term} | {Expression}";
        }
    }
}