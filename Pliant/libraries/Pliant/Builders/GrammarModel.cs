using System;
using System.Collections.Generic;
using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class GrammarModel
    {
        public GrammarModel()
        {
            this.reachabilityMatrix = new ReachabilityMatrix();

            this.productionModels = new List<ProductionModel>();
            this.lexerRuleModels = new List<LexerRuleModel>();

            this.ignoreSettings = new List<IgnoreSettingModel>();
            this.triviaSettings = new List<TriviaSettingModel>();
        }

        public GrammarModel(ProductionModel start)
            : this()
        {
            Start = start;
        }

        public IReadOnlyCollection<IgnoreSettingModel> IgnoreSettings => this.ignoreSettings;

        public IReadOnlyCollection<LexerRuleModel> LexerRuleModels => this.lexerRuleModels;

        public IReadOnlyCollection<ProductionModel> ProductionModels => this.productionModels;

        public IReadOnlyCollection<TriviaSettingModel> TriviaSettings => this.triviaSettings;


        public void AddProduction(ProductionModel productionModel)
        {
            this.productionModels.Add(productionModel);
            this.reachabilityMatrix.AddProduction(productionModel);

        }

        public void AddLexerRule(LexerRuleModel lexerRule)
        {
            this.lexerRuleModels.Add(lexerRule);
        }

        public void AddIgnoreSetting(IgnoreSettingModel ignore)
        {
            this.ignoreSettings.Add(ignore);
        }

        public void AddTriviaSetting(TriviaSettingModel trivia)
        {
            this.triviaSettings.Add(trivia);
        }

        public ProductionModel Start
        {
            get => this.startProduction;
            set
            {
                if (value != null)
                {
                    StartSetting = new StartProductionSettingModel(value);
                }

                this.startProduction = value;
            }
        }

        public StartProductionSettingModel StartSetting { get; set; }

        public Grammar ToGrammar()
        {
            SetStartProduction();

            var productions = GetProductionsFromProductionsModel();
            var ignoreRules = GetIgnoreRulesFromIgnoreRulesModel();
            var triviaRules = GetTriviaRulesFromTriviaRulesModel();

            if (Start == null)
            {
                throw new Exception("Unable to generate Grammar. The grammar definition is missing a StartState production");
            }

            if (Start.LeftHandSide == null)
            {
                throw new Exception(
                    "Unable to generate Grammar. The grammar definition is missing a Left Hand Symbol to the StartState production.");
            }

            return new ConcreteGrammar(
                Start.LeftHandSide.NonTerminal,
                productions,
                ignoreRules,
                triviaRules);
        }

        // ReSharper disable once IdentifierTypo
        private void AssertStartProductionExistsForStartSetting()
        {
            if (!this.reachabilityMatrix.ProductionExistsForSymbol(new NonTerminalModel(NonTerminal.From(StartSetting.Value))))
            {
                throw new Exception("no start production found for start symbol");
            }
        }

        // ReSharper disable once IdentifierTypo
        private void AssertStartProductionExistsForStartSymbol()
        {
            if (!this.reachabilityMatrix.ProductionExistsForSymbol(Start.LeftHandSide))
            {
                throw new Exception("no start production found for start symbol");
            }
        }

        private ProductionModel FindProduction(QualifiedName name)
        {
            foreach (var productionModel in this.productionModels)
            {
                if (productionModel.LeftHandSide.NonTerminal.Is(name))
                {
                    return productionModel;
                }
            }

            return null;
        }

        private List<LexerRule> GetIgnoreRulesFromIgnoreRulesModel()
        {
            return GetLexerRulesFromSettings(this.ignoreSettings);
        }

        private LexerRule GetLexerRuleByName(QualifiedName value)
        {
            foreach (var lexerRuleModel in this.lexerRuleModels)
            {
                var lexerRule = lexerRuleModel.LexerRule;
                if (lexerRule.TokenName.Id.Equals(value.FullName))
                {
                    return lexerRule;
                }
            }

            return null;
        }

        private List<LexerRule> GetLexerRulesFromSettings(IEnumerable<SettingModel> settings)
        {
            var lexerRules = new List<LexerRule>();
            foreach (var setting in settings)
            {
                var lexerRule = GetLexerRuleByName(setting.Value);
                if (lexerRule == null)
                {
                    throw new Exception($"lexer rule {setting.Value} not found.");
                }

                lexerRules.Add(lexerRule);
            }

            return lexerRules;
        }

        private List<Production> GetProductionsFromProductionsModel()
        {
            var productions = new List<Production>();
            foreach (var productionModel in this.productionModels)
            {
                foreach (var production in productionModel.ToProductions())
                {
                    productions.Add(production);
                }
            }

            return productions;
        }

        private List<LexerRule> GetTriviaRulesFromTriviaRulesModel()
        {
            return GetLexerRulesFromSettings(this.triviaSettings);
        }

        private void PopulateMissingProductionsFromStart(ProductionModel start)
        {
            var visited = new HashSet<NonTerminal>();
            PopulateMissingProductionsRecursively(start, visited);
        }

        private void PopulateMissingProductionsRecursively(ProductionModel production, ISet<NonTerminal> visited)
        {
            if (visited.Add(production.LeftHandSide.NonTerminal))
            {
                AddProduction(production);
                foreach (var alteration in production.Alterations)
                {
                    foreach (var symbol in alteration.Symbols)
                    {
                        if (symbol is ProductionModel productionModel)
                        {
                            PopulateMissingProductionsRecursively(productionModel, visited);
                        }
                    }
                }
            }
        }

        private bool ProductionsAreEmpty()
        {
            return ProductionModels.Count == 0;
        }

        private void SetStartProduction()
        {
            if (StartSymbolExists())
            {
                if (ProductionsAreEmpty())
                {
                    PopulateMissingProductionsFromStart(Start);
                }

                AssertStartProductionExistsForStartSymbol();
            }
            else if (StartSettingExists())
            {
                if (ProductionsAreEmpty())
                {
                    throw new InvalidOperationException(
                        "Unable to determine start symbol. No productions exist and a start symbol was not specified.");
                }

                AssertStartProductionExistsForStartSetting();
                Start = FindProduction(StartSetting.Value);
            }
            else
            {
                Start = this.reachabilityMatrix.GetStartProduction();
            }
        }

        private bool StartSettingExists()
        {
            return StartSetting != null;
        }

        private bool StartSymbolExists()
        {
            return Start != null;
        }

        private readonly List<IgnoreSettingModel> ignoreSettings;
        private readonly List<TriviaSettingModel> triviaSettings;
        private readonly List<LexerRuleModel> lexerRuleModels;
        private readonly List<ProductionModel> productionModels;

        // ReSharper disable once IdentifierTypo
        private readonly ReachabilityMatrix reachabilityMatrix;

        private ProductionModel startProduction;
    }
}