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
            : base(value.GetHashCode())
        {
            Value = value;
        }

        public string Value { get; }

        public override bool ThisEquals(EbnfLexerRuleFactorLiteral other)
        {
            return Value.Equals(other.Value);
        }
    }

    public sealed class EbnfLexerRuleFactorRegex : ValueEqualityBase<EbnfLexerRuleFactorRegex>, IEbnfLexerRuleFactor
    {
        public EbnfLexerRuleFactorRegex(Regex regex)
            : base(regex.GetHashCode())
        {
            Regex = regex;
        }

        public Regex Regex { get; }

        public override bool ThisEquals(EbnfLexerRuleFactorRegex other)
        {
            return other.Regex.Equals(Regex);
        }
    }
}