using System;
using System.Collections.Generic;
using System.Linq;
using Pliant.Automata;
using Pliant.Builders;
using Pliant.Grammars;
using Pliant.LexerRules;
using Pliant.RegularExpressions;
using Pliant.Terminals;
using Pliant.Tokens;

namespace Pliant.Ebnf
{
    public class EbnfGrammarGenerator
    {
        public EbnfGrammarGenerator()
        {
            this.regexToNfa = new ThompsonConstructionAlgorithm();
            this.nfaToDfa = new SubsetConstructionAlgorithm();
        }

        public Grammar Generate(IEbnfDefinition ebnf)
        {
            var grammarModel = new GrammarModel();
            Definition(ebnf, grammarModel);
            return grammarModel.ToGrammar();
        }

        private static QualifiedName GetQualifiedName(EbnfQualifiedIdentifier qualifiedIdentifier)
        {
            var identifiers = qualifiedIdentifier.Identifiers;

            if (identifiers.Length <= 1)
            {
                return new QualifiedName(identifiers[0]);
            }

            return new QualifiedName(
                string.Join(".", identifiers.Take(identifiers.Length - 1)),
                identifiers[identifiers.Length - 1]);
        }

        private void Block(IEbnfBlock block, GrammarModel grammarModel)
        {
            switch (block)
            {
                case EbnfBlockLexerRule blockLexerRule:
                    grammarModel.AddLexerRule(LexerRule(blockLexerRule));
                    break;

                case EbnfBlockRule blockRule:
                    foreach (var production in Rule(blockRule.Rule))
                    {
                        grammarModel.AddProduction(production);
                    }

                    break;

                case EbnfBlockSetting blockSetting:
                    switch (blockSetting.Setting.SettingIdentifier.Value)
                    {
                        case StartProductionSettingModel.SettingKey:
                            grammarModel.StartSetting = StartSetting(blockSetting);
                            break;

                        case IgnoreSettingModel.SettingKey:
                            grammarModel.AddIgnoreSetting(IgnoreSetting(blockSetting));
                            break;

                        case TriviaSettingModel.SettingKey:
                            grammarModel.AddTriviaSetting(TriviaSetting(blockSetting));
                            break;

                        default:
                            throw new NotImplementedException($"invalid setting `{blockSetting.Setting.SettingIdentifier}´" +
                                                              $" with value `{blockSetting.Setting.Identifier}´");
                    }

                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void Definition(IEbnfDefinition definition, GrammarModel grammarModel)
        {
            Block(definition.Block, grammarModel);

            if (definition is EbnfDefinitionConcatenation definitionConcatenation)
            {
                Definition(definitionConcatenation.Definition, grammarModel);
            }
        }

        private IEnumerable<ProductionModel> Expression(IEbnfExpression expression, ProductionModel currentProduction)
        {
            foreach (var production in Term(expression.Term, currentProduction))
            {
                yield return production;
            }

            if (expression is EbnfExpressionAlteration expressionAlteration)
            {
                currentProduction.Lambda();

                foreach (var production in Expression(expressionAlteration.Expression, currentProduction))
                {
                    yield return production;
                }
            }
        }

        private IEnumerable<ProductionModel> Factor(IEbnfFactor factor, ProductionModel currentProduction)
        {
            switch (factor)
            {
                case EbnfFactorGrouping grouping:
                    foreach (var production in Grouping(grouping, currentProduction))
                    {
                        yield return production;
                    }

                    break;

                case EbnfFactorOptional optional:
                    foreach (var production in Optional(optional, currentProduction))
                    {
                        yield return production;
                    }

                    break;

                case EbnfFactorRepetition repetition:
                    foreach (var production in Repetition(repetition, currentProduction))
                    {
                        yield return production;
                    }

                    break;

                case EbnfFactorIdentifier identifier:
                    var nonTerminal = NonTerminal.From(GetQualifiedName(identifier.QualifiedIdentifier));
                    currentProduction.AddWithAnd(new NonTerminalModel(nonTerminal));
                    break;

                case EbnfFactorLiteral literal:
                    var stringLiteralRule = new StringLiteralLexerRule(literal.Value);
                    currentProduction.AddWithAnd(new LexerRuleModel(stringLiteralRule));
                    break;

                case EbnfFactorRegex regex:
                    var nfa = this.regexToNfa.Transform(regex.Regex);
                    var dfa = this.nfaToDfa.Transform(nfa);
                    var dfaLexerRule = new DfaLexerRule(dfa, regex.Regex.ToString());
                    currentProduction.AddWithAnd(new LexerRuleModel(dfaLexerRule));
                    break;
            }
        }

        private IEnumerable<ProductionModel> Grouping(EbnfFactorGrouping grouping, ProductionModel currentProduction)
        {
            var name = grouping.ToString();
            var nonTerminal = NonTerminal.From(name);
            var groupingProduction = ProductionModel.From(nonTerminal);

            currentProduction.AddWithAnd(new NonTerminalModel(nonTerminal));

            var expression = grouping.Expression;
            foreach (var production in Expression(expression, groupingProduction))
            {
                yield return production;
            }

            yield return groupingProduction;
        }

        private IgnoreSettingModel IgnoreSetting(EbnfBlockSetting blockSetting)
        {
            var fullyQualifiedName =
                GetQualifiedName(blockSetting.Setting.Identifier);
            return new IgnoreSettingModel(fullyQualifiedName);
        }

        private LexerRuleModel LexerRule(EbnfBlockLexerRule blockLexerRule)
        {
            var ebnfLexerRule = blockLexerRule.LexerRule;

            var fullyQualifiedName = GetQualifiedName(
                ebnfLexerRule.Identifier);

            var lexerRule = LexerRuleExpression(
                fullyQualifiedName,
                ebnfLexerRule.Expression);

            return new LexerRuleModel(lexerRule);
        }

        private LexerRule LexerRuleExpression(
            QualifiedName fullyQualifiedName,
            IEbnfLexerRuleExpression ebnfLexerRule)
        {
            if (TryRecognizeSimpleLiteralExpression(fullyQualifiedName, ebnfLexerRule, out var lexerRule))
            {
                return lexerRule;
            }

            var nfa = LexerRuleExpression(ebnfLexerRule);
            var dfa = this.nfaToDfa.Transform(nfa);

            return new DfaLexerRule(dfa, fullyQualifiedName.FullName);
        }

        private Nfa LexerRuleExpression(IEbnfLexerRuleExpression expression)
        {
            var nfa = LexerRuleTerm(expression.Term);
            if (expression is EbnfLexerRuleExpressionAlteration alteration)
            {
                var alterationNfa = LexerRuleExpression(alteration);
                nfa = nfa.Union(alterationNfa);
            }

            return nfa;
        }

        private Nfa LexerRuleFactor(IEbnfLexerRuleFactor factor)
        {
            switch (factor)
            {
                case EbnfLexerRuleFactorLiteral literal:
                    return LexerRuleFactorLiteral(literal);

                case EbnfLexerRuleFactorRegex regex:
                    return LexerRuleFactorRegex(regex);

                default:
                    throw new InvalidOperationException(
                        $"Invalid IEbnfLexerRuleFactor node type detected. Found {factor.GetType().Name}, expected EbnfLexerRuleFactorLiteral or EbnfLexerRuleFactorRegex");
            }
        }

        private Nfa LexerRuleFactorLiteral(EbnfLexerRuleFactorLiteral ebnfLexerRuleFactorLiteral)
        {
            var literal = ebnfLexerRuleFactorLiteral.Value;
            var states = new NfaState[literal.Length + 1];
            for (var i = 0; i < states.Length; i++)
            {
                var current = new NfaState();
                states[i] = current;

                if (i == 0)
                {
                    continue;
                }

                var previous = states[i - 1];
                previous.AddTransition(new CharacterTerminal(literal[i - 1]), current);
            }

            return new Nfa(states[0], states[states.Length - 1]);
        }

        private Nfa LexerRuleFactorRegex(EbnfLexerRuleFactorRegex ebnfLexerRuleFactorRegex)
        {
            var regex = ebnfLexerRuleFactorRegex.Regex;
            return this.regexToNfa.Transform(regex);
        }

        private Nfa LexerRuleTerm(IEbnfLexerRuleTerm term)
        {
            var nfa = LexerRuleFactor(term.Factor);
            if (term is EbnfLexerRuleTermConcatenation concatenation)
            {
                var concatNfa = LexerRuleTerm(concatenation.Term);
                nfa = nfa.Concatenation(concatNfa);
            }

            return nfa;
        }

        private IEnumerable<ProductionModel> Optional(EbnfFactorOptional optional, ProductionModel currentProduction)
        {
            var name = optional.ToString();
            var nonTerminal = NonTerminal.From(name);
            var optionalProduction = ProductionModel.From(nonTerminal);

            currentProduction.AddWithAnd(new NonTerminalModel(nonTerminal));

            var expression = optional.Expression;
            foreach (var production in Expression(expression, optionalProduction))
            {
                yield return production;
            }

            optionalProduction.Lambda();
            yield return optionalProduction;
        }

        private IEnumerable<ProductionModel> Repetition(EbnfFactorRepetition repetition, ProductionModel currentProduction)
        {
            var name = repetition.ToString();
            var nonTerminal = NonTerminal.From(name);
            var repetitionProduction = ProductionModel.From(nonTerminal);

            currentProduction.AddWithAnd(new NonTerminalModel(nonTerminal));

            var expression = repetition.Expression;
            foreach (var production in Expression(expression, repetitionProduction))
            {
                yield return production;
            }

            repetitionProduction.AddWithAnd(new NonTerminalModel(nonTerminal));
            repetitionProduction.Lambda();

            yield return repetitionProduction;
        }

        private IEnumerable<ProductionModel> Rule(EbnfRule rule)
        {
            var nonTerminal = GetQualifiedName(rule.Identifier);
            var productionModel = ProductionModel.From(nonTerminal);
            foreach (var production in Expression(rule.Expression, productionModel))
            {
                yield return production;
            }

            yield return productionModel;
        }

        private StartProductionSettingModel StartSetting(EbnfBlockSetting blockSetting)
        {
            var productionName = GetQualifiedName(
                blockSetting.Setting.Identifier);
            return new StartProductionSettingModel(productionName);
        }

        private IEnumerable<ProductionModel> Term(IEbnfTerm term, ProductionModel currentProduction)
        {
            foreach (var production in Factor(term.Factor, currentProduction))
            {
                yield return production;
            }

            if (term is EbnfTermConcatenation concatenation)
            {
                foreach (var production in Term(concatenation.Term, currentProduction))
                {
                    yield return production;
                }
            }
        }

        private TriviaSettingModel TriviaSetting(EbnfBlockSetting blockSetting)
        {
            var fullyQualifiedName =
                GetQualifiedName(blockSetting.Setting.Identifier);
            return new TriviaSettingModel(fullyQualifiedName);
        }

        private bool TryRecognizeSimpleLiteralExpression(
            QualifiedName fullyQualifiedName,
            IEbnfLexerRuleExpression ebnfLexerRule,
            out LexerRule lexerRule)
        {
            lexerRule = null;

            if (ebnfLexerRule is EbnfLexerRuleExpressionSimple)
            {
                var term = ebnfLexerRule.Term;
                if (term is EbnfLexerRuleTermConcatenation)
                {
                    return false;
                }

                var factor = term.Factor;
                if (factor is EbnfLexerRuleFactorLiteral literal)
                {
                    lexerRule = new StringLiteralLexerRule(literal.Value, new TokenName(fullyQualifiedName.FullName));

                    return true;
                }
            }

            return false;
        }

        private readonly NfaToDfa nfaToDfa;
        private readonly RegexToNfa regexToNfa;
    }
}