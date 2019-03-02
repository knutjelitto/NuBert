using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public interface IEbnfBlock
    {
    }

    public sealed class EbnfBlockRule : ValueEqualityBase<EbnfBlockRule>, IEbnfBlock
    {
        public EbnfBlockRule(EbnfRule rule) 
            : base(rule.GetHashCode())
        {
            Rule = rule;
        }

        public EbnfRule Rule { get; }

        public override bool ThisEquals(EbnfBlockRule other)
        {
            return Rule.Equals(other.Rule);
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfBlockRule other && Rule.Equals(other.Rule);
        }

        public override int GetHashCode()
        {
            return Rule.GetHashCode();
        }
    }

    public sealed class EbnfBlockSetting : ValueEqualityBase<EbnfBlockSetting>, IEbnfBlock
    {
        public EbnfBlockSetting(EbnfSetting setting)
            : base(setting.GetHashCode())
        {
            Setting = setting;
        }

        public EbnfSetting Setting { get; }

        public override bool ThisEquals(EbnfBlockSetting other)
        {
            return other.Setting.Equals(Setting);
        }
    }

    public sealed class EbnfBlockLexerRule : ValueEqualityBase<EbnfBlockLexerRule>, IEbnfBlock
    {
        public EbnfBlockLexerRule(EbnfLexerRule lexerRule)
            : base(lexerRule.GetHashCode())
        {
            LexerRule = lexerRule;
        }

        public EbnfLexerRule LexerRule { get; }

        public override bool ThisEquals(EbnfBlockLexerRule other)
        {
            return other.LexerRule.Equals(LexerRule);
        }
    }
}