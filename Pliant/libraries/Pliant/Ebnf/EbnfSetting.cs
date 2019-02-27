namespace Pliant.Ebnf
{
    public class EbnfSetting : EbnfNode
    {
        public EbnfSetting(EbnfSettingIdentifier settingIdentifier, EbnfQualifiedIdentifier qualifiedEbnfQualifiedIdentifier)
        {
            SettingIdentifier = settingIdentifier;
            QualifiedEbnfQualifiedIdentifier = qualifiedEbnfQualifiedIdentifier;
        }

        public EbnfSettingIdentifier SettingIdentifier { get; }
        public EbnfQualifiedIdentifier QualifiedEbnfQualifiedIdentifier { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfSetting other && 
                   other.SettingIdentifier.Equals(SettingIdentifier) && 
                   other.QualifiedEbnfQualifiedIdentifier.Equals(QualifiedEbnfQualifiedIdentifier);
        }

        public override int GetHashCode()
        {
            return (SettingIdentifier, QualifiedIdentifier: QualifiedEbnfQualifiedIdentifier).GetHashCode();
        }
    }
}