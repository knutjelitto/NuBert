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
            : base(atom)
        {
            Atom = atom;
        }

        public RegexAtom Atom { get; }

        public override bool ThisEquals(RegexFactorAtom other)
        {
            return Atom.Equals(other.Atom);
        }

        public override string ToString()
        {
            return Atom.ToString();
        }
    }

    public class RegexFactorIterator : ValueEqualityBase<RegexFactorIterator>, IRegexFactor
    {
        public RegexFactorIterator(RegexAtom atom, RegexIterator iterator)
            : base((atom, iterator))
        {
            Atom = atom;
            Iterator = iterator;
        }

        public RegexAtom Atom { get; }
        public RegexIterator Iterator { get; }

        public override bool ThisEquals(RegexFactorIterator other)
        {
            return Atom.Equals(other.Atom) && Iterator.Equals(other.Iterator);
        }

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