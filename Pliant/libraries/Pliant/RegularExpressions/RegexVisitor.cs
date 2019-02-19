using System;
using Pliant.Tree;

namespace Pliant.RegularExpressions
{
    public class RegexVisitor : TreeNodeVisitorBase
    {
        public Regex Regex { get; private set; }

        public override void Visit(IInternalTreeNode node)
        {
            if (RegexGrammar.Regex == node.Symbol.Value)
            {
                Regex = VisitRegexNode(node);
            }
        }

        private static RegexCharacterClassCharacter VisitCharacterClassCharacterNode(IInternalTreeNode internalNode)
        {
            foreach (var child in internalNode.Children)
            {
                if (child is ITokenTreeNode childTokenNode)
                {
                    var tokenValue = childTokenNode.Token.Value;
                    var isEscaped = tokenValue.StartsWith(@"\", StringComparison.CurrentCulture);

                    var value = isEscaped
                                    ? tokenValue[1]
                                    : tokenValue[0];

                    return new RegexCharacterClassCharacter(value, isEscaped);
                }
            }

            throw new Exception("Invalid Regex Character Class Character.");
        }

        private static RegexCharacters VisitCharacterRangeNode(IInternalTreeNode internalNode)
        {
            RegexCharacterClassCharacter start = null;
            RegexCharacterClassCharacter end = null;

            foreach (var child in internalNode.Children)
            {
                if (child is IInternalTreeNode childInternalNode)
                {
                    if (RegexGrammar.CharacterClassCharacter == childInternalNode.Symbol.Value)
                    {
                        if (start == null)
                        {
                            start = VisitCharacterClassCharacterNode(childInternalNode);
                        }
                        else
                        {
                            end = VisitCharacterClassCharacterNode(childInternalNode);
                        }
                    }
                }
            }

            return end == null 
                       ? (RegexCharacters)new RegexCharactersUnit(start)
                       : (RegexCharacters)new RegexCharactersRange(start, end);
        }

        private RegexCharacterClass VisitCharacterClassNode(IInternalTreeNode internalNode)
        {
            RegexCharacters characterRange = null;
            RegexCharacterClass characterClass = null;

            foreach (var child in internalNode.Children)
            {
                if (child is IInternalTreeNode childInternalNode)
                {
                    var childInternalNodeSymbolValue = childInternalNode.Symbol.Value;

                    if (RegexGrammar.CharacterRange == childInternalNodeSymbolValue)
                    {
                        characterRange = VisitCharacterRangeNode(childInternalNode);
                    }
                    else if (RegexGrammar.CharacterClass == childInternalNodeSymbolValue)
                    {
                        characterClass = VisitCharacterClassNode(childInternalNode);
                    }
                }
            }

            return characterClass == null
                       ? new RegexCharacterClass(characterRange)
                       : new RegexCharacterClassAlteration(characterRange, characterClass);
        }

        private RegexSet VisitInnerSetNode(bool negate, IInternalTreeNode internalNode)
        {
            foreach (var child in internalNode.Children)
            {
                if (child is IInternalTreeNode childInternalNode)
                {
                    if (RegexGrammar.CharacterClass == childInternalNode.Symbol.Value)
                    {
                        var characterClass = VisitCharacterClassNode(childInternalNode);
                        return new RegexSet(negate, characterClass);
                    }
                }
            }

            throw new Exception("Invalid Inner Set Detected");
        }

        private RegexAtom VisitRegexAtomNode(IInternalTreeNode internalNode)
        {
            foreach (var child in internalNode.Children)
            {
                if (child is IInternalTreeNode childInternalNode)
                {
                    var childInternalNodeSymbolValue = childInternalNode.Symbol.Value;
                    if (RegexGrammar.Character == childInternalNodeSymbolValue)
                    {
                        var character = VisitRegexCharacterNode(childInternalNode);
                        return new RegexAtomCharacter(character);
                    }

                    if (RegexGrammar.Expression == childInternalNodeSymbolValue)
                    {
                        var expression = VisitRegexExpressionNode(childInternalNode);
                        return new RegexAtomExpression(expression);
                    }

                    if (RegexGrammar.Set == childInternalNodeSymbolValue)
                    {
                        var set = VisitRegexSetNode(childInternalNode);
                        return new RegexAtomSet(set);
                    }
                }
                else if (child is ITokenTreeNode childTokenNode)
                {
                    switch (childTokenNode.Token.Value)
                    {
                        case ".":
                            return new RegexAtomAny();

                        default:
                            continue;
                    }
                }
            }

            throw new Exception("Unable to parse atom. Invalid child production.");
        }

        private RegexCharacter VisitRegexCharacterNode(IInternalTreeNode internalNode)
        {
            foreach (var child in internalNode.Children)
            {
                if (child is ITokenTreeNode childTokenNode)
                {
                    var isEscaped = childTokenNode.Token.Value.StartsWith(@"\", StringComparison.CurrentCulture);
                    var value = isEscaped
                                    ? childTokenNode.Token.Value[1]
                                    : childTokenNode.Token.Value[0];

                    return new RegexCharacter(value, isEscaped);
                }
            }

            throw new Exception("Invalid character detected.");
        }

        private RegexExpression VisitRegexExpressionNode(IInternalTreeNode internalNode)
        {
            RegexExpression expression = null;
            RegexTerm term = null;

            foreach (var child in internalNode.Children)
            {
                if (child is IInternalTreeNode childInternalNode)
                {
                    var childInternalNodeSymbolValue = childInternalNode.Symbol.Value;

                    if (RegexGrammar.Expression == childInternalNodeSymbolValue)
                    {
                        expression = VisitRegexExpressionNode(childInternalNode);
                    }
                    else if (RegexGrammar.Term == childInternalNodeSymbolValue)
                    {
                        term = VisitRegexTermNode(childInternalNode);
                    }
                }
            }

            if (expression != null && term != null)
            {
                return new RegexExpressionAlteration(term, expression);
            }

            if (term != null)
            {
                return new RegexExpressionTerm(term);
            }

            throw new InvalidOperationException("Unable to create null expression.");
        }

        private RegexFactor VisitRegexFactorNode(IInternalTreeNode internalNode)
        {
            RegexAtom atom = null;
            RegexIterator? iterator = null;

            foreach (var child in internalNode.Children)
            {
                if (child is IInternalTreeNode childInternalNode)
                {
                    var childInternalNodeSymbolValue = childInternalNode.Symbol.Value;

                    if (RegexGrammar.Atom == childInternalNodeSymbolValue)
                    {
                        atom = VisitRegexAtomNode(childInternalNode);
                    }
                    else if (RegexGrammar.Iterator == childInternalNodeSymbolValue)
                    {
                        iterator = VisitRegexIteratorNode(childInternalNode);
                    }
                }
            }

            return iterator.HasValue
                       ? (RegexFactor)new RegexFactorIterator(atom, iterator.Value)
                       : (RegexFactor)new RegexFactorAtom(atom);
        }

        private RegexIterator VisitRegexIteratorNode(IInternalTreeNode internalNode)
        {
            foreach (var child in internalNode.Children)
            {
                if (child is ITokenTreeNode tokenChildNode)
                {
                    switch (tokenChildNode.Token.Value)
                    {
                        case "*":
                            return RegexIterator.ZeroOrMany;

                        case "?":
                            return RegexIterator.ZeroOrOne;

                        case "+":
                            return RegexIterator.OneOrMany;
                    }
                }
            }

            throw new Exception("Invalid iterator detected.");
        }

        private Regex VisitRegexNode(IInternalTreeNode node)
        {
            RegexExpression expression = null;
            var startsWith = false;
            var endsWith = false;

            foreach (var child in node.Children)
            {
                if (child is IInternalTreeNode internalNode)
                {
                    var internalNodeSymbolValue = internalNode.Symbol.Value;
                    if (RegexGrammar.Expression == internalNodeSymbolValue)
                    {
                        expression = VisitRegexExpressionNode(internalNode);
                    }
                }
                else if (child is ITokenTreeNode tokenNode)
                {
                    switch (tokenNode.Token.Value)
                    {
                        case "$":
                            endsWith = true;
                            break;

                        case "^":
                            startsWith = true;
                            break;
                    }
                }
            }

            return new Regex(startsWith, expression, endsWith);
        }

        private RegexSet VisitRegexSetNode(IInternalTreeNode internalNode)
        {
            foreach (var child in internalNode.Children)
            {
                if (child is IInternalTreeNode childInternalNode)
                {
                    var childInternalNodeSymbolValue = childInternalNode.Symbol.Value;

                    if (RegexGrammar.PositiveSet == childInternalNodeSymbolValue)
                    {
                        return VisitInnerSetNode(false, childInternalNode);
                    }

                    if (RegexGrammar.NegativeSet == childInternalNodeSymbolValue)
                    {
                        return VisitInnerSetNode(true, childInternalNode);
                    }
                }
            }

            throw new Exception("Invalid Set Detected.");
        }

        private RegexTerm VisitRegexTermNode(IInternalTreeNode internalNode)
        {
            RegexFactor factorAtom = null;
            RegexTerm term = null;

            foreach (var child in internalNode.Children)
            {
                if (child is IInternalTreeNode childInternalNode)
                {
                    var symbolValue = childInternalNode.Symbol.Value;

                    if (RegexGrammar.Factor == symbolValue)
                    {
                        factorAtom = VisitRegexFactorNode(childInternalNode);
                    }
                    else if (RegexGrammar.Term == symbolValue)
                    {
                        term = VisitRegexTermNode(childInternalNode);
                    }
                }
            }

            return term == null 
                       ? (RegexTerm)new RegexTermFactor(factorAtom) 
                       : (RegexTerm)new RegexTermFactorTerm(factorAtom, term);
        }
    }
}