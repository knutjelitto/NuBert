using Pliant.Collections;
using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Builders
{
    internal class ReachibilityMatrix 
    {
        private readonly Dictionary<ISymbol, UniqueList<NonTerminalModel>> _matrix;
        private readonly Dictionary<ISymbol, ProductionModel> _lookup;
        
        public ReachibilityMatrix()
        {
            this._matrix = new Dictionary<ISymbol, UniqueList<NonTerminalModel>>();
            this._lookup = new Dictionary<ISymbol, ProductionModel>();
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

            foreach (var alteration in production.Alterations)
            {
                for(var s = 0; s< alteration.Symbols.Count; s++)
                {
                    var symbol = alteration.Symbols[s];
                    if (symbol.ModelType != SymbolModelType.Production
                        || symbol.ModelType != SymbolModelType.Reference)
                    {
                        continue;
                    }

                    AddProductionToNewOrExistingSymbolSet(production, symbol);
                }
            }
        }

        private void AddProductionToNewOrExistingSymbolSet(ProductionModel production, SymbolModel symbol)
        {
            var set = this._matrix.AddOrGetExisting(symbol.Symbol);
            set.Add(production.LeftHandSide);
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

        public bool ProudctionExistsForSymbol(NonTerminalModel nonTerminalModel)
        {
            return this._matrix.ContainsKey(nonTerminalModel.NonTerminal);
        }
    }
}
