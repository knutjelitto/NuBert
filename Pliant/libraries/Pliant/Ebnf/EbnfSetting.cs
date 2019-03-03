﻿using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public sealed class EbnfSetting : ValueEqualityBase<EbnfSetting>, IEbnfNode
    {
        public EbnfSetting(EbnfSettingIdentifier settingIdentifier, EbnfQualifiedIdentifier identifier)
            : base((settingIdentifier, identifier))
        {
            SettingIdentifier = settingIdentifier;
            Identifier = identifier;
        }

        public EbnfSettingIdentifier SettingIdentifier { get; }
        public EbnfQualifiedIdentifier Identifier { get; }

        public override bool ThisEquals(EbnfSetting other)
        {
            return SettingIdentifier.Equals(other.SettingIdentifier) &&
                   Identifier.Equals(other.Identifier);
        }
    }
}