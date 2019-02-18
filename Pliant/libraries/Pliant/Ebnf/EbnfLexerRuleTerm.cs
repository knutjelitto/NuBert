namespace Pliant.Ebnf
{
    public class EbnfLexerRuleTerm : EbnfNode
    {
        public EbnfLexerRuleTerm(EbnfLexerRuleFactor factor)
        {
            Factor = factor;
        }

        public EbnfLexerRuleFactor Factor { get; }

        public override int GetHashCode()
        {
            return Factor.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfLexerRuleTerm term &&
                   term.Factor.Equals(Factor);
        }
    }

    public class EbnfLexerRuleTermConcatenation : EbnfLexerRuleTerm
    {
        public EbnfLexerRuleTermConcatenation(EbnfLexerRuleFactor factor, EbnfLexerRuleTerm term)
            : base(factor)
        {
            Term = term;
        }

        public EbnfLexerRuleTerm Term { get; }

        public override int GetHashCode()
        {
            return (Factor, Term).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfLexerRuleTermConcatenation term &&
                   term.Factor.Equals(Factor) &&
                   term.Term.Equals(Term);
        }
    }
}