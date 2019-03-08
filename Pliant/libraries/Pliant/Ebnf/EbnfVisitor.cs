using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Pliant.LexerRules;
using Pliant.RegularExpressions;
using Pliant.Tree;

namespace Pliant.Ebnf
{
    public class EbnfVisitor : TreeNodeVisitorBase
    {
        public IEbnfDefinition Definition { get; private set; }

        public override void Visit(IInternalTreeNode node)
        {
            if (node.Is(EbnfGrammar.Definition))
            {
                Definition = VisitDefinitionNode(node);
            }
        }

        private static Exception UnreachableCodeException()
        {
            return new InvalidOperationException("Unreachable Code Detected");
        }

        private static IEbnfLexerRuleFactor VisitLexerRuleFactorNode(IInternalTreeNode node)
        {
            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                if (internalNode.Is(EbnfGrammar.Literal))
                {
                    return new EbnfLexerRuleFactorLiteral(VisitLiteralNode(internalNode));
                }

                if (internalNode.Is(RegexGrammar.Regex))
                {
                    var regexVisitor = new RegexVisitor();
                    internalNode.Accept(regexVisitor);
                    return new EbnfLexerRuleFactorRegex(regexVisitor.Regex);
                }
            }

            throw UnreachableCodeException();
        }

        private static string VisitLiteralNode(IInternalTreeNode node)
        {
            foreach (var tokenNode in node.Children.OfType<ITokenTreeNode>())
            {
                var token = tokenNode.Token;
                var tokenName = token.TokenName;

                // if token type is string token type remove surrounding quotes
                if (tokenName.Equals(SimpleSingleQuoteStringLexerRule.Class) ||
                    tokenName.Equals(SimpleDoubleQuoteStringLexerRule.Class))
                {
                    return token.Value.Substring(1, token.Value.Length - 2);
                }

                // TODO: Find a better solution for identifing the lexer rule based on id
                if (tokenNode.Token.TokenName.Id.Length > 5)
                {
                    return token.Value;
                }
            }

            throw UnreachableCodeException();
        }

        private IEbnfBlock VisitBlockNode(IInternalTreeNode node)
        {
            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                if (internalNode.Is(EbnfGrammar.Rule))
                {
                    return VisitRuleNode(internalNode);
                }

                if (internalNode.Is(EbnfGrammar.Setting))
                {
                    return VisitSettingNode(internalNode);
                }

                if (internalNode.Is(EbnfGrammar.LexerRule))
                {
                    return VisitLexerRuleNode(internalNode);
                }
            }

