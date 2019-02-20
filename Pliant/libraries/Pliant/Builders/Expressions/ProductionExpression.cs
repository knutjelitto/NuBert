using System.Collections.Generic;
using Pliant.Grammars;

namespace Pliant.Builders.Expressions
{
    public sealed class ProductionExpression : BaseExpression
    {
        public ProductionExpression(NonTerminal leftHandSide)
        {
            ProductionModel = new ProductionModel(leftHandSide);
        }

        public ProductionExpression(QualifiedName fullyQualifiedName)
        {
            ProductionModel = new ProductionModel(fullyQualifiedName);
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

        public static implicit operator ProductionExpression(string leftHandSide)
        {
            return new ProductionExpression(new NonTerminal(leftHandSide));
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
                        alterationModel.Symbols.Add(productionExpression.ProductionModel);
                        break;
                    case SymbolExpression symbolExpression:
                        alterationModel.Symbols.Add(symbolExpression.SymbolModel);
                        break;
                    case ProductionReferenceExpression productionReferenceExpression:
                        alterationModel.Symbols.Add(productionReferenceExpression.ProductionReferenceModel);
                        break;
                    case Expr expr:
                        foreach (var symbolModel in GetSymbolModelListFromExpr(expr))
                        {
                            alterationModel.Symbols.Add(symbolModel);
                        }

                        break;
                }
            }

            return alterationModel;
        }

        private static IEnumerable<SymbolModel> GetSymbolModelListFromExpr(Expr expr)
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