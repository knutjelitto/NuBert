using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Terminals;

namespace Pliant.Tests.Unit
{
    /// <summary>
    /// Summary description for ChartTests
    /// </summary>
    [TestClass]
    public class ChartTests
    {
        [TestMethod]
        public void ChartEnqueShouldAvoidDuplication()
        {
            ProductionExpression L = "L";
            var aToZ = new RangeTerminal('a', 'z');
            L.Rule = (L + aToZ) | aToZ;
            var grammar = new GrammarExpression(L, new[] { L }).ToGrammar();
            var chart = new Chart();
            var factory = new StateFactory(grammar.DottedRules);
            var dottedRule = grammar.DottedRules.Get(grammar.Productions[0], 0);
            var firstState = factory.NewState(dottedRule, 1);
            var secondState = factory.NewState(dottedRule, 1);
            chart.Enqueue(0, firstState);
            chart.Enqueue(0, secondState);
            Assert.AreEqual(1, chart[0].Predictions.Count);
        }
    }
}