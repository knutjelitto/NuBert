using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class TriviaSettingModel : SettingModel
    {
        public const string SettingKey = ":trivia";

        public TriviaSettingModel(QualifiedName fullyQualifiedName)
            : base(fullyQualifiedName)
        {
        }
    }
}