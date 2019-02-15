using Pliant.Utilities;

namespace Pliant.RegularExpressions
{
    public class RegexTerm : RegexNode
    {
        public RegexFactor Factor { get; private set; }

        public override RegexNodeType NodeType => RegexNodeType.RegexTerm;

        public RegexTerm(RegexFactor factor)
        {
            Factor = factor;
            this._hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var term = obj as RegexTerm;
            if (term == null)
            {
                return false;
            }

            return term.Factor.Equals(Factor);
        }
        
        private readonly int _hashCode;
        
        int ComputeHashCode()
        {
            return HashCode.Compute(Factor.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return Factor.ToString();
        }
    }

    public class RegexTermFactor : RegexTerm
    {
        public RegexTerm Term { get; private set; }

        public RegexTermFactor(RegexFactor factor, RegexTerm term)
            : base(factor)
        {
            Term = term;
            this._hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var termFactor = obj as RegexTermFactor;
            if (termFactor == null)
            {
                return false;
            }

            return termFactor.Factor.Equals(Factor)
                && termFactor.Term.Equals(Term);
        }
        
        private readonly int _hashCode;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                    Term.GetHashCode(),
                    Factor.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override RegexNodeType NodeType => RegexNodeType.RegexTermFactor;

        public override string ToString()
        {
            return $"{Factor}{Term}";
        }
    }
}