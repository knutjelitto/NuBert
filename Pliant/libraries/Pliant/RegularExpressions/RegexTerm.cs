using Pliant.Utilities;

namespace Pliant.RegularExpressions
{
    public class RegexTerm : RegexNode
    {
        public RegexTerm(RegexFactor factor)
        {
            Factor = factor;
        }

        public RegexFactor Factor { get; }

        public override bool Equals(object obj)
        {
            return obj is RegexTerm other && 
                   other.Factor.Equals(Factor);
        }

        public override int GetHashCode()
        {
            return Factor.GetHashCode();
        }

        public override string ToString()
        {
            return Factor.ToString();
        }
    }

    public class RegexTermFactor : RegexTerm
    {
        public RegexTermFactor(RegexFactor factor, RegexTerm term)
            : base(factor)
        {
            Term = term;
        }

        public RegexTerm Term { get; }

        public override bool Equals(object obj)
        {
            return obj is RegexTermFactor other && 
                   other.Factor.Equals(Factor) && 
                   other.Term.Equals(Term);
        }

        public override int GetHashCode()
        {
            return (Factor, Term).GetHashCode();
        }

        public override string ToString()
        {
            return $"{Factor}{Term}";
        }
    }
}