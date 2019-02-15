using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Pliant.Builders
{
    public class GrammarModel
    {
        private readonly ObservableCollection<ProductionModel> _productions;
        private readonly List<LexerRuleModel> _lexerRules;

        private readonly List<TriviaSettingModel> _triviaSettings;
        private readonly List<IgnoreSettingModel> _ignoreSettings;

        public ICollection<ProductionModel> Productions => this._productions;

        public ICollection<TriviaSettingModel> TriviaSettings => this._triviaSettings;

        public ICollection<IgnoreSettingModel> IgnoreSettings => this._ignoreSettings;

        public ICollection<LexerRuleModel> LexerRules => this._lexerRules;

        ProductionModel _start;

        public ProductionModel Start
        {
            get => this._start;
            set
            {
                if (value != null)
                {
                    StartSetting = new StartProductionSettingModel(value);
                }

                this._start = value;
            }
        }

        public StartProductionSettingModel StartSetting { get; set; }

        private readonly ReachibilityMatrix _reachibilityMatrix;

        public GrammarModel()
        {
            this._reachibilityMatrix = new ReachibilityMatrix();

            this._productions = new ObservableCollection<ProductionModel>();
            this._productions.CollectionChanged += ProductionsCollectionChanged;
            this._lexerRules = new List<LexerRuleModel>();

            this._ignoreSettings = new List<IgnoreSettingModel>();
            this._triviaSettings = new List<TriviaSettingModel>();
        }

        public GrammarModel(ProductionModel start)
            : this()
        {
            Start = start;
        }
        
        void ProductionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach(var item in e.NewItems)
                    {
                        if (item is ProductionModel)
                        {
                            OnAddProduction(item as ProductionModel);
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.NewItems)
                    {
                        if (item is ProductionModel)
                        {
                            OnRemoveProduction(item as ProductionModel);
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
            this._reachibilityMatrix.RemoveProduction(productionModel);
        }

        private void OnAddProduction(ProductionModel productionModel)
        {
            this._reachibilityMatrix.AddProduction(productionModel);
        }

        private void OnResetProductions()
        {
            Start = null;
            this._reachibilityMatrix.ClearProductions();
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
                throw new Exception("Unable to generate Grammar. The grammar definition is missing a Left Hand Symbol to the Start production.");
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

                AssertStartProductionExistsForStartSymbol(this._reachibilityMatrix);
            }
            else if(StartSettingExists())
            {
                if (ProductionsAreEmpty())
                {
                    throw new InvalidOperationException("Unable to determine start symbol. No productions exist and a start symbol was not specified.");
                }

                AssertStartProductionexistsForStartSetting(this._reachibilityMatrix);
                Start = FindProduction(StartSetting.Value);
            }
            else { Start = this._reachibilityMatrix.GetStartProduction(); }
        }

        private ProductionModel FindProduction(string value)
        {
            for (var p = 0; p < this._productions.Count; p++)
            {
                var productionModel = this._productions[p];
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
            foreach (var productionModel in this._productions)
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
            return GetLexerRulesFromSettings(this._ignoreSettings);
        }

        private List<ILexerRule> GetTriviaRulesFromTriviaRulesModel()
        {
            return GetLexerRulesFromSettings(this._triviaSettings);
        }

        private List<ILexerRule> GetLexerRulesFromSettings(IReadOnlyList<SettingModel> settings)
        {
            var lexerRules = new List<ILexerRule>();
            for (var i = 0; i < settings.Count; i++)
            {
                var setting = settings[i];
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
            for (var i = 0; i < this._lexerRules.Count; i++)
            {
                var lexerRuleModel = this._lexerRules[i];
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
                    for (var s =0; s< alteration.Symbols.Count; s++)
                    {
                        var symbol = alteration.Symbols[s];
                        if (symbol.ModelType == SymbolModelType.Production)
                        {
                            PopulateMissingProductionsRecursively(symbol as ProductionModel, visited);
                        }
                    }
                }
            }
        }

        private void AssertStartProductionExistsForStartSymbol(ReachibilityMatrix reachibilityMatrix)
        {
            if (!reachibilityMatrix.ProudctionExistsForSymbol(Start.LeftHandSide))
            {
                throw new Exception("no start production found for start symbol");
            }
        }

        private void AssertStartProductionexistsForStartSetting(ReachibilityMatrix reachibilityMatrix)
        {
            if (!reachibilityMatrix.ProudctionExistsForSymbol(
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
