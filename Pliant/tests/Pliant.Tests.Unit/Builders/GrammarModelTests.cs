﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using Pliant.Grammars;

namespace Pliant.Tests.Unit.Builders
{
    [TestClass]
    public class GrammarModelTests
    {
        [TestMethod]
        public void GrammarModelShouldAddProductionModel()
        {
            var grammar = new GrammarModel();
            grammar.AddProduction(new ProductionModel(""));
            Assert.AreEqual(1, grammar.ProductionModels.Count);
        }

        [TestMethod]
        public void GrammarModelShouldAddIgnoreLexerRuleModel()
        {
            var grammar = new GrammarModel();
            var lexerRuleModel = new LexerRuleModel(new StringLiteralLexerRule("this is a literal"));
            grammar.IgnoreSettingModels.Add(
                new IgnoreSettingModel(
                    lexerRuleModel));
            grammar.LexerRuleModels.Add(lexerRuleModel);
            Assert.AreEqual(1, grammar.LexerRuleModels.Count);
            Assert.AreEqual(1, grammar.IgnoreSettingModels.Count);
        }

        [TestMethod]
        public void GrammarModelToGrammarShouldCreateGrammar()
        {
            var grammarModel = new GrammarModel();

            var S = new ProductionModel("S");
            var A = new ProductionModel("A");
            var B = new ProductionModel("B");

            var a = new StringLiteralLexerRule("a");
            var b = new StringLiteralLexerRule("b");
            var space = new StringLiteralLexerRule(" ");

            S.AddWithAnd(A.LeftHandSide);
            S.AddWithAnd(B.LeftHandSide);
            S.AddWithOr(B.LeftHandSide);
            A.AddWithAnd(new LexerRuleModel(a));
            B.AddWithAnd(new LexerRuleModel(b));

            grammarModel.AddProduction(S);
            grammarModel.AddProduction(A);
            grammarModel.AddProduction(B);

            var lexerRuleModel = new LexerRuleModel(space);
            grammarModel.LexerRuleModels.Add(lexerRuleModel);
            grammarModel.IgnoreSettingModels.Add(
                new IgnoreSettingModel(lexerRuleModel));

            grammarModel.Start = S;

            var grammar = grammarModel.ToGrammar();
            Assert.AreEqual(4, grammar.Productions.Count);            
            Assert.AreEqual(1, grammar.Ignores.Count);
        }

        [TestMethod]
        public void GrammarModelToGrammarShouldResolverProductionReferencesFromOtherGrammars()
        {
            var S = new ProductionModel(new QualifiedName("ns1", "S"));
            var A = new ProductionModel(new QualifiedName("ns1", "A"));
            S.Alterations.Add(
                new AlterationModel(
                    new[] { A }));
            A.Alterations.Add(
                new AlterationModel(
                    new[] { new LexerRuleModel(
                        new StringLiteralLexerRule("a"))})
            );
            var ns1GrammarModel = new GrammarModel
            {
                Start = S
            };
            ns1GrammarModel.AddProduction(S);
            ns1GrammarModel.AddProduction(A);

            var ns1ProductionReference = new ProductionReferenceModel(ns1GrammarModel.ToGrammar());

            var Z = new ProductionModel(new QualifiedName("ns2", "Z"));
            var X = new ProductionModel(new QualifiedName("ns2", "X"));
            X.Alterations.Add(
                new AlterationModel(
                    new SymbolModel[]
                    {
                        Z, ns1ProductionReference
                    }));

            var ns2GrammarModel = new GrammarModel
            {
                Start = Z
            };
            ns2GrammarModel.AddProduction(Z);
            ns2GrammarModel.AddProduction(X);

            var ns2Grammar = ns2GrammarModel.ToGrammar();

            Assert.AreEqual(4, ns2Grammar.Productions.Count);
        }

        [TestMethod]
        public void GrammarModelToGrammarShouldAddProductionWhenEmptyDefinition()
        {
            var S = new ProductionModel("S");
            var grammarModel = new GrammarModel(S);
            var grammar = grammarModel.ToGrammar();
            Assert.AreEqual(1, grammar.Productions.Count);
        }

        [TestMethod]
        public void GrammarModelConstructorGivenOnlyStartProductionShouldDiscoverLinkedProductions()
        {
            var S = new ProductionModel("S");
            var A = new ProductionModel("A");
            var B = new ProductionModel("B");
            var C = new ProductionModel("C");
            S.AddWithAnd(A);
            A.AddWithAnd(B);
            A.AddWithOr(C);
            B.AddWithAnd(new LexerRuleModel(new StringLiteralLexerRule("b")));
            C.AddWithAnd(new LexerRuleModel(new StringLiteralLexerRule("c")));

            var grammarModel = new GrammarModel(S);
            var grammar = grammarModel.ToGrammar();

            Assert.AreEqual(5, grammar.Productions.Count);
        }

        [TestMethod]
        public void GrammarModelConstructorGivenOnlyStartProductionShouldTraverseRecursiveStructureOnlyOnce()
        {
            var S = new ProductionModel("S");
            var A = new ProductionModel("A");
            S.AddWithAnd(S);
            S.AddWithOr(A);
            A.AddWithAnd(new LexerRuleModel(new StringLiteralLexerRule("a")));

            var grammarModel = new GrammarModel(S);
            var grammar = grammarModel.ToGrammar();

            Assert.AreEqual(3, grammar.Productions.Count);
        }

        [TestMethod]
        public void GrammarModelGivenNullStartShouldResolveStartFromProductions()
        {
            var S = new ProductionModel("S");
            var A = new ProductionModel("A");
            var B = new ProductionModel("B");

            S.AddWithAnd(A);
            S.AddWithAnd(B);
            A.AddWithAnd(new LexerRuleModel(new StringLiteralLexerRule("a")));
            A.AddWithAnd(B);
            B.AddWithAnd(new LexerRuleModel(new StringLiteralLexerRule("b")));

            var grammarModel = new GrammarModel();
            grammarModel.AddProduction(S);
            grammarModel.AddProduction(A);
            grammarModel.AddProduction(B);

            var grammar = grammarModel.ToGrammar();
            Assert.AreEqual(3, grammar.Productions.Count);
            Assert.IsNotNull(grammar.Start);
        }
    }
}
