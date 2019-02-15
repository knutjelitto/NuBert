using System;
using Pliant.LexerRules;
using Pliant.RegularExpressions;
using Pliant.Tree;

namespace Pliant.Ebnf
{
    public class EbnfVisitor : TreeNodeVisitorBase
    {
        public EbnfDefinition Definition { get; private set; }

        public override void Visit(IInternalTreeNode node)
        {
            if (EbnfGrammar.Definition == node.Symbol.Value)
            {
                Definition = VisitDefinitionNode(node);
            }
        }

        private static Exception UnreachableCodeException()
        {
            return new InvalidOperationException("Unreachable Code Detected");
        }

        private static EbnfLexerRuleFactor VisitLexerRuleFactorNode(IInternalTreeNode node)
        {
            foreach (var child in node.Children)
            {
                if (child is IInternalTreeNode internalNode)
                {
                    var symbolValue = internalNode.Symbol.Value;

                    if (EbnfGrammar.Literal == symbolValue)
                    {
                        return new EbnfLexerRuleFactorLiteral(VisitLiteralNode(internalNode));
                    }

                    if (RegexGrammar.Regex == symbolValue)
                    {
                        var regexVisitor = new RegexVisitor();
                        internalNode.Accept(regexVisitor);
                        return new EbnfLexerRuleFactorRegex(regexVisitor.Regex);
                    }
                }
            }

            throw UnreachableCodeException();
        }

        private static string VisitLiteralNode(IInternalTreeNode node)
        {
            foreach (var child in node.Children)
            {
                if (child is ITokenTreeNode tokenNode)
                {
                    var token = tokenNode.Token;
                    var tokenType = token.TokenType;

                    // if token type is string token type remove surrounding quotes
                    if (tokenType.Equals(SingleQuoteStringLexerRule.TokenTypeDescriptor) ||
                        tokenType.Equals(DoubleQuoteStringLexerRule.TokenTypeDescriptor))
                    {
                        return token.Value.Substring(1, token.Value.Length - 2);
                    }

                    // TODO: Find a better solution for identifing the lexer rule based on id
                    if (tokenNode.Token.TokenType.Id.Length > 5)
                    {
                        return token.Value;
                    }
                }
            }

            throw UnreachableCodeException();
        }

        private EbnfBlock VisitBlockNode(IInternalTreeNode node)
        {
            foreach (var child in node.Children)
            {
                if (child is IInternalTreeNode internalNode)
                {
                    var symbolValue = internalNode.Symbol.Value;

                    if (EbnfGrammar.Rule == symbolValue)
                    {
                        return VisitRuleNode(internalNode);
                    }

                    if (EbnfGrammar.Setting == symbolValue)
                    {
                        return VisitSettingNode(internalNode);
                    }

                    if (EbnfGrammar.LexerRule == symbolValue)
                    {
                        return VisitLexerRuleNode(internalNode);
                    }
                }
            }

            throw UnreachableCodeException();
        }

        private EbnfDefinition VisitDefinitionNode(IInternalTreeNode node)
        {
            EbnfBlock block = null;
            EbnfDefinition definition = null;

            foreach (var child in node.Children)
            {
                if (child is IInternalTreeNode internalNode)
                {
                    var symbolValue = internalNode.Symbol.Value;

                    if (EbnfGrammar.Block == symbolValue)
                    {
                        block = VisitBlockNode(internalNode);
                    }
                    else if (EbnfGrammar.Definition == symbolValue)
                    {
                        definition = VisitDefinitionNode(internalNode);
                    }
                }
            }

            return definition == null
                       ? new EbnfDefinition(block)
                       : new EbnfDefinitionConcatenation(block, definition);
        }

        private EbnfExpression VisitExpressionNode(IInternalTreeNode node)
        {
            EbnfTerm term = null;
            EbnfExpression expression = null;

            foreach (var child in node.Children)
            {
                if (child is IInternalTreeNode internalNode)
                {
                    var symbolValue = internalNode.Symbol.Value;
                    if (EbnfGrammar.Term == symbolValue)
                    {
                        term = VisitTermNode(internalNode);
                    }
                    else if (EbnfGrammar.Expression == symbolValue)
                    {
                        expression = VisitExpressionNode(internalNode);
                    }
                }
            }

            return expression == null
                       ? new EbnfExpression(term)
                       : new EbnfExpressionAlteration(term, expression);
        }

