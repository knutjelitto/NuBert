﻿using System.Collections.Generic;
using System.Linq;
using Pliant.Grammars;

namespace Pliant.Builders.Expressions
{
    public sealed class GrammarExpression
    {
        public GrammarExpression(
            ProductionExpression start,
            IReadOnlyList<ProductionExpression> productions = null,
            IReadOnlyList<LexerRuleModel> ignore = null,
            IReadOnlyList<LexerRuleModel> trivia = null)
        {
            Initialize(start, productions, ignore, trivia);
        }

        public GrammarExpression(
            ProductionExpression start,
            IReadOnlyList<ProductionExpression> productions,
            IReadOnlyList<Lexer> ignore,
            IReadOnlyList<Lexer> trivia = null)
        {
            var ignoreModelList = ToLexerRuleModelList(ignore);
            var triviaModelList = ToLexerRuleModelList(trivia);

            Initialize(start, productions, ignoreModelList, triviaModelList);
        }

        public GrammarModel GrammarModel { get; private set; }

        public Grammar ToGrammar()
        {
            return GrammarModel.ToGrammar();
        }

        private static List<LexerRuleModel> ToLexerRuleModelList(IReadOnlyCollection<Lexer> lexerRules)
        {
            if (lexerRules == null || lexerRules.Count == 0)
            {
                return null;
            }

            return lexerRules.Select(rule => new LexerRuleModel(rule)).ToList();
        }

        private void Initialize(ProductionExpression start,
                                IReadOnlyList<ProductionExpression> productions,
                                IReadOnlyList<LexerRuleModel> ignore,
                                IReadOnlyList<LexerRuleModel> trivia)
        {
            GrammarModel = new GrammarModel
            {
                Start = start.ProductionModel
            };

            if (productions != null)
            {
                foreach (var production in productions)
                {
                    GrammarModel.AddProduction(production.ProductionModel);
                }
            }

            if (ignore != null)
            {
                foreach (var ignoreRule in ignore)
                {
                    GrammarModel.AddIgnoreSetting(new IgnoreSettingModel(ignoreRule));
                    GrammarModel.AddLexerRule(ignoreRule);
                }
            }

            if (trivia != null)
            {
                foreach (var triviaRule in trivia)
                {
                    GrammarModel.AddTriviaSetting(new TriviaSettingModel(triviaRule));
                    GrammarModel.AddLexerRule(triviaRule);
                }
            }
        }
    }
}