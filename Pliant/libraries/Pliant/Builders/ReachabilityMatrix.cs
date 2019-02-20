using System.Collections.Generic;
using Pliant.Collections;
using Pliant.Grammars;

namespace Pliant.Builders
{
    internal class ReachabilityMatrix
    {
        public ReachabilityMatrix()
        {
            this._matrix = new Dictionary<Symbol, UniqueList<NonTerminalModel>>();
            this._lookup = new Dictionary<Symbol, ProductionModel>();
        }

        public void AddProduction(ProductionModel production)
        {
            if (!this._matrix.ContainsKey(production.LeftHandSide.NonTerminal))
            {
                this._matrix[production.LeftHandSide.NonTerminal] = new UniqueList<NonTerminalModel>();
            }

            if (!this._lookup.ContainsKey(production.LeftHandSide.NonTerminal))
            {
                this._lookup[production.LeftHandSide.NonTerminal] = production;
            }

#if false
            foreach (var alteration in production.Alterations)
            {
                foreach (var symbol in alteration.Symbols)
                {
                    if (symbol.ModelType == SymbolModelType.Production && symbol.ModelType == SymbolModelType.Reference)
                    {
                        AddProductionToNewOrExistingSymbolSet(production, symbol);
                    }
                }
            }
#endif
        }

        public void ClearProductions()
        {
            this._matrix.Clear();
            this._lookup.Clear();
        }

        public ProductionModel GetStartProduction()
        {
            foreach (var leftHandSide in this._matrix.Keys)
            {
                var symbolsReachableByLeftHandSide = this._matrix[leftHandSide];
                if (symbolsReachableByLeftHandSide.Count == 0)
                {
                    return this._lookup[leftHandSide];
                }
            }

            return null;
        }

        public bool ProductionExistsForSymbol(NonTerminalModel nonTerminalModel)
        {
            return this._matrix.ContainsKey(nonTerminalModel.NonTerminal);
        }

        public void RemoveProduction(ProductionModel productionModel)
        {
            if (!this._matrix.ContainsKey(productionModel.LeftHandSide.NonTerminal))
            {
                this._matrix.Remove(productionModel.LeftHandSide.NonTerminal);
            }

            if (!this._lookup.ContainsKey(productionModel.LeftHandSide.NonTerminal))
            {
                this._lookup.Remove(productionModel.LeftHandSide.NonTerminal);
            }
        }

        private void AddProductionToNewOrExistingSymbolSet(ProductionModel production, SymbolModel symbol)
        {
            var set = this._matrix.AddOrGetExisting(symbol.Symbol);
            set.AddUnique(production.LeftHandSide);
        }

        private readonly Dictionary<Symbol, ProductionModel> _lookup;
        private readonly Dictionary<Symbol, UniqueList<NonTerminalModel>> _matrix;
    }
}