﻿namespace Pliant.RegularExpressions
{
    public enum RegexNodeType
    {
        Regex,
        RegexAtom,
        RegexAtomAny,
        RegexAtomCharacter,
        RegexAtomExpression,
        RegexAtomSet,
        RegexCharacter,
        RegexCharacterClass,
        RegexCharacterClassAlteration,
        RegexCharacterClassCharacter,
        RegexCharacterUnitRange,
        RegexCharacterRange,
        RegexExpression,
        RegexExpressionTerm,
        RegexExpressionAlteration,
        RegexFactorAtom,
        RegexFactorIterator,
        RegexSet,
        RegexTerm,
        RegexTermFactor
    }
}