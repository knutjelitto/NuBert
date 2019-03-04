using Pliant.RegularExpressions;
using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public interface IEbnfLexerRuleFactor : IEbnfNode
    {
    }

    public sealed class EbnfLexerRuleFactorLiteral : ValueEqualityBase<EbnfLexerRuleFactorLiteral>, IEbnfLexerRuleFactor
    {
        public EbnfLexerRuleFactorLiteral(string value)
        {
            Value = value;
        }

        public string Value { get; }

        protected override bool ThisEquals(EbnfLexerRuleFactorLiteral other)
        {
            return Value.Equals(other.Value);
        }

        protected override object ThisHashCode => Value;
    }

    public sealed class EbnfLexerRuleFactorRegex : ValueEqualityBase<EbnfLexerRuleFactorRegex>, IEbnfLexerRuleFactor
    {
        public EbnfLexerRuleFactorRegex(Regex regex)
        {
            Regex = regex;
        }

        public Regex Regex { get; }

        protected override bool ThisEquals(EbnfLexerRuleFactorRegex other)
        {
            return other.Regex.Equals(Regex);
        }

        protected override object ThisHashCode => Regex;
    }
}