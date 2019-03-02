using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class TriviaSettingModel : SettingModel
    {
        public const string SettingKey = ":trivia";

        public TriviaSettingModel(LexerRuleModel lexerRuleModel)
            : base(lexerRuleModel.LexerRule.TokenType.Id)
        {
        }

        public TriviaSettingModel(QualifiedName fullyQualifiedName)
            : base(fullyQualifiedName.FullName)
        {
        }
    }
}