using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public interface IEbnfBlock
    {
    }

    public sealed class EbnfBlockRule : ValueEqualityBase<EbnfBlockRule>, IEbnfBlock
    {
        public EbnfBlockRule(EbnfRule rule) 
        {
            Rule = rule;
        }

        public EbnfRule Rule { get; }

        protected override bool ThisEquals(EbnfBlockRule other)
        {
            return Rule.Equals(other.Rule);
        }

        protected override object ThisHashCode => Rule;

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
        {
            Setting = setting;
        }

        public EbnfSetting Setting { get; }

        protected override bool ThisEquals(EbnfBlockSetting other)
        {
            return other.Setting.Equals(Setting);
        }

        protected override object ThisHashCode => Setting;
    }

    public sealed class EbnfBlockLexerRule : ValueEqualityBase<EbnfBlockLexerRule>, IEbnfBlock
    {
        public EbnfBlockLexerRule(EbnfLexerRule lexerRule)
        {
            LexerRule = lexerRule;
        }

        public EbnfLexerRule LexerRule { get; }

        protected override bool ThisEquals(EbnfBlockLexerRule other)
        {
            return other.LexerRule.Equals(LexerRule);
        }

        protected override object ThisHashCode => LexerRule;
    }
}