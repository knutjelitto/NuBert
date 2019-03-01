﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Forest;
using Pliant.Automata;
using Pliant.Grammars;
using Pliant.RegularExpressions;
using Pliant.Tokens;
using Pliant.Builders.Expressions;
using Pliant.Builders;
using Pliant.Runtime;
using Pliant.Terminals;
using Pliant.Tests.Unit.Runtime;

// ReSharper disable once CheckNamespace
namespace Pliant.Tests.Common.Forest
{
    [TestClass]
    public class ForestNodeVisitorTests
    {
        private readonly LexerRule _whitespace;

        private static LexerRule CreateWhitespaceRule()
        {
            var start = DfaState.Inner();
            var end = DfaState.Final();
            start.AddTransition(WhitespaceTerminal.Instance, end);
            end.AddTransition(WhitespaceTerminal.Instance, end);
            return new DfaLexerRule(start, new TokenType("whitespace"));
        }

        public ForestNodeVisitorTests()
        {
            this._whitespace = CreateWhitespaceRule();
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
                    Assert.Fail($"error parsing input at position {regexLexer.Position}");
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
            ProductionExpression
                And = "AND",
                Panda = "Panda",
                AAn = "AAn",
                ShootsLeaves = "ShootsAndLeaves",
                EatsShootsLeaves = "EatsShootsLeaves";

            And.Rule = (Expr)'a' + 'n' + 'd';
            var and = new GrammarExpression(And, new[] { And }).ToGrammar();

            Panda.Rule = (Expr)'p' + 'a' + 'n' + 'd' + 'a';
            var panda = new GrammarExpression(Panda, new[] { Panda }).ToGrammar();

            AAn.Rule = (Expr)'a' | ((Expr)'a' + 'n');
            var aAn = new GrammarExpression(AAn, new[] { AAn }).ToGrammar();

            ShootsLeaves.Rule =
                (Expr)"shoots"
                | (Expr)"leaves";
            var shootsLeaves = new GrammarExpression(ShootsLeaves, new[] { ShootsLeaves }).ToGrammar();

            EatsShootsLeaves.Rule =
                ((Expr)'e' + 'a' + 't' + 's')
                | ((Expr)'s' + 'h' + 'o' + 'o' + 't' + 's')
                | ((Expr)'l' + 'e' + 'a' + 'v' + 'e' + 's');
            var eatsShootsLeaves = new GrammarExpression(EatsShootsLeaves, new[] { EatsShootsLeaves }).ToGrammar();

            ProductionExpression
                S = "S",
                NP = "NP",
                VP = "VP",
                NN = "NN",
                NNS = "NNS",
                DT = "DT",
                CC = "CC",
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
                new[] { S, NP, VP, CC, DT, NN, NNS, VBZ },
                new[] { new LexerRuleModel(this._whitespace) })
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
        }
    }
}