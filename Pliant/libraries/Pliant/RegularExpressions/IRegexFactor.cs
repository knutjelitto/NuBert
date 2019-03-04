using System;
using Pliant.Utilities;

namespace Pliant.RegularExpressions
{
    public interface IRegexFactor : IRegexNode
    {
    }

    public class RegexFactorAtom : ValueEqualityBase<RegexFactorAtom>, IRegexFactor
    {
        public RegexFactorAtom(RegexAtom atom)
        {
            Atom = atom;
        }

        public RegexAtom Atom { get; }

        protected override bool ThisEquals(RegexFactorAtom other)
        {
            return Atom.Equals(other.Atom);
        }

        protected override object ThisHashCode => Atom;

        public override string ToString()
        {
            return Atom.ToString();
        }
    }

    public class RegexFactorIterator : ValueEqualityBase<RegexFactorIterator>, IRegexFactor
    {
        public RegexFactorIterator(RegexAtom atom, RegexIterator iterator)
        {
            Atom = atom;
            Iterator = iterator;
        }

        public RegexAtom Atom { get; }
        public RegexIterator Iterator { get; }

        protected override bool ThisEquals(RegexFactorIterator other)
        {
            return Atom.Equals(other.Atom) && Iterator.Equals(other.Iterator);
        }

        protected override object ThisHashCode => (Atom, Iterator);

        public override string ToString()
        {
            switch (Iterator)
            {
                case RegexIterator.OneOrMany:
                    return $"{Atom}+";
                case RegexIterator.ZeroOrMany:
                    return $"{Atom}*";
                case RegexIterator.ZeroOrOne:
                    return $"{Atom}?";
            }

            throw new InvalidOperationException("Unexpected RegexIterator encountered.");
        }
    }
}