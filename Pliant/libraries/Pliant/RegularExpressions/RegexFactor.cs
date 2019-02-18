using System;
using Pliant.Utilities;

namespace Pliant.RegularExpressions
{
    public class RegexFactor : RegexNode
    {
        public RegexFactor(RegexAtom atom)
        {
            Atom = atom;
        }

        public RegexAtom Atom { get; }

        public override bool Equals(object obj)
        {
            return obj is RegexFactor other && other.Atom.Equals(Atom);
        }

        public override int GetHashCode()
        {
            return Atom.GetHashCode();
        }

        public override string ToString()
        {
            return Atom.ToString();
        }
    }

    public class RegexFactorIterator : RegexFactor
    {
        public RegexFactorIterator(RegexAtom atom, RegexIterator iterator)
            : base(atom)
        {
            Iterator = iterator;
        }

        public RegexIterator Iterator { get; }

        public override bool Equals(object obj)
        {
            return obj is RegexFactorIterator other &&
                   other.Atom.Equals(Atom) &&
                   other.Iterator.Equals(Iterator);
        }

        public override int GetHashCode()
        {
            return (Atom, Iterator).GetHashCode();
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