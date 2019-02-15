using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public abstract class EbnfBlock : EbnfNode
    {
    }

    public class EbnfBlockRule : EbnfBlock
    {
        private readonly int _hashCode;

        public EbnfRule Rule { get; private set; }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfBlockRule;

        public EbnfBlockRule(EbnfRule rule)
        {
            Rule = rule;
            this._hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var blockRule = obj as EbnfBlockRule;
            if (blockRule == null)
            {
                return false;
            }

            return blockRule.NodeType == NodeType
                && blockRule.Rule.Equals(Rule);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                Rule.GetHashCode(), 
                NodeType.GetHashCode());
        }
                
        public override int GetHashCode()
        {
            return this._hashCode;
        }
    }

    public class EbnfBlockSetting : EbnfBlock
    {
        private readonly int _hashCode;
        public EbnfSetting Setting { get; private set; }
        public override EbnfNodeType NodeType => EbnfNodeType.EbnfBlockSetting;

        public EbnfBlockSetting(EbnfSetting setting)
        {
            Setting = setting;
            this._hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var blockRule = obj as EbnfBlockSetting;
            if (blockRule == null)
            {
                return false;
            }

            return blockRule.NodeType == NodeType
                && blockRule.Setting.Equals(Setting);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                Setting.GetHashCode(),
                NodeType.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }
    }

    public class EbnfBlockLexerRule : EbnfBlock
    {
        private readonly int _hashCode;
        public EbnfLexerRule LexerRule { get; private set; }
        public override EbnfNodeType NodeType => EbnfNodeType.EbnfBlockLexerRule;

        public EbnfBlockLexerRule(EbnfLexerRule lexerRule)
        {
            LexerRule = lexerRule;
            this._hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var blockRule = obj as EbnfBlockLexerRule;
            if (blockRule == null)
            {
                return false;
            }

            return blockRule.NodeType == NodeType
                && blockRule.LexerRule.Equals(LexerRule);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                LexerRule.GetHashCode(),
                NodeType.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }
    }
}
