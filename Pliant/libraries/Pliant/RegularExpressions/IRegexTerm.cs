using Pliant.Utilities;

namespace Pliant.RegularExpressions
{
    public interface IRegexTerm : IRegexNode
    {
    }

    public sealed class RegexTermFactor : ValueEqualityBase<RegexTermFactor>, IRegexTerm
    {
        public RegexTermFactor(IRegexFactor factor)
            : base(factor.GetHashCode())
        {
            Factor = factor;
        }

        public IRegexFactor Factor { get; }

        public override bool ThisEquals(RegexTermFactor other)
        {
            return Factor.Equals(other.Factor);
        }

        public override string ToString()
        {
            return Factor.ToString();
        }
    }

    public sealed class RegexTermFactorTerm : ValueEqualityBase<RegexTermFactorTerm>, IRegexTerm
    {
        public RegexTermFactorTerm(IRegexFactor factor, IRegexTerm term)
            : base((factor, term))
        {
            Factor = factor;
            Term = term;
        }

        public IRegexFactor Factor { get; }
        public IRegexTerm Term { get; }

        public override bool ThisEquals(RegexTermFactorTerm other)
        {
            return Factor.Equals(other.Factor) && Term.Equals(other.Term);
        }

        public override string ToString()
        {
            return $"{Factor}{Term}";
        }
    }
}