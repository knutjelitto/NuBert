using Pliant.Utilities;

namespace Pliant.RegularExpressions
{
    public interface IRegexTerm : IRegexNode
    {
    }

    public sealed class RegexTermFactor : ValueEqualityBase<RegexTermFactor>, IRegexTerm
    {
        public RegexTermFactor(IRegexFactor factor)
        {
            Factor = factor;
        }

        public IRegexFactor Factor { get; }

        protected override bool ThisEquals(RegexTermFactor other)
        {
            return Factor.Equals(other.Factor);
        }

        protected override object ThisHashCode => Factor;

        public override string ToString()
        {
            return Factor.ToString();
        }
    }

    public sealed class RegexTermFactorTerm : ValueEqualityBase<RegexTermFactorTerm>, IRegexTerm
    {
        public RegexTermFactorTerm(IRegexFactor factor, IRegexTerm term)
        {
            Factor = factor;
            Term = term;
        }

        public IRegexFactor Factor { get; }
        public IRegexTerm Term { get; }

        protected override bool ThisEquals(RegexTermFactorTerm other)
        {
            return Factor.Equals(other.Factor) && Term.Equals(other.Term);
        }

        protected override object ThisHashCode => (Factor, Term);

        public override string ToString()
        {
            return $"{Factor}{Term}";
        }
    }
}