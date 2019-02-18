namespace Pliant.Ebnf
{
    public class EbnfLexerRuleExpression : EbnfNode
    {
        public EbnfLexerRuleExpression(EbnfLexerRuleTerm term)
        {
            Term = term;
        }

        public EbnfLexerRuleTerm Term { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfLexerRuleExpression expression && 
                   expression.Term.Equals(Term);
        }

        public override int GetHashCode()
        {
            return Term.GetHashCode();
        }
    }

    public class EbnfLexerRuleExpressionAlteration : EbnfLexerRuleExpression
    {
        public EbnfLexerRuleExpressionAlteration(EbnfLexerRuleTerm term, EbnfLexerRuleExpression expression)
            : base(term)
        {
            Expression = expression;
        }

        public EbnfLexerRuleExpression Expression { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfLexerRuleExpressionAlteration expression && 
                   expression.Term.Equals(Term) && 
                   expression.Expression.Equals(Expression);
        }

        public override int GetHashCode()
        {
            return (Term, Expression).GetHashCode();
        }
    }
}