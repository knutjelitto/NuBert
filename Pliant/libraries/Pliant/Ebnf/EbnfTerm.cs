using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public class EbnfTerm : EbnfNode
    {
        private readonly int _hashCode;

        public EbnfFactor Factor { get; private set; }
        
        public EbnfTerm(EbnfFactor factor)
        {
            Factor = factor;
            this._hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfTerm;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                Factor.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfTerm term && 
                   term.NodeType == NodeType && 
                   term.Factor.Equals(Factor);
        }

        public override string ToString()
        {
            return Factor.ToString();
        }
    }

    public class EbnfTermConcatenation : EbnfTerm
    {
        private readonly int _hashCode;

        public EbnfTerm Term { get; private set; }

        public EbnfTermConcatenation(EbnfFactor factor, EbnfTerm term)
            : base(factor)
        {
            Term = term;
            this._hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfTermConcatenation;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                Factor.GetHashCode(),
                Term.GetHashCode());
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

            var term = obj as EbnfTermConcatenation;
            if (term == null)
            {
                return false;
            }

            return term.NodeType == NodeType
                && term.Factor.Equals(Factor)
                && term.Term.Equals(Term);
        }

        public override string ToString()
        {
            return $"{Factor} {Term}";
        }
    }
}