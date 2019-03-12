using System.Collections.Generic;
using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class ProductionModel : SymbolModel
    {
        private ProductionModel(NonTerminalModel leftHandSide)
            : base(leftHandSide.NonTerminal)
        {
            LeftHandSide = leftHandSide;
            Alterations = new List<AlterationModel>();
        }

        public static ProductionModel From(string name)
        {
            return From(new QualifiedName(name));
        }

        public static ProductionModel From(QualifiedName name)
        {
            return From(NonTerminal.From(name));
        }

        public static ProductionModel From(NonTerminal nonTerminal)
        {
            return new ProductionModel(new NonTerminalModel(nonTerminal));
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
                yield return Production.From(LeftHandSide.NonTerminal);

                yield break;
            }

            foreach (var alteration in Alterations)
            {
                var symbols = new List<Symbol>();
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

                yield return Production.From(LeftHandSide.NonTerminal, symbols.ToArray());
            }
        }
    }
}