            throw UnreachableCodeException();
        }

        private IEbnfDefinition VisitDefinitionNode(IInternalTreeNode node)
        {
            IEbnfBlock block = null;
            IEbnfDefinition definition = null;

            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                if (internalNode.Is(EbnfGrammar.Block))
                {
                    block = VisitBlockNode(internalNode);
                }
                else if (internalNode.Is(EbnfGrammar.Definition))
                {
                    definition = VisitDefinitionNode(internalNode);
                }
            }

            if (definition == null)
            {
                return new EbnfDefinitionSimple(block);
            }

            return new EbnfDefinitionConcatenation(block, definition);
        }

        private IEbnfExpression VisitExpressionNode(IInternalTreeNode node)
        {
            IEbnfTerm term = null;
            IEbnfExpression expression = null;

            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                if (internalNode.Is(EbnfGrammar.Term))
                {
                    term = VisitTermNode(internalNode);
                }
                else if (internalNode.Is(EbnfGrammar.Expression))
                {
                    expression = VisitExpressionNode(internalNode);
                }
            }

            if (expression == null)
            {
                return new EbnfExpressionSimple(term);
            }

            return new EbnfExpressionAlteration(term, expression);
        }

        private IEbnfFactor VisitFactorNode(IInternalTreeNode node)
        {
            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                if (internalNode.Is(EbnfGrammar.QualifiedIdentifier))
                {
                    return new EbnfFactorIdentifier(VisitQualifiedIdentifierNode(internalNode));
                }

                if (internalNode.Is(EbnfGrammar.Literal))
                {
                    return new EbnfFactorLiteral(VisitLiteralNode(internalNode));
                }

                if (internalNode.Is(EbnfGrammar.Repetition))
                {
                    return VisitRepetitionNode(internalNode);
                }

                if (internalNode.Is(EbnfGrammar.Optional))
                {
                    return VisitOptionalNode(internalNode);
                }

                if (internalNode.Is(EbnfGrammar.Grouping))
                {
                    return VisitGroupingNode(internalNode);
                }

                if (internalNode.Is(RegexGrammar.Regex))
                {
                    var regexVisitor = new RegexVisitor();
                    internalNode.Accept(regexVisitor);
                    return new EbnfFactorRegex(regexVisitor.Regex);
                }
            }

            throw UnreachableCodeException();
        }

        private EbnfFactorGrouping VisitGroupingNode(IInternalTreeNode node)
        {
            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                if (internalNode.Is(EbnfGrammar.Expression))
                {
                    return new EbnfFactorGrouping(VisitExpressionNode(internalNode));
                }
            }

            throw UnreachableCodeException();
        }

        private IEbnfLexerRuleExpression VisitLexerRuleExpressionNode(IInternalTreeNode node)
        {
            IEbnfLexerRuleTerm term = null;
            IEbnfLexerRuleExpression expression = null;

            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                if (internalNode.Is(EbnfGrammar.LexerRuleTerm))
                {
                    term = VisitLexerRuleTermNode(internalNode);
                }
                else if (internalNode.Is(EbnfGrammar.LexerRuleExpression))
                {
                    expression = VisitLexerRuleExpressionNode(internalNode);
                }
            }

            if (expression == null)
            {
                return new EbnfLexerRuleExpressionSimple(term);
            }

            return new EbnfLexerRuleExpressionAlteration(term, expression);
        }

        private EbnfBlockLexerRule VisitLexerRuleNode(IInternalTreeNode node)
        {
            EbnfQualifiedIdentifier identifier = null;
            IEbnfLexerRuleExpression expression = null;

            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                if (internalNode.Is(EbnfGrammar.QualifiedIdentifier))
                {
                    identifier = VisitQualifiedIdentifierNode(internalNode);
                }
                else if (internalNode.Is(EbnfGrammar.LexerRuleExpression))
                {
                    expression = VisitLexerRuleExpressionNode(internalNode);
                }
            }

            return new EbnfBlockLexerRule(new EbnfLexerRule(identifier, expression));
        }

        private IEbnfLexerRuleTerm VisitLexerRuleTermNode(IInternalTreeNode node)
        {
            IEbnfLexerRuleFactor factor = null;
            IEbnfLexerRuleTerm term = null;

            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                if (internalNode.Is(EbnfGrammar.LexerRuleFactor))
                {
                    factor = VisitLexerRuleFactorNode(internalNode);
                }
                else if (internalNode.Is(EbnfGrammar.LexerRuleTerm))
                {
                    term = VisitLexerRuleTermNode(internalNode);
                }
            }

            if (term == null)
            {
                return new EbnfLexerRuleTermSimple(factor);
            }

            return new EbnfLexerRuleTermConcatenation(factor, term);
        }

        private EbnfFactorOptional VisitOptionalNode(IInternalTreeNode node)
        {
            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                if (internalNode.Is(EbnfGrammar.Expression))
                {
                    return new EbnfFactorOptional(VisitExpressionNode(internalNode));
                }
            }

            throw UnreachableCodeException();
        }

        private IEnumerable<string> VisitQualifiedIdentifier(IInternalTreeNode node)
        {
            foreach (var child in node.Children)
            {
                switch (child)
                {
                    case IInternalTreeNode internalNode:
                        if (internalNode.Is(EbnfGrammar.QualifiedIdentifier))
                        {
                            foreach (var identifier in VisitQualifiedIdentifier(internalNode))
                            {
                                yield return identifier;
                            }
                        }
                        break;

                    case ITokenTreeNode tokenNode:
                        var token = tokenNode.Token;
                        if (token.TokenName.Equals(EbnfGrammar.TokenClasses.Identifier))
                        {
                            yield return token.Value;
                        }
                        break;
                }
            }
        }

        private EbnfQualifiedIdentifier VisitQualifiedIdentifierNode(IInternalTreeNode node)
        {
#if true
            return new EbnfQualifiedIdentifier(VisitQualifiedIdentifier(node));
#else
            EbnfQualifiedIdentifier repetitionEbnfQualifiedIdentifier = null;
            string identifier = null;
            foreach (var child in node.Children)
            {
                switch (child)
                {
                    case IInternalTreeNode internalNode:
                        if (internalNode.Is(EbnfGrammar.QualifiedIdentifier))
                        {
                            repetitionEbnfQualifiedIdentifier = VisitQualifiedIdentifierNode(internalNode);
                        }

                        break;

                    case ITokenTreeNode tokenNode:
                        var token = tokenNode.Token;
                        if (token.TokenType.Equals(EbnfGrammar.TokenTypes.Identifier))
                        {
                            identifier = token.Value;
                        }

                        break;
                }
            }

            if (repetitionEbnfQualifiedIdentifier == null)
            {
                return new EbnfQualifiedIdentifierSimple(identifier);
            }

            return new EbnfQualifiedIdentifierConcatenation(identifier, repetitionEbnfQualifiedIdentifier);
#endif
        }

        private EbnfFactorRepetition VisitRepetitionNode(IInternalTreeNode node)
        {
            Debug.Assert(node.Children.Count > 0);

            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                if (internalNode.Is(EbnfGrammar.Expression))
                {
                    return new EbnfFactorRepetition(VisitExpressionNode(internalNode));
                }
            }

            throw UnreachableCodeException();
        }

        private EbnfBlockRule VisitRuleNode(IInternalTreeNode node)
        {
            EbnfQualifiedIdentifier qualifiedEbnfQualifiedIdentifier = null;
            IEbnfExpression expression = null;

            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                if (internalNode.Is(EbnfGrammar.QualifiedIdentifier))
                {
                    qualifiedEbnfQualifiedIdentifier = VisitQualifiedIdentifierNode(internalNode);
                }
                else if (internalNode.Is(EbnfGrammar.Expression))
                {
                    expression = VisitExpressionNode(internalNode);
                }
            }

            return new EbnfBlockRule(new EbnfRule(qualifiedEbnfQualifiedIdentifier, expression));
        }

        private EbnfBlockSetting VisitSettingNode(IInternalTreeNode node)
        {
            EbnfSettingIdentifier settingIdentifier = null;
            EbnfQualifiedIdentifier qualifiedEbnfQualifiedIdentifier = null;

            foreach (var child in node.Children)
            {
                switch (child)
                {
                    case IInternalTreeNode internalNode:
                        if (internalNode.Is(EbnfGrammar.QualifiedIdentifier))
                        {
                            qualifiedEbnfQualifiedIdentifier = VisitQualifiedIdentifierNode(internalNode);
                        }

                        break;
                    case ITokenTreeNode tokenNode:
                        var token = tokenNode.Token;
                        if (token.TokenName.Equals(EbnfGrammar.TokenClasses.SettingIdentifier))
                        {
                            settingIdentifier = new EbnfSettingIdentifier(token.Value);
                        }

                        break;
                }
            }

            return new EbnfBlockSetting(new EbnfSetting(settingIdentifier, qualifiedEbnfQualifiedIdentifier));
        }

        private IEbnfTerm VisitTermNode(IInternalTreeNode node)
        {
            IEbnfFactor factor = null;
            IEbnfTerm term = null;

            foreach (var internalNode in node.Children.OfType<IInternalTreeNode>())
            {
                if (internalNode.Is(EbnfGrammar.Factor))
                {
                    factor = VisitFactorNode(internalNode);
                }
                else if (internalNode.Is(EbnfGrammar.Term))
                {
                    term = VisitTermNode(internalNode);
                }
            }

            if (term == null)
            {
                return new EbnfTermSimple(factor);
            }

            return new EbnfTermConcatenation(factor, term);
        }
    }
}