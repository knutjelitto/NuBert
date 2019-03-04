using System.Collections.Generic;
using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class ProductionModel : SymbolModel
    {
        public ProductionModel(NonTerminal leftHandSide)
            : this(new NonTerminalModel(leftHandSide))
        {
        }

        public ProductionModel(string leftHandSide)
            : this(new NonTerminal(leftHandSide))
        {
        }

        public ProductionModel(QualifiedName fullyQualifiedName)
            : this(new NonTerminalModel(fullyQualifiedName))
        {
        }

        private ProductionModel(NonTerminalModel leftHandSide)
            : base(leftHandSide.NonTerminal)
        {
            LeftHandSide = leftHandSide;
            Alterations = new List<AlterationModel>();
        }

        public List<AlterationModel> Alterations { get; }

        public NonTerminalModel LeftHandSide { get; }

        public void AddWithAnd(SymbolModel model)
        {
            if (Alterations.Count == 0)
            {
                AddWithOr(model);
            }
            else
            {
                Alterations[Alterations.Count - 1].AddSymbol(model);
            }
        }

        public void AddWithOr(SymbolModel model)
        {
            var alterationModel = new AlterationModel();
            alterationModel.AddSymbol(model);
            Alterations.Add(alterationModel);
        }

        public void Lambda()
        {
            Alterations.Add(new AlterationModel());
        }

        public IEnumerable<Production> ToProductions()
        {
            if (Alterations == null || Alterations.Count == 0)
            {
                yield return new Production(LeftHandSide.NonTerminal);

                yield break;
            }

            foreach (var alteration in Alterations)
            {
                var symbols = new List<ISymbol>();
                foreach (var symbolModel in alteration.Symbols)
                {
                    symbols.Add(symbolModel.Symbol);

                    if (symbolModel is GrammarReferenceModel grammarReferenceModel)
                    {
                        foreach (var production in grammarReferenceModel.Grammar.Productions)
                        {
                            yield return production;
                        }
                    }
                }

                yield return new Production(LeftHandSide.NonTerminal, symbols);
            }
        }
    }
}