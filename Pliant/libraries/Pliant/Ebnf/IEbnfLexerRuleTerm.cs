using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public interface IEbnfLexerRuleTerm : IEbnfNode
    {
        IEbnfLexerRuleFactor Factor { get; }
    }

    public sealed class EbnfLexerRuleTermSimple : ValueEqualityBase<EbnfLexerRuleTermSimple>, IEbnfLexerRuleTerm
    {
        public IEbnfLexerRuleFactor Factor { get; }

        public EbnfLexerRuleTermSimple(IEbnfLexerRuleFactor factor)
            : base(factor.GetHashCode())
        {
            Factor = factor;
        }

        public override bool ThisEquals(EbnfLexerRuleTermSimple other)
        {
            return Factor.Equals(other.Factor);
        }
    }

    public sealed class EbnfLexerRuleTermConcatenation : ValueEqualityBase<EbnfLexerRuleTermConcatenation>, IEbnfLexerRuleTerm
    {
        public EbnfLexerRuleTermConcatenation(IEbnfLexerRuleFactor factor, IEbnfLexerRuleTerm term)
            : base((factor, term).GetHashCode())
        {
            Factor = factor;
            Term = term;
        }

        public IEbnfLexerRuleFactor Factor { get; }
        public IEbnfLexerRuleTerm Term { get; }

        public override bool ThisEquals(EbnfLexerRuleTermConcatenation other)
        {
            return Factor.Equals(other.Factor) && Term.Equals(other.Term);
        }
    }
}