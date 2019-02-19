using System;
using Pliant.Utilities;

namespace Pliant.RegularExpressions
{
    public abstract class RegexFactor : RegexNode
    {
    }

    public class RegexFactorAtom : RegexFactor
    {
        public RegexFactorAtom(RegexAtom atom)
        {
            Atom = atom;
            this._hashCode = ComputeHashCode();
        }

        public RegexAtom Atom { get; }

        public override RegexNodeType NodeType => RegexNodeType.RegexFactorAtom;

        public override bool Equals(object obj)
        {
            return obj is RegexFactorAtom other && other.Atom.Equals(Atom);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return Atom.ToString();
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                Atom.GetHashCode());
        }

        private readonly int _hashCode;
    }

    public class RegexFactorIterator : RegexFactor
    {
        public RegexFactorIterator(RegexAtom atom, RegexIterator iterator)
        {
            Atom = atom;
            Iterator = iterator;
            this._hashCode = ComputeHashCode();
        }

        public RegexAtom Atom { get; }
        public RegexIterator Iterator { get; }

        public override RegexNodeType NodeType => RegexNodeType.RegexFactorIterator;

        public override bool Equals(object obj)
        {
            return obj is RegexFactorIterator other && other.Atom.Equals(Atom) && other.Iterator.Equals(Iterator);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
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

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                Atom.GetHashCode(),
                Iterator.GetHashCode());
        }

        private readonly int _hashCode;
    }
}