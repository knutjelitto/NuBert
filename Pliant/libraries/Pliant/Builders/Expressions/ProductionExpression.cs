using System.Collections.Generic;
using Pliant.Grammars;

namespace Pliant.Builders.Expressions
{
    public sealed class ProductionExpression : BaseExpression
    {
        private ProductionExpression(NonTerminal leftHandSide)
        {
            ProductionModel = ProductionModel.From(leftHandSide);
        }

        private ProductionExpression(QualifiedName qualifiedName)
            : this(NonTerminal.From(qualifiedName))
        {
        }

        public ProductionModel ProductionModel { get; }

        public RuleExpression Rule
        {
            set
            {
                ProductionModel.Alterations.Clear();
                if (value == null)
                {
                    return;
                }

                foreach (var alteration in value.Alterations)
                {
                    ProductionModel.Alterations.Add(GetAlterationModelFromAlterationExpression(alteration));
                }
            }
        }

        public void Rules(RuleExpression value)
        {
            ProductionModel.Alterations.Clear();
            if (value == null)
            {
                return;
            }

            foreach (var alteration in value.Alterations)
            {
                ProductionModel.Alterations.Add(GetAlterationModelFromAlterationExpression(alteration));
            }
        }

        public static implicit operator ProductionExpression(string leftHandSide)
        {
            return new ProductionExpression(NonTerminal.From(leftHandSide));
        }

        public static implicit operator ProductionExpression(QualifiedName fullyQualifiedName)
        {
            return new ProductionExpression(fullyQualifiedName);
        }

        private static AlterationModel GetAlterationModelFromAlterationExpression(IEnumerable<BaseExpression> symbols)
        {
            var alterationModel = new AlterationModel();
            foreach (var symbol in symbols)
            {
                switch (symbol)
                {
                    case ProductionExpression productionExpression:
                        alterationModel.AddSymbol(productionExpression.ProductionModel);
                        break;
                    case SymbolExpression symbolExpression:
                        alterationModel.AddSymbol(symbolExpression.SymbolModel);
                        break;
                    case GrammarReferenceExpression grammarReferenceExpression:
                        alterationModel.AddSymbol(grammarReferenceExpression.GrammarReferenceModel);
                        break;
                    case RuleExpression expr:
                        foreach (var symbolModel in GetSymbolModelsFromExpr(expr))
                        {
                            alterationModel.AddSymbol(symbolModel);
                        }

                        break;
                }
            }

            return alterationModel;
        }

        private static IEnumerable<SymbolModel> GetSymbolModelsFromExpr(RuleExpression expr)
        {
            foreach (var alteration in expr.Alterations)
            {
                foreach (var expression in alteration)
                {
                    if (expression is SymbolExpression symbolExpression)
                    {
                        yield return symbolExpression.SymbolModel;
                    }
                }
            }
        }
    }
}