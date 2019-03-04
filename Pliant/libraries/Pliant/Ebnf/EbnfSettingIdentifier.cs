using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public sealed class EbnfSettingIdentifier : ValueEqualityBase<EbnfSettingIdentifier>, IEbnfNode
    {
        public EbnfSettingIdentifier(string value)
        {
            Value = value;
        }

        public string Value { get; }

        protected override bool ThisEquals(EbnfSettingIdentifier other)
        {
            return Value.Equals(other.Value);
        }

        protected override object ThisHashCode => Value;

        public override string ToString()
        {
            return $":{Value}";
        }
    }
}