using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.Json;
using Pliant.LexerRules;
using Pliant.RegularExpressions;
using Pliant.Tests.Common.Grammars;

namespace Pliant.Tests.Unit.Grammars
{
    [TestClass]
    public class PreComputedGrammarTests
    {
        private static Grammar jsonGrammar;

        [ClassInitialize]
        public static void InitializeClass(TestContext testContext)
        {
            jsonGrammar = new JsonGrammar();
        }

        [TestMethod]
        public void PreComputedGrammarShouldLoadExpressionGrammar()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new PreComputedGrammar(new ExpressionGrammar());
        }

        [TestMethod]
        public void PreComputedGrammarShouldLoadNullableGrammar()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new PreComputedGrammar(new NullableGrammar());
        }        

        [TestMethod]
        public void PreComputedGrammarShouldLoadJsonGrammar()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new PreComputedGrammar(jsonGrammar);
        }

        [TestMethod]
        public void PreComputedGrammarIsRightRecursiveShouldFindSimpleRecursion()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new PreComputedGrammar(new RightRecursionGrammar());
        }

        [TestMethod]
        public void PreComputedGrammarIsRightRecursiveShouldFindCyclicRecursion()
        {
            var grammar = new HiddenRightRecursionGrammar();
            var preComputedGrammar = new PreComputedGrammar(grammar);

            foreach (var production in grammar.Productions)
            {
                Assert.IsTrue(preComputedGrammar.Grammar.IsRightRecursive(production.LeftHandSide));
            }
        }

        [TestMethod]
        public void PreComputedGrammarIsRightRecursiveShouldNotContainSymbolsWithoutCycles()
        {
            ProductionExpression
                A = "A",
                B = "B",
                C = "C",
                D = "D",
                E = "E";

            A.Rule = B + C;
            B.Rule = 'b';
            C.Rule = A | D;
            D.Rule = (E + D) | 'd';
            E.Rule = 'e';

            var grammar = new GrammarExpression(A).ToGrammar();
            var preComputedGrammar = new PreComputedGrammar(grammar);

            var rightRecursiveRules = new[] { A, C, D };
            var notRightRecursiveRules = new[] { B, E };

            foreach(var rightRecursiveRule in rightRecursiveRules)
            {
                var leftHandSide = rightRecursiveRule.ProductionModel.LeftHandSide.NonTerminal;
                Assert.IsTrue(preComputedGrammar.Grammar.IsRightRecursive(leftHandSide));
            }

            foreach (var notRightRecursiveRule in notRightRecursiveRules)
            {
                var leftHandSide = notRightRecursiveRule.ProductionModel.LeftHandSide.NonTerminal;
                Assert.IsFalse(preComputedGrammar.Grammar.IsRightRecursive(leftHandSide));
            }
        }
    }
}
