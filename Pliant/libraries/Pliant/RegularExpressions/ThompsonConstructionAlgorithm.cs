using System;
using Pliant.Automata;
using Pliant.Terminals;

namespace Pliant.RegularExpressions
{
    public class ThompsonConstructionAlgorithm : RegexToNfa
    {
        public override Nfa Transform(Regex regex)
        {
            return Expression(regex.Expression);
        }

        private static Nfa Any()
        {
            var start = new NfaState();
            var end = new NfaState();
            start.AddTransition(AnyTerminal.Instance, end);
            return new Nfa(start, end);
        }

        private static Nfa Atom(RegexAtom atom)
        {
            switch (atom)
            {
                case RegexAtomAny _:
                    return Any();

                case RegexAtomCharacter regexAtomCharacter:
                    return Character(regexAtomCharacter.Character);

                case RegexAtomExpression regexAtomExpression:
                    return Expression(regexAtomExpression.Expression);

                case RegexAtomSet regexAtomSet:
                    return Set(regexAtomSet);
            }

            throw new InvalidOperationException("Unrecognized regex atom");
        }

        private static Nfa Character(RegexCharacterClassCharacter character, bool negate)
        {
            var start = new NfaState();
            var end = new NfaState();
            var terminal = CreateTerminalForCharacter(character.Value, character.IsEscaped, negate);

            start.AddTransition(terminal, end);

            return new Nfa(start, end);
        }

        private static Nfa Character(RegexCharacter character)
        {
            var start = new NfaState();
            var end = new NfaState();

            var terminal = CreateTerminalForCharacter(character.Value, character.IsEscaped, false);

            start.AddTransition(terminal, end);

            return new Nfa(start, end);
        }

        private static Nfa CharacterClass(RegexCharacterClass characterClass, bool negate)
        {
            switch (characterClass)
            {
                case RegexCharacterClassAlteration alteration:
                    return Union(
                        UnitRange(alteration.CharacterRange, negate),
                        CharacterClass(alteration.CharacterClass, negate));

                case RegexCharacterClass _:
                    return UnitRange(characterClass.CharacterRange, negate);
            }

            throw new InvalidOperationException("Unreachable code detected.");
        }

        private static Nfa Concatenation(Nfa first, Nfa second)
        {
            first.End.AddEpsilon(second.Start);
            return new Nfa(first.Start, second.End);
        }

        private static AtomTerminal CreateTerminalForCharacter(char value, bool isEscaped, bool negate)
        {
            AtomTerminal terminal;
            if (!isEscaped)
            {
                terminal = new CharacterTerminal(value);
            }
            else
            {
                switch (value)
                {
                    case 's':
                        terminal = WhitespaceTerminal.Instance;
                        break;
                    case 'd':
                        terminal = DigitTerminal.Instance;
                        break;
                    case 'w':
                        terminal = WordTerminal.Instance;
                        break;
                    case 'D':
                        terminal = DigitTerminal.Instance;
                        negate = !negate;
                        break;
                    case 'S':
                        terminal = WhitespaceTerminal.Instance;
                        negate = !negate;
                        break;
                    case 'W':
                        terminal = WordTerminal.Instance;
                        negate = !negate;
                        break;
                    default:
                        terminal = new CharacterTerminal(value);
                        break;
                }
            }

            if (negate)
            {
                terminal = new NegationTerminal(terminal);
            }

            return terminal;
        }

        private static Nfa Empty()
        {
            var start = new NfaState();
            var end = new NfaState();
            start.AddEpsilon(end);
            return new Nfa(start, end);
        }

        private static Nfa Expression(RegexExpression expression)
        {
            switch (expression)
            {
                case RegexExpressionAlteration alteration:
                    var termNfa = Term(alteration.Term);
                    var expressionNfa = Expression(alteration.Expression);
                    return Union(termNfa, expressionNfa);

                case RegexExpressionTerm term:
                    return Term(term.Term);

                case RegexExpression _:
                    return Empty();
            }

            throw new InvalidOperationException("Unrecognized Regex Expression");
        }

        private static Nfa Factor(IRegexFactor factorAtom)
        {
            switch (factorAtom)
            {
                case RegexFactorAtom atom:
                    return Atom(atom.Atom);

                case RegexFactorIterator iterator:
                    var atomNfa = Atom(iterator.Atom);
                    switch (iterator.Iterator)
                    {
                        case RegexIterator.ZeroOrMany:
                            return KleeneStar(atomNfa);
                        case RegexIterator.OneOrMany:
                            return KleenePlus(atomNfa);
                        case RegexIterator.ZeroOrOne:
                            return Optional(atomNfa);
                    }

                    break;
            }

            throw new InvalidOperationException("Unrecognized regex factor");
        }

        private static Nfa KleenePlus(Nfa nfa)
        {
            var end = new NfaState();
            nfa.End.AddEpsilon(end);
            nfa.End.AddEpsilon(nfa.Start);
            return new Nfa(nfa.Start, end);
        }

        private static Nfa KleeneStar(Nfa nfa)
        {
            var start = new NfaState();

            start.AddEpsilon(nfa.Start);
            nfa.End.AddEpsilon(nfa.Start);

            var end = new NfaState();

            start.AddEpsilon(end);
            nfa.End.AddEpsilon(end);

            return new Nfa(start, end);
        }

        private static Nfa Optional(Nfa nfa)
        {
            var start = new NfaState();
            var end = new NfaState();
            start.AddEpsilon(nfa.Start);
            start.AddEpsilon(end);
            nfa.End.AddEpsilon(end);
            return new Nfa(start, end);
        }

        private static Nfa Range(RegexCharactersRange range, bool negate)
        {
            // combine characters into a character range terminal
            var start = range.StartCharacter.Value;
            var end = range.EndCharacter.Value;
            AtomTerminal terminal = new RangeTerminal(start, end);
            var nfaStartState = new NfaState();
            var nfaEndState = new NfaState();
            if (negate)
            {
                terminal = new NegationTerminal(terminal);
            }

            nfaStartState.AddTransition(terminal, nfaEndState);
            return new Nfa(nfaStartState, nfaEndState);
        }

        private static Nfa Set(RegexAtomSet atomSet)
        {
            return Set(atomSet.Set);
        }

        private static Nfa Set(RegexSet set)
        {
            return CharacterClass(set.CharacterClass, set.Negate);
        }

        private static Nfa Term(IRegexTerm termX)
        {
            switch (termX)
            {
                case RegexTermFactor termFactor:
                    return Factor(termFactor.Factor);

                case RegexTermFactorTerm regexTermFactor:
                    var factorNfa = Factor(regexTermFactor.Factor);
                    var termNfa = Term(regexTermFactor.Term);
                    return Concatenation(factorNfa, termNfa);
            }

            throw new InvalidOperationException("Unrecognized Regex Term");
        }

        private static Nfa Union(Nfa first, Nfa second)
        {
            var start = new NfaState();
            var end = new NfaState();
            start.AddEpsilon(first.Start);
            start.AddEpsilon(second.Start);

            first.End.AddEpsilon(end);
            second.End.AddEpsilon(end);

            return new Nfa(start, end);
        }

        private static Nfa UnitRange(RegexCharacters unitRange, bool negate)
        {
            switch (unitRange)
            {
                case RegexCharactersUnit unit:
                    return Character(unit.Character, negate);

                case RegexCharactersRange range:
                    return Range(range, negate);
            }

            throw new InvalidOperationException("Unreachable code detected.");
        }
    }
}