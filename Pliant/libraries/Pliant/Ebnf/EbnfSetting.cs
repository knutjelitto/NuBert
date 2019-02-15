using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public class EbnfSetting : EbnfNode
    {
        private readonly int _hashCode;

        public EbnfSettingIdentifier SettingIdentifier { get; private set; }
        public EbnfQualifiedIdentifier QualifiedIdentifier { get; private set; }
        
        public override EbnfNodeType NodeType => EbnfNodeType.EbnfSetting;

        public EbnfSetting(EbnfSettingIdentifier settingIdentifier, EbnfQualifiedIdentifier qualifiedIdentifier)
        {
            SettingIdentifier = settingIdentifier;
            QualifiedIdentifier = qualifiedIdentifier;
            this._hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var qualifiedIdentifier = obj as EbnfSetting;
            if (qualifiedIdentifier == null)
            {
                return false;
            }

            return qualifiedIdentifier.NodeType == NodeType
                && qualifiedIdentifier.SettingIdentifier.Equals(SettingIdentifier)
                && qualifiedIdentifier.QualifiedIdentifier.Equals(QualifiedIdentifier);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                SettingIdentifier.GetHashCode(),
                QualifiedIdentifier.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

    }
}