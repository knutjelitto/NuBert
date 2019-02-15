namespace Pliant.Builders
{
    public abstract class SettingModel
    {
        protected SettingModel(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public string Value { get; }
    }
}