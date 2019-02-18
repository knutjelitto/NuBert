namespace Pliant.Ebnf
{
    public class EbnfTerm : EbnfNode
    {
        public EbnfTerm(EbnfFactor factor)
        {
            Factor = factor;
        }

        public EbnfFactor Factor { get; }

        public override int GetHashCode()
        {
            return Factor.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfTerm other &&
                   other.Factor.Equals(Factor);
        }

        public override string ToString()
        {
            return Factor.ToString();
        }
    }

    public class EbnfTermConcatenation : EbnfTerm
    {
        public EbnfTermConcatenation(EbnfFactor factor, EbnfTerm term)
            : base(factor)
        {
            Term = term;
        }

        public EbnfTerm Term { get; }

        public override int GetHashCode()
        {
            return (Factor, Term).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfTermConcatenation term && 
                   term.Factor.Equals(Factor) && 
                   term.Term.Equals(Term);
        }

        public override string ToString()
        {
            return $"{Factor} {Term}";
        }
    }
}