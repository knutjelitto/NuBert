using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public class EbnfLexerRuleExpression : EbnfNode
    {
        public EbnfLexerRuleTerm Term { get; private set;  }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfLexerRuleExpression;

        private readonly int _hashCode;

        public EbnfLexerRuleExpression(EbnfLexerRuleTerm term)
        {
            Term = term;
            this._hashCode = ComputeHashCode();
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var expression = obj as EbnfLexerRuleExpression;
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
    }

    public class EbnfLexerRuleExpressionAlteration : EbnfLexerRuleExpression
    {
        private readonly int _hashCode;

        public EbnfLexerRuleExpression Expression { get; private set; }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfLexerRuleExpressionAlteration;

        public EbnfLexerRuleExpressionAlteration(EbnfLexerRuleTerm term, EbnfLexerRuleExpression expression)
            : base(term)
        {
            Expression = expression;
            this._hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var expression = obj as EbnfLexerRuleExpressionAlteration;
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
    }
}
