using System.Collections.Generic;
using Pliant.Collections;
using Pliant.Grammars;

namespace Pliant.Builders
{
    internal class ReachabilityMatrix
    {
        public ReachabilityMatrix()
        {
            this.matrix = new Dictionary<Symbol, UniqueList<NonTerminalModel>>();
            this.lookup = new Dictionary<Symbol, ProductionModel>();
        }

        public void AddProduction(ProductionModel production)
        {
            if (!this.matrix.ContainsKey(production.LeftHandSide.NonTerminal))
            {
                this.matrix[production.LeftHandSide.NonTerminal] = new UniqueList<NonTerminalModel>();
            }

            if (!this.lookup.ContainsKey(production.LeftHandSide.NonTerminal))
            {
                this.lookup[production.LeftHandSide.NonTerminal] = production;
            }

#if true
            foreach (var alteration in production.Alterations)
            {
                foreach (var symbol in alteration.Symbols)
                {
                    if (symbol is NonTerminalModel)
                    {
                        AddProductionToNewOrExistingSymbolSet(production, symbol);
                    }
                }
            }
#endif
        }

        public ProductionModel GetStartProduction()
        {
            foreach (var leftHandSide in this.matrix.Keys)
            {
                var symbolsReachableByLeftHandSide = this.matrix[leftHandSide];
                if (symbolsReachableByLeftHandSide.Count == 0)
                {
                    return this.lookup[leftHandSide];
                }
            }

            return null;
        }

        public bool ProductionExistsForSymbol(NonTerminalModel nonTerminalModel)
        {
            return this.matrix.ContainsKey(nonTerminalModel.NonTerminal);
        }

        private void AddProductionToNewOrExistingSymbolSet(ProductionModel production, SymbolModel symbol)
        {
            var set = this.matrix.AddOrGetExisting(symbol.Symbol);
            set.AddUnique(production.LeftHandSide);
        }

        private readonly Dictionary<Symbol, ProductionModel> lookup;
        private readonly Dictionary<Symbol, UniqueList<NonTerminalModel>> matrix;
    }
}