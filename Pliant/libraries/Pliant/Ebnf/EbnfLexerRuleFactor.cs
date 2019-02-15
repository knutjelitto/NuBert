using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public abstract class EbnfLexerRuleFactor : EbnfNode
    {     
    }

    public class EbnfLexerRuleFactorLiteral : EbnfLexerRuleFactor
    {
        public override EbnfNodeType NodeType => EbnfNodeType.EbnfLexerRuleFactorLiteral;

        public string Value { get; private set; }

        private readonly int _hashCode;

        public EbnfLexerRuleFactorLiteral(string value)
        {
            Value = value;
            this._hashCode = ComputeHashCode();
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(NodeType.GetHashCode(), Value.GetHashCode());
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

            var factor = obj as EbnfLexerRuleFactorLiteral;
            if (factor == null)
            {
                return false;
            }

            return factor.NodeType == NodeType
                && factor.Value.Equals(Value);
        }
    }

    public class EbnfLexerRuleFactorRegex : EbnfLexerRuleFactor
    {
        public override EbnfNodeType NodeType => EbnfNodeType.EbnfLexerRuleFactorRegex;
        private readonly int _hashCode;

        public RegularExpressions.Regex Regex { get; private set; }

        public EbnfLexerRuleFactorRegex(RegularExpressions.Regex regex)
        {
            Regex = regex;
            this._hashCode = ComputeHashCode();
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                Regex.GetHashCode());
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

            var factor = obj as EbnfFactorRegex;
            if (factor == null)
            {
                return false;
            }

            return factor.NodeType == NodeType
                && factor.Regex.Equals(Regex);
        }
    }
}