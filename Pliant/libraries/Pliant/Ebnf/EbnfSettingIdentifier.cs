namespace Pliant.Ebnf
{
    public sealed class EbnfSettingIdentifier : EbnfNode
    {
        public EbnfSettingIdentifier(string value)
        {
            Value = value.StartsWith(":") ? value.Substring(1) : value;
        }

        public string Value { get; }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfSettingIdentifier other && 
                   other.Value.Equals(Value);
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}