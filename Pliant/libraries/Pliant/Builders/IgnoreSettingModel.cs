using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class IgnoreSettingModel : SettingModel
    {
        public const string SettingKey = "ignore";

        public IgnoreSettingModel(LexerRuleModel lexerRuleModel)
            : base(SettingKey, lexerRuleModel.Value.TokenType.Id)
        {
        }

        public IgnoreSettingModel(QualifiedName fullyQualifiedName)
            : base(SettingKey, fullyQualifiedName.FullName)
        {
        }
    }
}