using System;
using Pliant.Automata;
using Pliant.Grammars;

namespace Pliant.RegularExpressions
{
    public class ThompsonConstructionAlgorithm : IRegexToNfa
    {
        public Nfa Transform(Regex regex)
        {
            return Expression(regex.Expression);
        }

        private static Nfa Any()
        {
            var start = new NfaState();
            var end = new NfaState();
            start.AddTransistion(new TerminalNfaTransition(new AnyTerminal(), end));
            return new Nfa(start, end);
        }

        private static Nfa Atom(RegexAtom atom)
        {
            switch (atom.NodeType)
            {
                case RegexNodeType.RegexAtomAny:
                    return Any();

                case RegexNodeType.RegexAtomCharacter:
                    var regexAtomCharacter = atom as RegexAtomCharacter;
                    return Character(regexAtomCharacter.Character);

                case RegexNodeType.RegexAtomExpression:
                    var regexAtomExpression = atom as RegexAtomExpression;
                    return Expression(regexAtomExpression.Expression);

                case RegexNodeType.RegexAtomSet:
                    var regexAtomSet = atom as RegexAtomSet;
                    return Set(regexAtomSet);
            }

            throw new InvalidOperationException("Unrecognized regex atom");
        }

        private static Nfa Character(RegexCharacterClassCharacter character, bool negate)
        {
            var start = new NfaState();
            var end = new NfaState();
            var terminal = CreateTerminalForCharacter(character.Value, character.IsEscaped, negate);

            var transition = new TerminalNfaTransition(
                terminal,
                end);

            start.AddTransistion(transition);

            return new Nfa(start, end);
        }

        private static Nfa Character(RegexCharacter character)
        {
            var start = new NfaState();
            var end = new NfaState();

            var terminal = CreateTerminalForCharacter(character.Value, character.IsEscaped, false);

            var transition = new TerminalNfaTransition(
                terminal,
                end);

            start.AddTransistion(transition);

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
            first.End.AddTransistion(new NullNfaTransition(second.Start));
            return new Nfa(first.Start, second.End);
        }

        private static Terminal CreateTerminalForCharacter(char value, bool isEscaped, bool negate)
        {
            Terminal terminal = null;
            if (!isEscaped)
            {
                terminal = new CharacterTerminal(value);
            }
            else
            {
                switch (value)
                {
                    case 's':
                        terminal = new WhitespaceTerminal();
                        break;
                    case 'd':
                        terminal = new DigitTerminal();
                        break;
                    case 'w':
                        terminal = new WordTerminal();
                        break;
                    case 'D':
                        terminal = new DigitTerminal();
                        negate = !negate;
                        break;
                    case 'S':
                        terminal = new WhitespaceTerminal();
                        negate = !negate;
                        break;
                    case 'W':
                        terminal = new WordTerminal();
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
            start.AddTransistion(new NullNfaTransition(end));
            return new Nfa(start, end);
        }

        private static Nfa Expression(RegexExpression expression)
        {
            switch (expression.NodeType)
            {
                case RegexNodeType.RegexExpression:
                    return Empty();

                case RegexNodeType.RegexExpressionAlteration:
                    var regexExpressionAlteration = expression as RegexExpressionAlteration;

                    var termNfa = Term(regexExpressionAlteration.Term);
                    var expressionNfa = Expression(regexExpressionAlteration.Expression);

                    return Union(termNfa, expressionNfa);

                case RegexNodeType.RegexExpressionTerm:
                    var regexExpressionTerm = expression as RegexExpressionTerm;
                    return Term(regexExpressionTerm.Term);
            }

            throw new InvalidOperationException("Unrecognized Regex Expression");
        }

        private static Nfa Factor(RegexFactor factor)
        {
            var atomNfa = Atom(factor.Atom);
            switch (factor.NodeType)
            {
                case RegexNodeType.RegexFactor:
                    return atomNfa;

                case RegexNodeType.RegexFactorIterator:
                    var regexFactorIterator = factor as RegexFactorIterator;
                    switch (regexFactorIterator.Iterator)
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
            nfa.End.AddTransistion(new NullNfaTransition(end));
            nfa.End.AddTransistion(new NullNfaTransition(nfa.Start));
            return new Nfa(nfa.Start, end);
        }

        private static Nfa KleeneStar(Nfa nfa)
        {
            var start = new NfaState();
            var nullToNfaStart = new NullNfaTransition(nfa.Start);

            start.AddTransistion(nullToNfaStart);
            nfa.End.AddTransistion(nullToNfaStart);

            var end = new NfaState();
            var nullToNewEnd = new NullNfaTransition(end);

            start.AddTransistion(nullToNewEnd);
            nfa.End.AddTransistion(nullToNewEnd);

            return new Nfa(start, end);
        }

        private static Nfa Optional(Nfa nfa)
        {
            var start = new NfaState();
            var end = new NfaState();
            start.AddTransistion(new NullNfaTransition(nfa.Start));
            start.AddTransistion(new NullNfaTransition(end));
            nfa.End.AddTransistion(new NullNfaTransition(end));
            return new Nfa(start, end);
        }

        private static Nfa Range(RegexCharacterRange range, bool negate)
        {
            // combine characters into a character range terminal
            var start = range.StartCharacter.Value;
            var end = range.EndCharacter.Value;
            Terminal terminal = new RangeTerminal(start, end);
            var nfaStartState = new NfaState();
            var nfaEndState = new NfaState();
            if (negate)
            {
                terminal = new NegationTerminal(terminal);
            }

            nfaStartState.AddTransistion(
                new TerminalNfaTransition(terminal, nfaEndState));
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

        private static Nfa Term(RegexTerm term)
        {
            switch (term.NodeType)
            {
                case RegexNodeType.RegexTerm:
                    return Factor(term.Factor);

                case RegexNodeType.RegexTermFactor:
                    var regexTermFactor = term as RegexTermFactor;
                    var factorNfa = Factor(regexTermFactor.Factor);
                    var termNfa = Term(regexTermFactor.Term);
                    return Concatenation(factorNfa, termNfa);
            }

            throw new InvalidOperationException("Unrecognized Regex Term");
        }

        private static Nfa Union(Nfa first, Nfa second)
        {
            var start = new NfaState();
            start.AddTransistion(new NullNfaTransition(first.Start));
            start.AddTransistion(new NullNfaTransition(second.Start));

            var end = new NfaState();
            var endTransition = new NullNfaTransition(end);
            first.End.AddTransistion(endTransition);
            second.End.AddTransistion(endTransition);

            return new Nfa(start, end);
        }

        private static Nfa UnitRange(RegexCharacterUnitRange unitRange, bool negate)
        {
            switch (unitRange.NodeType)
            {
                case RegexNodeType.RegexCharacterUnitRange:
                    return Character(unitRange.StartCharacter, negate);

                case RegexNodeType.RegexCharacterRange:
                    var range = unitRange as RegexCharacterRange;
                    return Range(range, negate);
            }

            throw new InvalidOperationException("Unreachable code detected.");
        }
    }
}