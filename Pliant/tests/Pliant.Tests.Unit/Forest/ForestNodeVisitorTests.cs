using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.Builders;
using Pliant.Builders.Expressions;
using Pliant.Forest;
using Pliant.Grammars;
using Pliant.LexerRules;
using Pliant.RegularExpressions;
using Pliant.Runtime;
using Pliant.Terminals;
using Pliant.Tests.Unit.Runtime;
using Pliant.Tokens;

// ReSharper disable once CheckNamespace
namespace Pliant.Tests.Common.Forest
{
    [TestClass]
    public class ForestNodeVisitorTests
    {
        public ForestNodeVisitorTests()
        {
            this.whitespace = CreateWhitespaceRule();
        }

        private static LexerRule CreateWhitespaceRule()
        {
            var start = DfaState.Inner();
            var end = DfaState.Final();
            start.AddTransition(WhitespaceTerminal.Instance, end);
            end.AddTransition(WhitespaceTerminal.Instance, end);
            return new DfaLexerRule(start, new TokenName("whitespace"));
        }

        [TestMethod]
        public void NodeVisitorShouldWalkSimpleRegex()
        {
            var regexGrammar = new RegexGrammar();
            var regexParseEngine = new ParseEngine(regexGrammar);
            var regexLexer = new ParseRunner(regexParseEngine, @"[(]\d[)]");
            while (!regexLexer.EndOfStream())
            {
                if (!regexLexer.Read())
                {
                    Assert.Fail($"error parsing input at position {regexLexer.Position}");
                }
            }

            Assert.IsTrue(regexParseEngine.IsAccepted());

            var nodeVisitor = new LoggingNodeVisitor(
                new SelectFirstChildDisambiguationAlgorithm());
            var root = regexParseEngine.GetParseForestRootNode();
            root.Accept(nodeVisitor);
            Assert.AreEqual(31, nodeVisitor.VisitLog.Count);
        }

        [TestMethod]
        public void NodeVisitorShouldEnumerateAllParseTrees()
        {
#if false
            ProductionExpression
                // ReSharper disable once InconsistentNaming
                And = "AND",
                // ReSharper disable once InconsistentNaming
                Panda = "Panda",
                // ReSharper disable once InconsistentNaming
                AAn = "AAn",
                // ReSharper disable once InconsistentNaming
                ShootsLeaves = "ShootsAndLeaves",
                // ReSharper disable once InconsistentNaming
                EatsShootsLeaves = "EatsShootsLeaves";

            And.Rule = (Expr) 'a' + 'n' + 'd';
            var and = new GrammarExpression(And, new[] {And}).ToGrammar();

            Panda.Rule = (Expr) 'p' + 'a' + 'n' + 'd' + 'a';
            var panda = new GrammarExpression(Panda, new[] {Panda}).ToGrammar();

            AAn.Rule = (Expr) 'a' | ((Expr) 'a' + 'n');
            var aAn = new GrammarExpression(AAn, new[] {AAn}).ToGrammar();

            ShootsLeaves.Rule =
                (Expr) "shoots"
                | (Expr) "leaves";
            var shootsLeaves = new GrammarExpression(ShootsLeaves, new[] {ShootsLeaves}).ToGrammar();

            EatsShootsLeaves.Rule =
                ((Expr) 'e' + 'a' + 't' + 's')
                | ((Expr) 's' + 'h' + 'o' + 'o' + 't' + 's')
                | ((Expr) 'l' + 'e' + 'a' + 'v' + 'e' + 's');
            var eatsShootsLeaves = new GrammarExpression(EatsShootsLeaves, new[] {EatsShootsLeaves}).ToGrammar();

            ProductionExpression
                // ReSharper disable once InconsistentNaming
                S = "S",
                // ReSharper disable once InconsistentNaming
                NP = "NP",
                // ReSharper disable once InconsistentNaming
                VP = "VP",
                // ReSharper disable once InconsistentNaming
                NN = "NN",
                // ReSharper disable once InconsistentNaming
                NNS = "NNS",
                // ReSharper disable once InconsistentNaming
                DT = "DT",
                // ReSharper disable once InconsistentNaming
                CC = "CC",
                // ReSharper disable once InconsistentNaming
                VBZ = "VBZ";

            S.Rule =
                NP + VP + '.';
            NP.Rule =
                NN
                | NNS
                | (DT + NN)
                | (NN + NNS)
                | (NNS + CC + NNS);
            VP.Rule = (VBZ + NP)
                      | (VP + VBZ + NNS)
                      | (VP + CC + VP)
                      | (VP + VP + CC + VP)
                      | VBZ;
            CC.Rule = new GrammarLexerRule(nameof(CC), and);
            DT.Rule = new GrammarLexerRule(nameof(DT), aAn);
            NN.Rule = new GrammarLexerRule(nameof(NN), panda);
            NNS.Rule = new GrammarLexerRule(nameof(NNS), shootsLeaves);
            VBZ.Rule = new GrammarLexerRule(nameof(VBZ), eatsShootsLeaves);

            var grammar = new GrammarExpression(
                    S,
                    new[] {S, NP, VP, CC, DT, NN, NNS, VBZ},
                    new[] {new LexerRuleModel(this.whitespace)})
                .ToGrammar();
            var sentence = "a panda eats shoots and leaves.";

            var parseEngine = new ParseEngine(grammar);
            var parseRunner = new ParseRunner(parseEngine, sentence);

            while (!parseRunner.EndOfStream())
            {
                Assert.IsTrue(parseRunner.Read(),
                    $"Error parsing position: {parseRunner.Position}");
            }

            Assert.IsTrue(parseRunner.ParseEngine.IsAccepted());
#endif
        }

        private readonly LexerRule whitespace;
    }
}