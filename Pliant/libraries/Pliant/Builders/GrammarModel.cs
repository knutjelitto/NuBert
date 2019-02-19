﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Pliant.Grammars;

namespace Pliant.Builders
{
    public class GrammarModel
    {
        public GrammarModel()
        {
            this._reachabilityMatrix = new ReachabilityMatrix();

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

        public ICollection<IgnoreSettingModel> IgnoreSettings => this._ignoreSettings;

        public ICollection<LexerRuleModel> LexerRules => this._lexerRules;

        public ICollection<ProductionModel> Productions => this._productions;

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

        public ICollection<TriviaSettingModel> TriviaSettings => this._triviaSettings;

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

        // ReSharper disable once IdentifierTypo
        private void AssertStartProductionExistsForStartSetting(ReachabilityMatrix reachabilityMatrix)
        {
            if (!reachabilityMatrix.ProductionExistsForSymbol(
                    new NonTerminalModel(StartSetting.Value)))
            {
                throw new Exception("no start production found for start symbol");
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

        private ProductionModel FindProduction(string value)
        {
            foreach (var productionModel in this._productions)
            {
                if (productionModel.LeftHandSide.NonTerminal.Value.Equals(value))
                {
                    return productionModel;
                }
            }

            return null;
        }

        private List<LexerRule> GetIgnoreRulesFromIgnoreRulesModel()
        {
            return GetLexerRulesFromSettings(this._ignoreSettings);
        }

        private LexerRule GetLexerRuleByName(string value)
        {
            foreach (var lexerRuleModel in this._lexerRules)
            {
                var lexerRule = lexerRuleModel.Value;
                if (lexerRule.TokenType.Id.Equals(value))
                {
                    return lexerRule;
                }
            }

            return null;
        }

        private List<LexerRule> GetLexerRulesFromSettings(IReadOnlyList<SettingModel> settings)
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

        private List<LexerRule> GetTriviaRulesFromTriviaRulesModel()
        {
            return GetLexerRulesFromSettings(this._triviaSettings);
        }

        private void OnAddProduction(ProductionModel productionModel)
        {
            this._reachabilityMatrix.AddProduction(productionModel);
        }

        private void OnRemoveProduction(ProductionModel productionModel)
        {
            this._reachabilityMatrix.RemoveProduction(productionModel);
        }

        private void OnResetProductions()
        {
            Start = null;
            this._reachabilityMatrix.ClearProductions();
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

        private bool ProductionsAreEmpty()
        {
            return Productions.Count == 0;
        }

        private void ProductionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        private void SetStartProduction()
        {
            if (StartSymbolExists())
            {
                if (ProductionsAreEmpty())
                {
                    PopulateMissingProductionsFromStart(Start);
                }

                AssertStartProductionExistsForStartSymbol(this._reachabilityMatrix);
            }
            else if (StartSettingExists())
            {
                if (ProductionsAreEmpty())
                {
                    throw new InvalidOperationException(
                        "Unable to determine start symbol. No productions exist and a start symbol was not specified.");
                }

                AssertStartProductionExistsForStartSetting(this._reachabilityMatrix);
                Start = FindProduction(StartSetting.Value);
            }
            else
            {
                Start = this._reachabilityMatrix.GetStartProduction();
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

        private readonly List<IgnoreSettingModel> _ignoreSettings;
        private readonly List<LexerRuleModel> _lexerRules;
        private readonly ObservableCollection<ProductionModel> _productions;

        // ReSharper disable once IdentifierTypo
        private readonly ReachabilityMatrix _reachabilityMatrix;

        private ProductionModel _start;

        private readonly List<TriviaSettingModel> _triviaSettings;
    }
}