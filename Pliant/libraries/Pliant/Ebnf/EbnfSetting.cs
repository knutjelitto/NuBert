using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public sealed class EbnfSetting : ValueEqualityBase<EbnfSetting>, IEbnfNode
    {
        public EbnfSetting(EbnfSettingIdentifier settingIdentifier, EbnfQualifiedIdentifier qualifiedEbnfQualifiedIdentifier)
            : base((settingIdentifier, qualifiedEbnfQualifiedIdentifier).GetHashCode())
        {
            SettingIdentifier = settingIdentifier;
            QualifiedEbnfQualifiedIdentifier = qualifiedEbnfQualifiedIdentifier;
        }

        public EbnfSettingIdentifier SettingIdentifier { get; }
        public EbnfQualifiedIdentifier QualifiedEbnfQualifiedIdentifier { get; }

        public override bool ThisEquals(EbnfSetting other)
        {
            return SettingIdentifier.Equals(other.SettingIdentifier) &&
                   QualifiedEbnfQualifiedIdentifier.Equals(other.QualifiedEbnfQualifiedIdentifier);
        }
    }
}