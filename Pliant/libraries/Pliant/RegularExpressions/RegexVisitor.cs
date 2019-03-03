using System;
using System.Linq;
using Pliant.Tree;

namespace Pliant.RegularExpressions
{
    public class RegexVisitor : TreeNodeVisitorBase
    {
        public Regex Regex { get; private set; }

        public override void Visit(IInternalTreeNode node)
        {
            if (node.Is(RegexGrammar.Regex))
            {
                Regex = VisitRegexNode(node);
            }
        }

        private static RegexCharacterClassCharacter VisitCharacterClassCharacterNode(IInternalTreeNode internalNode)
        {
            foreach (var tokenNode in internalNode.Children.OfType<ITokenTreeNode>())
            {
                var tokenValue = tokenNode.Token.Value;
                var isEscaped = tokenValue.StartsWith(@"\", StringComparison.CurrentCulture);

                var value = isEscaped
                                ? tokenValue[1]
                                : tokenValue[0];

                return new RegexCharacterClassCharacter(value, isEscaped);
            }

            throw new Exception("Invalid Regex Character Class Character.");
        }

        private static RegexCharacters VisitCharacterRangeNode(IInternalTreeNode node)
        {
            RegexCharacterClassCharacter start = null;
            RegexCharacterClassCharacter end = null;

            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                if (internalNode.Is(RegexGrammar.CharacterClassCharacter))
                {
                    if (start == null)
                    {
                        start = VisitCharacterClassCharacterNode(internalNode);
                    }
                    else
                    {
                        end = VisitCharacterClassCharacterNode(internalNode);
                    }
                }
            }

            if (end == null)
            {
                return new RegexCharactersUnit(start);
            }

            return new RegexCharactersRange(start, end);
        }

        private RegexCharacterClass VisitCharacterClassNode(IInternalTreeNode node)
        {
            RegexCharacters characterRange = null;
            RegexCharacterClass characterClass = null;

            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                    if (internalNode.Is(RegexGrammar.CharacterRange))
                    {
                        characterRange = VisitCharacterRangeNode(internalNode);
                    }
                    else if (internalNode.Is(RegexGrammar.CharacterClass))
                    {
                        characterClass = VisitCharacterClassNode(internalNode);
                    }
            }

            if (characterClass == null)
            {
                return new RegexCharacterClass(characterRange);
            }

            return new RegexCharacterClassAlteration(characterRange, characterClass);
        }

        private RegexSet VisitInnerSetNode(bool negate, IInternalTreeNode node)
        {
            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                    if (internalNode.Is(RegexGrammar.CharacterClass))
                    {
                        var characterClass = VisitCharacterClassNode(internalNode);
                        return new RegexSet(characterClass, negate);
                    }
            }

            throw new Exception("Invalid Inner Set Detected");
        }

        private RegexAtom VisitRegexAtomNode(IInternalTreeNode node)
        {
            foreach (var child in node.Children)
            {
                if (child is IInternalTreeNode internalNode)
                {
                    if (internalNode.Is(RegexGrammar.Character))
                    {
                        var character = VisitRegexCharacterNode(internalNode);
                        return new RegexAtomCharacter(character);
                    }

                    if (internalNode.Is(RegexGrammar.Expression))
                    {
                        var expression = VisitRegexExpressionNode(internalNode);
                        return new RegexAtomExpression(expression);
                    }

                    if (internalNode.Is(RegexGrammar.Set))
                    {
                        var set = VisitRegexSetNode(internalNode);
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
            foreach (var tokenNode in internalNode.Children.OfType<ITokenTreeNode>())
            {
                    var isEscaped = tokenNode.Token.Value.StartsWith(@"\", StringComparison.CurrentCulture);
                    var value = isEscaped
                                    ? tokenNode.Token.Value[1]
                                    : tokenNode.Token.Value[0];

                    return new RegexCharacter(value, isEscaped);
            }

            throw new Exception("Invalid character detected.");
        }

        private RegexExpression VisitRegexExpressionNode(IInternalTreeNode node)
        {
            RegexExpression expression = null;
            IRegexTerm term = null;

            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                    if (internalNode.Is(RegexGrammar.Expression))
                    {
                        expression = VisitRegexExpressionNode(internalNode);
                    }
                    else if (internalNode.Is(RegexGrammar.Term))
                    {
                        term = VisitRegexTermNode(internalNode);
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

        private IRegexFactor VisitRegexFactorNode(IInternalTreeNode node)
        {
            RegexAtom atom = null;
            RegexIterator? iterator = null;

            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                    if (internalNode.Is(RegexGrammar.Atom))
                    {
                        atom = VisitRegexAtomNode(internalNode);
                    }
                    else if (internalNode.Is(RegexGrammar.Iterator))
                    {
                        iterator = VisitRegexIteratorNode(internalNode);
                    }
            }

            if (iterator.HasValue)
            {
                return new RegexFactorIterator(atom, iterator.Value);
            }

            return new RegexFactorAtom(atom);
        }

        private RegexIterator VisitRegexIteratorNode(IInternalTreeNode internalNode)
        {
            foreach (var tokenNode in internalNode.Children.OfType<ITokenTreeNode>())
            {
                    switch (tokenNode.Token.Value)
                    {
                        case "*":
                            return RegexIterator.ZeroOrMany;

                        case "?":
                            return RegexIterator.ZeroOrOne;

                        case "+":
                            return RegexIterator.OneOrMany;
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
                switch (child)
                {
                    case IInternalTreeNode internalNode:
                        if (internalNode.Is(RegexGrammar.Expression))
                        {
                            expression = VisitRegexExpressionNode(internalNode);
                        }

                        break;
                    case ITokenTreeNode tokenNode:
                        switch (tokenNode.Token.Value)
                        {
                            case "$":
                                endsWith = true;
                                break;

                            case "^":
                                startsWith = true;
                                break;
                        }

                        break;
                }
            }

            return new Regex(startsWith, expression, endsWith);
        }

        private RegexSet VisitRegexSetNode(IInternalTreeNode node)
        {
            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                    if (internalNode.Is(RegexGrammar.PositiveSet))
                    {
                        return VisitInnerSetNode(false, internalNode);
                    }

                    if (internalNode.Is(RegexGrammar.NegativeSet))
                    {
                        return VisitInnerSetNode(true, internalNode);
                    }
            }

            throw new Exception("Invalid Set Detected.");
        }

        private IRegexTerm VisitRegexTermNode(IInternalTreeNode node)
        {
            IRegexFactor factorAtom = null;
            IRegexTerm term = null;

            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                    if (internalNode.Is(RegexGrammar.Factor))
                    {
                        factorAtom = VisitRegexFactorNode(internalNode);
                    }
                    else if (internalNode.Is(RegexGrammar.Term))
                    {
                        term = VisitRegexTermNode(internalNode);
                    }
            }

            if (term == null)
            {
                return new RegexTermFactor(factorAtom);
            }

            return new RegexTermFactorTerm(factorAtom, term);
        }
    }
}