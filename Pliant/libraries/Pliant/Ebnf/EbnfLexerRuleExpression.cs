namespace Pliant.Ebnf
{
    public abstract class EbnfLexerRuleExpression : EbnfNode
    {
        protected EbnfLexerRuleExpression(EbnfLexerRuleTerm term)
        {
            Term = term;
        }

        public EbnfLexerRuleTerm Term { get; }
    }

    public sealed class EbnfLexerRuleExpressionSimple : EbnfLexerRuleExpression
    {
        public EbnfLexerRuleExpressionSimple(EbnfLexerRuleTerm term)
            : base(term)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfLexerRuleExpressionSimple other &&
                   other.Term.Equals(Term);
        }

        public override int GetHashCode()
        {
            return Term.GetHashCode();
        }
    }

    public sealed class EbnfLexerRuleExpressionAlteration : EbnfLexerRuleExpression
    {
        public EbnfLexerRuleExpressionAlteration(EbnfLexerRuleTerm term, EbnfLexerRuleExpression expression)
            : base(term)
        {
            Expression = expression;
        }

        public EbnfLexerRuleExpression Expression { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfLexerRuleExpressionAlteration other &&
                   Term.Equals(other.Term) &&
                   Expression.Equals(other.Expression);
        }

        public override int GetHashCode()
        {
            return (Term, Expression).GetHashCode();
        }
    }
}