using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public class EbnfLexerRuleTerm : EbnfNode
    {
        public EbnfLexerRuleFactor Factor { get; private set; }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfLexerRuleTerm;

        readonly int _hashCode;

        public EbnfLexerRuleTerm(EbnfLexerRuleFactor factor)
        {
            Factor = factor;
            this._hashCode = ComputeHashCode();
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(NodeType.GetHashCode(), Factor.GetHashCode());
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

            var term = obj as EbnfLexerRuleTerm;
            if (term == null)
            {
                return false;
            }

            return term.NodeType == NodeType
                && term.Factor.Equals(Factor);
        }
    }

    public class EbnfLexerRuleTermConcatenation : EbnfLexerRuleTerm
    {
        public EbnfLexerRuleTerm Term { get; private set; }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfLexerRuleTermConcatenation;

        readonly int _hashCode;

        public EbnfLexerRuleTermConcatenation(EbnfLexerRuleFactor factor, EbnfLexerRuleTerm term)
            : base(factor)
        {
            Term = term;
            this._hashCode = ComputeHashCode();
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(NodeType.GetHashCode(), Factor.GetHashCode(), Term.GetHashCode());
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

            var term = obj as EbnfLexerRuleTermConcatenation;
            if (term == null)
            {
                return false;
            }

            return term.NodeType == NodeType
                && term.Factor.Equals(Factor)
                && term.Term.Equals(Term);
        }
    }
}