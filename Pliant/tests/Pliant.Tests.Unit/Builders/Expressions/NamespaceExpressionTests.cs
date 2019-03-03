using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;

namespace Pliant.Tests.Unit.Builders.Expressions
{
    [TestClass]
    public class NamespaceExpressionTests
    {
        [TestMethod]
        public void NamespaceExpressionShouldCreateFullyQualifiedNameFromNamespaceExpressionAndNameString()
        {
            QualifierExpression ns1 = "namespace1";
            ProductionExpression
                S = ns1 + "S",
                A = ns1 + "A";
            S.Rule = A;
            A.Rule = 'a';

            var symbolS = S.ProductionModel.LeftHandSide;
            Assert.IsNotNull(symbolS);
            Assert.AreEqual(ns1.Qualifier, symbolS.NonTerminal.QualifiedName.Qualifier);
            Assert.AreEqual(S.ProductionModel.LeftHandSide.NonTerminal.QualifiedName.Name, symbolS.NonTerminal.QualifiedName.Name);

            var symbolA = A.ProductionModel.LeftHandSide;
            Assert.IsNotNull(symbolA);
            Assert.AreEqual(ns1.Qualifier, symbolA.NonTerminal.QualifiedName.Qualifier);
            Assert.AreEqual(A.ProductionModel.LeftHandSide.NonTerminal.QualifiedName.Name, symbolA.NonTerminal.QualifiedName.Name);
        }
    }
}
