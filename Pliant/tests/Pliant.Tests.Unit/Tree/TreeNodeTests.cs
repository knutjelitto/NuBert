using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Tree;
using Pliant.Builders.Expressions;
using Pliant.Runtime;

namespace Pliant.Tests.Unit.Tree
{
    [TestClass]
    public class TreeNodeTests
    {
        [TestMethod]
        public void TreeNodeShouldFlattenIntermediateNodes()
        {
            ProductionExpression S = "S", A = "A", B = "B", C = "C";
            S.Rule = A + B + C;
            A.Rule = 'a';
            B.Rule = 'b';
            C.Rule = 'c';
            var grammar = new GrammarExpression(S, new[] { S, A, B, C }).ToGrammar();
            var input = "abc";
            var treeNode = GetTreeNode(grammar, input);
            var childCount = 0;
            foreach (var child in treeNode.Children)
            {
                childCount++;
                if (child is IInternalTreeNode internalChild)
                {
                    var grandChildCount = 0;
                    foreach (var grandChild in internalChild.Children)
                    {
                        grandChildCount++;
                        Assert.IsInstanceOfType(grandChild, typeof(ITokenTreeNode));
                    }

                    Assert.AreEqual(1, grandChildCount);
                }
                else
                {
                    Assert.Fail();
                }
            }
            Assert.AreEqual(3, childCount);
        }

        [TestMethod]
        public void TreeNodeWhenAmbiguousParseShouldReturnFirstParseTree()
        {
            ProductionExpression A = "A";
            A.Rule = (A + '+' + A)
                     | (A + '-' + A)
                     | 'a';
            var grammar = new GrammarExpression(A, new[] { A }).ToGrammar();

            var input = "a+a+a";
            var treeNode = GetTreeNode(grammar, input);
        }

        private static InternalTreeNode GetTreeNode(Grammar grammar, string input)
        {
            var parseEngine = new ParseEngine(grammar);
            var parseRunner = new ParseRunner(parseEngine, input);
            while (!parseRunner.EndOfStream())
            {
                Assert.IsTrue(parseRunner.Read());
            }
            Assert.IsTrue(parseEngine.IsAccepted());
            
            var parseForest = parseEngine.GetParseForestRootNode();

            Assert.IsNotNull(parseForest);

            var internalNode = parseForest;

            var treeNode = new InternalTreeNode(internalNode);
            return treeNode;            
        }
    }
}