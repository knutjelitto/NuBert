using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public interface IEbnfTerm : IEbnfNode
    {
        IEbnfFactor Factor { get; }
    }

    public sealed class EbnfTermSimple : ValueEqualityBase<EbnfTermSimple>, IEbnfTerm
    {
        public IEbnfFactor Factor { get; }

        public EbnfTermSimple(IEbnfFactor factor)
        {
            Factor = factor;
        }

        protected override bool ThisEquals(EbnfTermSimple other)
        {
            return other.Factor.Equals(Factor);
        }

        protected override object ThisHashCode => Factor;

        public override string ToString()
        {
            return Factor.ToString();
        }
    }

    public sealed class EbnfTermConcatenation : ValueEqualityBase<EbnfTermConcatenation>, IEbnfTerm
    {
        public EbnfTermConcatenation(IEbnfFactor factor, IEbnfTerm term)
        {
            Factor = factor;
            Term = term;
        }

        public IEbnfFactor Factor { get; }
        public IEbnfTerm Term { get; }

        protected override bool ThisEquals(EbnfTermConcatenation other)
        {
            return Factor.Equals(other.Factor) &&
                   Term.Equals(other.Term);
        }

        protected override object ThisHashCode => (Factor, Term);

        public override string ToString()
        {
            return $"{Factor} {Term}";
        }
    }
}