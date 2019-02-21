namespace Pliant.Ebnf
{
    public abstract class EbnfBlock : EbnfNode
    {
    }

    public sealed class EbnfBlockRule : EbnfBlock
    {
        public EbnfBlockRule(EbnfRule rule)
        {
            Rule = rule;
        }

        public EbnfRule Rule { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfBlockRule other && Rule.Equals(other.Rule);
        }

        public override int GetHashCode()
        {
            return Rule.GetHashCode();
        }
    }

    public sealed class EbnfBlockSetting : EbnfBlock
    {
        public EbnfBlockSetting(EbnfSetting setting)
        {
            Setting = setting;
        }

        public EbnfSetting Setting { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfBlockSetting other &&
                   other.Setting.Equals(Setting);
        }

        public override int GetHashCode()
        {
            return Setting.GetHashCode();
        }
    }

    public sealed class EbnfBlockLexerRule : EbnfBlock
    {
        public EbnfBlockLexerRule(EbnfLexerRule lexerRule)
        {
            LexerRule = lexerRule;
        }

        public EbnfLexerRule LexerRule { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfBlockLexerRule other &&
                   other.LexerRule.Equals(LexerRule);
        }

        public override int GetHashCode()
        {
            return LexerRule.GetHashCode();
        }
    }
}