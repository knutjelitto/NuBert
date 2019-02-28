namespace Pliant.Builders
{
    public abstract class SettingModel
    {
        protected SettingModel(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}