        private EbnfFactor VisitFactorNode(IInternalTreeNode node)
        {
            foreach (var child in node.Children)
            {
                if (child is IInternalTreeNode internalNode)
                {
                    var symbolValue = internalNode.Symbol.Value;

                    if (EbnfGrammar.QualifiedIdentifier == symbolValue)
                    {
                        return new EbnfFactorIdentifier(
                            VisitQualifiedIdentifierNode(internalNode));
                    }

                    if (EbnfGrammar.Literal == symbolValue)
                    {
                        return new EbnfFactorLiteral(
                            VisitLiteralNode(internalNode));
                    }

                    if (EbnfGrammar.Repetition == symbolValue)
                    {
                        return VisitRepetitionNode(internalNode);
                    }

                    if (EbnfGrammar.Optional == symbolValue)
                    {
                        return VisitOptionalNode(internalNode);
                    }

                    if (EbnfGrammar.Grouping == symbolValue)
                    {
                        return VisitGroupingNode(internalNode);
                    }

                    if (RegexGrammar.Regex == symbolValue)
                    {
                        var regexVisitor = new RegexVisitor();
                        internalNode.Accept(regexVisitor);
                        return new EbnfFactorRegex(regexVisitor.Regex);
                    }
                }
            }

            throw UnreachableCodeException();
        }

        private EbnfFactorGrouping VisitGroupingNode(IInternalTreeNode node)
        {
            foreach (var child in node.Children)
            {
                if (child is IInternalTreeNode internalNode)
                {
                    var symbolValue = internalNode.Symbol.Value;
                    if (EbnfGrammar.Expression == symbolValue)
                    {
                        return new EbnfFactorGrouping(VisitExpressionNode(internalNode));
                    }
                }
            }

            throw UnreachableCodeException();
        }

        private EbnfLexerRuleExpression VisitLexerRuleExpressionNode(IInternalTreeNode node)
        {
            EbnfLexerRuleTerm term = null;
            EbnfLexerRuleExpression expression = null;

            foreach (var child in node.Children)
            {
                if (child is IInternalTreeNode internalNode)
                {
                    var symbolValue = internalNode.Symbol.Value;
                    if (EbnfGrammar.LexerRuleTerm == symbolValue)
                    {
                        term = VisitLexerRuleTermNode(internalNode);
                    }

                    if (EbnfGrammar.LexerRuleExpression == symbolValue)
                    {
                        expression = VisitLexerRuleExpressionNode(internalNode);
                    }
                }
            }

            return expression == null
                       ? new EbnfLexerRuleExpression(term)
                       : new EbnfLexerRuleExpressionAlteration(term, expression);
        }

        private EbnfBlockLexerRule VisitLexerRuleNode(IInternalTreeNode node)
        {
            EbnfQualifiedIdentifier qualifiedIdentifier = null;
            EbnfLexerRuleExpression expression = null;

            foreach (var child in node.Children)
            {
                if (child is IInternalTreeNode internalNode)
                {
                    var symbolValue = internalNode.Symbol.Value;
                    if (EbnfGrammar.QualifiedIdentifier == symbolValue)
                    {
                        qualifiedIdentifier = VisitQualifiedIdentifierNode(internalNode);
                    }
                    else if (EbnfGrammar.LexerRuleExpression == symbolValue)
                    {
                        expression = VisitLexerRuleExpressionNode(internalNode);
                    }
                }
            }

            return new EbnfBlockLexerRule(new EbnfLexerRule(qualifiedIdentifier, expression));
        }

        private EbnfLexerRuleTerm VisitLexerRuleTermNode(IInternalTreeNode node)
        {
            EbnfLexerRuleFactor factor = null;
            EbnfLexerRuleTerm term = null;

            foreach (var child in node.Children)
            {
                if (child is IInternalTreeNode internalNode)
                {
                    var symbolValue = internalNode.Symbol.Value;
                    if (EbnfGrammar.LexerRuleFactor == symbolValue)
                    {
                        factor = VisitLexerRuleFactorNode(internalNode);
                    }

                    if (EbnfGrammar.LexerRuleTerm == symbolValue)
                    {
                        term = VisitLexerRuleTermNode(internalNode);
                    }
                }
            }

            return term == null
                       ? new EbnfLexerRuleTerm(factor)
                       : new EbnfLexerRuleTermConcatenation(factor, term);
        }

