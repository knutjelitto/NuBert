using System.Collections.Generic;
using Pliant.Grammars;

namespace Pliant.Builders.Expressions
{
    public class GrammarExpression
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
            IReadOnlyList<LexerRule> ignore,
            IReadOnlyList<LexerRule> trivia)
        {
            var ignoreModelList = ToLexerRuleModelList(ignore);
            var triviaModelList = ToLexerRuleModelList(trivia);

            Initialize(start, productions, ignoreModelList, triviaModelList);
        }

        public GrammarModel GrammarModel { get; private set; }

        private static List<LexerRuleModel> ToLexerRuleModelList(IReadOnlyList<LexerRule> lexerRuleList)
        {
            if (lexerRuleList == null || lexerRuleList.Count == 0)
            {
                return null;
            }

            var modelList = new List<LexerRuleModel>();

            foreach (var lexerRule in lexerRuleList)
            {
                modelList.Add(new LexerRuleModel(lexerRule));
            }

            return modelList;
        }

        private void Initialize(ProductionExpression start, IReadOnlyList<ProductionExpression> productions,
            IReadOnlyList<LexerRuleModel> ignore, IReadOnlyList<LexerRuleModel> trivia)
        {
            GrammarModel = new GrammarModel
            {
                Start = start.ProductionModel
            };

            if (productions != null)
            {
                foreach (var production in productions)
                {
                    GrammarModel.Productions.Add(production.ProductionModel);
                }
            }

            if (ignore != null)
            {
                foreach (var ignoreRule in ignore)
                {
                    GrammarModel.IgnoreSettings.Add(
                        new IgnoreSettingModel(ignoreRule));

                    GrammarModel.LexerRules.Add(
                        ignoreRule);
                }
            }

            if (trivia != null)
            {
                foreach (var triviaRule in trivia)
                {
                    GrammarModel.TriviaSettings.Add(
                        new TriviaSettingModel(triviaRule));
                    GrammarModel.LexerRules.Add(
                        triviaRule);
                }
            }
        }

        public IGrammar ToGrammar()
        {
            return GrammarModel.ToGrammar();
        }
    }
}