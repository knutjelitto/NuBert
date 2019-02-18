using Pliant.RegularExpressions;

namespace Pliant.Ebnf
{
    public abstract class EbnfLexerRuleFactor : EbnfNode
    {
    }

    public sealed class EbnfLexerRuleFactorLiteral : EbnfLexerRuleFactor
    {
        public EbnfLexerRuleFactorLiteral(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfLexerRuleFactorLiteral factor && 
                   factor.Value.Equals(Value);
        }
    }

    public sealed class EbnfLexerRuleFactorRegex : EbnfLexerRuleFactor
    {
        public EbnfLexerRuleFactorRegex(Regex regex)
        {
            Regex = regex;
        }

        public Regex Regex { get; }

        public override int GetHashCode()
        {
            return Regex.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfFactorRegex factor && 
                   factor.Regex.Equals(Regex);
        }
    }
}