        private EbnfFactorOptional VisitOptionalNode(IInternalTreeNode node)
        {
            foreach (var child in node.Children)
            {
                if (child is IInternalTreeNode internalNode)
                {
                    var symbolValue = internalNode.Symbol.Value;
                    if (EbnfGrammar.Expression == symbolValue)
                    {
                        return new EbnfFactorOptional(VisitExpressionNode(internalNode));
                    }
                }
            }

            throw UnreachableCodeException();
        }

        private EbnfQualifiedIdentifier VisitQualifiedIdentifierNode(IInternalTreeNode node)
        {
            EbnfQualifiedIdentifier repetitionIdentifier = null;
            string identifier = null;
            foreach (var child in node.Children)
            {
                switch (child)
                {
                    case IInternalTreeNode internalNode:
                        var symbolValue = internalNode.Symbol.Value;
                        if (EbnfGrammar.QualifiedIdentifier == symbolValue)
                        {
                            repetitionIdentifier = VisitQualifiedIdentifierNode(internalNode);
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

            return repetitionIdentifier == null
                       ? new EbnfQualifiedIdentifier(identifier)
                       : new EbnfQualifiedIdentifierConcatenation(identifier, repetitionIdentifier);
        }

        private EbnfFactorRepetition VisitRepetitionNode(IInternalTreeNode node)
        {
            foreach (var child in node.Children)
            {
                if (child is IInternalTreeNode internalNode)
                {
                    var symbolValue = internalNode.Symbol.Value;
                    if (EbnfGrammar.Expression == symbolValue)
                    {
                        return new EbnfFactorRepetition(VisitExpressionNode(internalNode));
                    }
                }
            }

            throw UnreachableCodeException();
        }

        private EbnfBlockRule VisitRuleNode(IInternalTreeNode node)
        {
            EbnfQualifiedIdentifier qualifiedIdentifier = null;
            EbnfExpression expression = null;

            foreach (var child in node.Children)
            {
                if (child is IInternalTreeNode internalNode)
                {
                    var symbolValue = internalNode.Symbol.Value;

                    if (EbnfGrammar.QualifiedIdentifier == symbolValue)
                    {
                        qualifiedIdentifier = VisitQualifiedIdentifierNode(internalNode);
                    }
                    else if (EbnfGrammar.Expression == symbolValue)
                    {
                        expression = VisitExpressionNode(internalNode);
                    }
                }
            }

            return new EbnfBlockRule(new EbnfRule(qualifiedIdentifier, expression));
        }

        private EbnfBlockSetting VisitSettingNode(IInternalTreeNode node)
        {
            EbnfSettingIdentifier settingIdentifier = null;
            EbnfQualifiedIdentifier qualifiedIdentifier = null;

            foreach (var child in node.Children)
            {
                switch (child)
                {
                    case ITokenTreeNode tokenNode:
                        var token = tokenNode.Token;
                        if (token.TokenType.Equals(EbnfGrammar.TokenTypes.SettingIdentifier))
                        {
                            settingIdentifier = new EbnfSettingIdentifier(token.Value);
                        }

                        break;
                    case IInternalTreeNode internalNode:
                        var symbolValue = internalNode.Symbol.Value;
                        if (EbnfGrammar.QualifiedIdentifier == symbolValue)
                        {
                            qualifiedIdentifier = VisitQualifiedIdentifierNode(internalNode);
                        }

                        break;
                }
            }

            return new EbnfBlockSetting(new EbnfSetting(settingIdentifier, qualifiedIdentifier));
        }

        private EbnfTerm VisitTermNode(IInternalTreeNode node)
        {
            EbnfFactor factor = null;
            EbnfTerm term = null;

            foreach (var child in node.Children)
            {
                if (child is IInternalTreeNode internalNode)
                {
                    var symbolValue = internalNode.Symbol.Value;
                    if (EbnfGrammar.Factor == symbolValue)
                    {
                        factor = VisitFactorNode(internalNode);
                    }
                    else if (EbnfGrammar.Term == symbolValue)
                    {
                        term = VisitTermNode(internalNode);
                    }
                }
            }

            return term == null
                       ? new EbnfTerm(factor)
                       : new EbnfTermConcatenation(factor, term);
        }
    }
}