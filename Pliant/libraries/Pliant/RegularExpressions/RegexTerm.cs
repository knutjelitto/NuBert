using Pliant.Utilities;

namespace Pliant.RegularExpressions
{
    public abstract class RegexTerm : RegexNode
    {
    }

    public sealed class RegexTermFactor : RegexTerm
    {
        public RegexTermFactor(RegexFactor factor)
        {
            Factor = factor;
            this._hashCode = ComputeHashCode();
        }

        public RegexFactor Factor { get; }

        public override bool Equals(object obj)
        {
            return obj is RegexTermFactor other && other.Factor.Equals(Factor);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return Factor.ToString();
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(Factor.GetHashCode());
        }

        private readonly int _hashCode;
    }

    public sealed class RegexTermFactorTerm : RegexTerm
    {
        public RegexTermFactorTerm(RegexFactor factor, RegexTerm term)
        {
            Factor = factor;
            Term = term;
            this._hashCode = ComputeHashCode();
        }

        public RegexFactor Factor { get; }
        public RegexTerm Term { get; }

        public override bool Equals(object obj)
        {
            return obj is RegexTermFactorTerm termFactor && termFactor.Factor.Equals(Factor) && termFactor.Term.Equals(Term);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return $"{Factor}{Term}";
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                Term.GetHashCode(),
                Factor.GetHashCode());
        }

        private readonly int _hashCode;
    }
}