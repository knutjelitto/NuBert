using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class IgnoreSettingModel : SettingModel
    {
        public const string SettingKey = "ignore";

        public IgnoreSettingModel(LexerRuleModel lexerRuleModel)
            : base(lexerRuleModel.LexerRule.TokenType.Id)
        {
        }

        public IgnoreSettingModel(QualifiedName fullyQualifiedName)
            : base(fullyQualifiedName.FullName)
        {
        }
    }
}