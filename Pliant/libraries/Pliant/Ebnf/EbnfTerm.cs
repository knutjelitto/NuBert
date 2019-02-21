namespace Pliant.Ebnf
{
    public abstract class EbnfTerm : EbnfNode
    {
        protected EbnfTerm(EbnfFactor factor)
        {
            Factor = factor;
        }

        public EbnfFactor Factor { get; }
    }

    public sealed class EbnfTermSimple : EbnfTerm
    {
        public EbnfTermSimple(EbnfFactor factor)
            : base(factor)
        {
        }

        public override int GetHashCode()
        {
            return Factor.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfTermSimple other && other.Factor.Equals(Factor);
        }

        public override string ToString()
        {
            return Factor.ToString();
        }
    }

    public sealed class EbnfTermConcatenation : EbnfTerm
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
            return obj is EbnfTermConcatenation other && 
                   Factor.Equals(other.Factor) && 
                   Term.Equals(other.Term);
        }

        public override string ToString()
        {
            return $"{Factor} {Term}";
        }
    }
}