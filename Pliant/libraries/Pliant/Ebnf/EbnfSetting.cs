namespace Pliant.Ebnf
{
    public class EbnfSetting : EbnfNode
    {
        public EbnfSetting(EbnfSettingIdentifier settingIdentifier, EbnfQualifiedIdentifier qualifiedIdentifier)
        {
            SettingIdentifier = settingIdentifier;
            QualifiedIdentifier = qualifiedIdentifier;
        }

        public EbnfSettingIdentifier SettingIdentifier { get; }
        public EbnfQualifiedIdentifier QualifiedIdentifier { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfSetting other && 
                   other.SettingIdentifier.Equals(SettingIdentifier) && 
                   other.QualifiedIdentifier.Equals(QualifiedIdentifier);
        }

        public override int GetHashCode()
        {
            return (SettingIdentifier, QualifiedIdentifier).GetHashCode();
        }
    }
}