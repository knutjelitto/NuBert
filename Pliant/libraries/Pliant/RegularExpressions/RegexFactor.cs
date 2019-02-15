using Pliant.Utilities;
using System;

namespace Pliant.RegularExpressions
{
    public class RegexFactor : RegexNode
    {
        public RegexAtom Atom { get; private set; }

        public RegexFactor(RegexAtom atom)
        {
            Atom = atom;
            this._hashCode = ComputeHashCode();
        }
        
        private readonly int _hashCode;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                    Atom.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var factor = obj as RegexFactor;
            if (factor == null)
            {
                return false;
            }

            return factor.Atom.Equals(Atom);
        }

        public override RegexNodeType NodeType => RegexNodeType.RegexFactor;

        public override string ToString()
        {
            return Atom.ToString();
        }
    }

    public class RegexFactorIterator : RegexFactor
    {
        public RegexIterator Iterator { get; private set; }

        public RegexFactorIterator(RegexAtom atom, RegexIterator iterator)
            : base(atom)
        {
            Iterator = iterator;
            this._hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var factor = obj as RegexFactor;
            if (factor == null)
            {
                return false;
            }

            return factor.Atom.Equals(Atom);
        }
        
        private readonly int _hashCode ;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                Atom.GetHashCode(),
                Iterator.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override RegexNodeType NodeType => RegexNodeType.RegexFactorIterator;

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