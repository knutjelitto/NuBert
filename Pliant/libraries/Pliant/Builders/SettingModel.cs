using Pliant.Grammars;

namespace Pliant.Builders
{
    public abstract class SettingModel
    {
        protected SettingModel(QualifiedName value)
        {
            Value = value;
        }

        public QualifiedName Value { get; }
    }
}