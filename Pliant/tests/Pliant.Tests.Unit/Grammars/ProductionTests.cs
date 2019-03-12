using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using Pliant.Terminals;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class ProductionTests
    {
        [TestMethod]
        public void ProductionToStringShouldGenerateCorrectlyFormattedString()
        {
            var production = Production.From(NonTerminal.From("A"), NonTerminal.From("B"));
            Assert.AreEqual("A -> B", production.ToString());
        }


        [TestMethod]
        public void ProductionGetHashCodeShouldProduceSameValueForEmptyProductionWithSameLeftHandSide()
        {
            var production1 = Production.From(NonTerminal.From("A"));
            var production2 = Production.From(NonTerminal.From("A"));
            Assert.AreEqual(production1.GetHashCode(), production2.GetHashCode());
        }

        [TestMethod]
        public void ProductionGetHashCodeShouldProduceSameValueForSameRightHandSides()
        {
            var production1 = Production.From(NonTerminal.From("A"), new CharacterTerminal('a'), NonTerminal.From("B"));
            var production2 = Production.From(NonTerminal.From("A"), new CharacterTerminal('a'), NonTerminal.From("B"));

            Assert.AreEqual(production1.GetHashCode(), production2.GetHashCode());
        }

        [TestMethod]
        public void ProductionGetHashCodeShouldNotProduceSameValueForDifferentLeftHandSides()
        {
            var production1 = Production.From(NonTerminal.From("A"));
            var production2 = Production.From(NonTerminal.From("B"));
            Assert.AreNotEqual(production1.GetHashCode(), production2.GetHashCode());
        }

        [TestMethod]
        public void ProductionGetHashCodeShouldProduceSameValueForSameObject()
        {
            var production = Production.From(
                NonTerminal.From("Z"),
                new CharacterTerminal('a'),
                NonTerminal.From("B"),
                new SetTerminal("az"));
            var hashCode = production.GetHashCode();
            Assert.AreEqual(hashCode, production.GetHashCode());
        }
    }
}