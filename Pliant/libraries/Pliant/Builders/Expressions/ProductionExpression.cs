using System.Collections.Generic;
using Pliant.Grammars;

namespace Pliant.Builders.Expressions
{
    public class ProductionExpression : BaseExpression
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
                    ProductionModel.Alterations.Add(
                        GetAlterationModelFromAlterationExpression(alteration));
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

        private static AlterationModel GetAlterationModelFromAlterationExpression(List<BaseExpression> symbols)
        {
            var alterationModel = new AlterationModel();
            foreach (var symbol in symbols)
            {
                if (symbol is ProductionExpression productionExpression)
                {
                    alterationModel.Symbols.Add(
                        productionExpression.ProductionModel);
                }
                else if (symbol is SymbolExpression symbolExpression)
                {
                    alterationModel.Symbols.Add(
                        symbolExpression.SymbolModel);
                }
                else if (symbol is ProductionReferenceExpression productionReferenceExpression)
                {
                    alterationModel.Symbols.Add(
                        productionReferenceExpression.ProductionReferenceModel);
                }
                else if (symbol is Expr expr)
                {
                    foreach (var symbolModel in GetSymbolModelListFromExpr(expr))
                    {
                        alterationModel.Symbols.Add(symbolModel);
                    }
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