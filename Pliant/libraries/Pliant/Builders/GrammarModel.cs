using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Pliant.Grammars;

namespace Pliant.Builders
{
    public class GrammarModel
    {
        private readonly List<IgnoreSettingModel> _ignoreSettings;
        private readonly List<LexerRuleModel> _lexerRules;
        private readonly ObservableCollection<ProductionModel> _productions;

        // ReSharper disable once IdentifierTypo
        private readonly ReachabilityMatrix _reachabilityMatrix;

        private readonly List<TriviaSettingModel> _triviaSettings;

        ProductionModel _start;

        public GrammarModel()
        {
            _reachabilityMatrix = new ReachabilityMatrix();

            _productions = new ObservableCollection<ProductionModel>();
            _productions.CollectionChanged += ProductionsCollectionChanged;
            _lexerRules = new List<LexerRuleModel>();

            _ignoreSettings = new List<IgnoreSettingModel>();
            _triviaSettings = new List<TriviaSettingModel>();
        }

        public GrammarModel(ProductionModel start)
            : this()
        {
            Start = start;
        }

        public ICollection<ProductionModel> Productions => _productions;

        public ICollection<TriviaSettingModel> TriviaSettings => _triviaSettings;

        public ICollection<IgnoreSettingModel> IgnoreSettings => _ignoreSettings;

        public ICollection<LexerRuleModel> LexerRules => _lexerRules;

        public ProductionModel Start
        {
            get => _start;
            set
            {
                if (value != null)
                {
                    StartSetting = new StartProductionSettingModel(value);
                }

                _start = value;
            }
        }

        public StartProductionSettingModel StartSetting { get; set; }

        void ProductionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        if (item is ProductionModel model)
                        {
                            OnAddProduction(model);
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.NewItems)
                    {
                        if (item is ProductionModel model)
                        {
                            OnRemoveProduction(model);
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    OnResetProductions();
                    break;
            }
        }

        private void OnRemoveProduction(ProductionModel productionModel)
        {
            _reachabilityMatrix.RemoveProduction(productionModel);
        }

        private void OnAddProduction(ProductionModel productionModel)
        {
            _reachabilityMatrix.AddProduction(productionModel);
        }

        private void OnResetProductions()
        {
            Start = null;
            _reachabilityMatrix.ClearProductions();
        }

        public IGrammar ToGrammar()
        {
            SetStartProduction();

            var productions = GetProductionsFromProductionsModel();
            var ignoreRules = GetIgnoreRulesFromIgnoreRulesModel();
            var triviaRules = GetTriviaRulesFromTriviaRulesModel();

            if (Start == null)
            {
                throw new Exception("Unable to generate Grammar. The grammar definition is missing a Start production");
            }

            if (Start.LeftHandSide == null)
            {
                throw new Exception(
                    "Unable to generate Grammar. The grammar definition is missing a Left Hand Symbol to the Start production.");
            }

            return new Grammar(
                Start.LeftHandSide.NonTerminal,
                productions,
                ignoreRules,
                triviaRules);
        }

        private void SetStartProduction()
        {
            if (StartSymbolExists())
            {
                if (ProductionsAreEmpty())
                {
                    PopulateMissingProductionsFromStart(Start);
                }

                AssertStartProductionExistsForStartSymbol(_reachabilityMatrix);
            }
            else if (StartSettingExists())
            {
                if (ProductionsAreEmpty())
                {
                    throw new InvalidOperationException(
                        "Unable to determine start symbol. No productions exist and a start symbol was not specified.");
                }

                AssertStartProductionExistsForStartSetting(_reachabilityMatrix);
                Start = FindProduction(StartSetting.Value);
            }
            else
            {
                Start = _reachabilityMatrix.GetStartProduction();
            }
        }

        private ProductionModel FindProduction(string value)
        {
            foreach (var productionModel in _productions)
            {
                if (productionModel.LeftHandSide.NonTerminal.Value.Equals(value))
                {
                    return productionModel;
                }
            }

            return null;
        }

        private List<IProduction> GetProductionsFromProductionsModel()
        {
            var productions = new List<IProduction>();
            foreach (var productionModel in _productions)
            {
                foreach (var production in productionModel.ToProductions())
                {
                    productions.Add(production);
                }
            }

            return productions;
        }

        private List<ILexerRule> GetIgnoreRulesFromIgnoreRulesModel()
        {
            return GetLexerRulesFromSettings(_ignoreSettings);
        }

        private List<ILexerRule> GetTriviaRulesFromTriviaRulesModel()
        {
            return GetLexerRulesFromSettings(_triviaSettings);
        }

        private List<ILexerRule> GetLexerRulesFromSettings(IReadOnlyList<SettingModel> settings)
        {
            var lexerRules = new List<ILexerRule>();
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

        private ILexerRule GetLexerRuleByName(string value)
        {
            foreach (var lexerRuleModel in _lexerRules)
            {
                var lexerRule = lexerRuleModel.Value;
                if (lexerRule.TokenType.Id.Equals(value))
                {
                    return lexerRule;
                }
            }

            return null;
        }

        private void PopulateMissingProductionsFromStart(ProductionModel start)
        {
            var visited = new HashSet<INonTerminal>();
            PopulateMissingProductionsRecursively(start, visited);
        }

        private void PopulateMissingProductionsRecursively(ProductionModel production, ISet<INonTerminal> visited)
        {
            if (visited.Add(production.LeftHandSide.NonTerminal))
            {
                Productions.Add(production);
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

        // ReSharper disable once IdentifierTypo
        private void AssertStartProductionExistsForStartSymbol(ReachabilityMatrix reachabilityMatrix)
        {
            if (!reachabilityMatrix.ProductionExistsForSymbol(Start.LeftHandSide))
            {
                throw new Exception("no start production found for start symbol");
            }
        }

        // ReSharper disable once IdentifierTypo
        private void AssertStartProductionExistsForStartSetting(ReachabilityMatrix reachabilityMatrix)
        {
            if (!reachabilityMatrix.ProductionExistsForSymbol(
                new NonTerminalModel(StartSetting.Value)))
            {
                throw new Exception("no start production found for start symbol");
            }
        }

        private bool StartSymbolExists()
        {
            return Start != null;
        }

        private bool StartSettingExists()
        {
            return StartSetting != null;
        }

        private bool ProductionsAreEmpty()
        {
            return Productions.Count == 0;
        }
    }
}