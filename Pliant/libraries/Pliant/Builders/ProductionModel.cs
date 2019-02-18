using System.Collections.Generic;
using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class ProductionModel : SymbolModel
    {
        public ProductionModel()
        {
            Alterations = new List<AlterationModel>();
        }

        public ProductionModel(NonTerminalModel leftHandSide)
            : this()
        {
            LeftHandSide = leftHandSide;
            Alterations = new List<AlterationModel>();
        }

        public ProductionModel(NonTerminal leftHandSide)
            : this(new NonTerminalModel(leftHandSide))
        {
        }

        public ProductionModel(string leftHandSide)
            : this(new NonTerminal(leftHandSide))
        {
        }

        public ProductionModel(FullyQualifiedName fullyQualifiedName)
            : this(new NonTerminalModel(fullyQualifiedName))
        {
        }

        public List<AlterationModel> Alterations { get; }

        public NonTerminalModel LeftHandSide { get; set; }

        public override SymbolModelType ModelType => SymbolModelType.Production;

        public override Symbol Symbol => LeftHandSide.NonTerminal;

        public IEnumerable<Production> ToProductions()
        {
            if (Alterations == null || Alterations.Count == 0)
            {
                yield return new Production(LeftHandSide.NonTerminal);
                yield break;
            }

            foreach (var alteration in Alterations)
            {
                var symbols = new List<Symbol>();
                foreach (var symbolModel in alteration.Symbols)
                {
                    symbols.Add(symbolModel.Symbol);

                    if (symbolModel is ProductionReferenceModel productionReferenceModel)
                    {
                        foreach (var production in productionReferenceModel.Grammar.Productions)
                        {
                            yield return production;
                        }
                    }
                }

                yield return new Production(LeftHandSide.NonTerminal, symbols);
            }
        }

        public void AddWithAnd(SymbolModel model)
        {
            if (Alterations.Count == 0)
            {
                AddWithOr(model);
            }
            else
            {
                Alterations[Alterations.Count - 1].Symbols.Add(model);
            }
        }

        public void AddWithOr(SymbolModel model)
        {
            var alterationModel = new AlterationModel();
            alterationModel.Symbols.Add(model);
            Alterations.Add(alterationModel);
        }

        public void Lambda()
        {
            Alterations.Add(new AlterationModel());
        }
    }
}