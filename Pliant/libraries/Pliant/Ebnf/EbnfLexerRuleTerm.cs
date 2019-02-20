namespace Pliant.Ebnf
{
    public abstract class EbnfLexerRuleTerm : EbnfNode
    {
        protected EbnfLexerRuleTerm(EbnfLexerRuleFactor factor)
        {
            Factor = factor;
        }

        public EbnfLexerRuleFactor Factor { get; }
    }

    public sealed class EbnfLexerRuleTermSimple : EbnfLexerRuleTerm
    {
        public EbnfLexerRuleTermSimple(EbnfLexerRuleFactor factor)
            : base(factor)
        {
        }

        public override int GetHashCode()
        {
            return Factor.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfLexerRuleTermSimple other &&
                   other.Factor.Equals(Factor);
        }
    }

    public sealed class EbnfLexerRuleTermConcatenation : EbnfLexerRuleTerm
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