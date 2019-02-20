using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class TriviaSettingModel : SettingModel
    {
        public const string SettingKey = "trivia";

        public TriviaSettingModel(LexerRuleModel lexerRuleModel)
            : base(SettingKey, lexerRuleModel.Value.TokenType.Id)
        {
        }

        public TriviaSettingModel(QualifiedName fullyQualifiedName)
            : base(SettingKey, fullyQualifiedName.FullName)
        {
        }
    }
}