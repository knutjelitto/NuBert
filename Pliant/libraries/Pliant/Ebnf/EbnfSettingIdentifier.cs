namespace Pliant.Ebnf
{
    public class EbnfSettingIdentifier : EbnfNode
    {
        public EbnfSettingIdentifier(string value)
        {
            Value = value;
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
    }
}