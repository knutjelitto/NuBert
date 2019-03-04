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
        {
            Factor = factor;
        }

        protected override bool ThisEquals(EbnfLexerRuleTermSimple other)
        {
            return Factor.Equals(other.Factor);
        }

        protected override object ThisHashCode => Factor;
    }

    public sealed class EbnfLexerRuleTermConcatenation : ValueEqualityBase<EbnfLexerRuleTermConcatenation>, IEbnfLexerRuleTerm
    {
        public EbnfLexerRuleTermConcatenation(IEbnfLexerRuleFactor factor, IEbnfLexerRuleTerm term)
        {
            Factor = factor;
            Term = term;
        }

        public IEbnfLexerRuleFactor Factor { get; }
        public IEbnfLexerRuleTerm Term { get; }

        protected override bool ThisEquals(EbnfLexerRuleTermConcatenation other)
        {
            return Factor.Equals(other.Factor) && Term.Equals(other.Term);
        }

        protected override object ThisHashCode => (Factor, Term);
    }
}