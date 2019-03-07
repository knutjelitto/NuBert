using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class IgnoreSettingModel : SettingModel
    {
        public const string SettingKey = ":ignore";

        public IgnoreSettingModel(QualifiedName qualifiedName)
            : base(qualifiedName)
        {
        }
    }
}