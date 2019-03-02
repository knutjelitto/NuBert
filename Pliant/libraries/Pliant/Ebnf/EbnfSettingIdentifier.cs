using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public sealed class EbnfSettingIdentifier : ValueEqualityBase<EbnfSettingIdentifier>, IEbnfNode
    {
        public EbnfSettingIdentifier(string value)
            : base(value.GetHashCode())
        {
            Value = value;
        }

        public string Value { get; }

        public override bool ThisEquals(EbnfSettingIdentifier other)
        {
            return Value.Equals(other.Value);
        }

        public override string ToString()
        {
            return $":{Value}";
        }
    }
}