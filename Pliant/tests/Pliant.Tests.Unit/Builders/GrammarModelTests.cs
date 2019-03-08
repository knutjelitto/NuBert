using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using Pliant.Grammars;
using Pliant.LexerRules;

namespace Pliant.Tests.Unit.Builders
{
    [TestClass]
    public class GrammarModelTests
    {
        [TestMethod]
        public void GrammarModelShouldAddProductionModel()
        {
            var grammar = new GrammarModel();
            grammar.AddProduction(ProductionModel.From(""));
            Assert.AreEqual(1, grammar.ProductionModels.Count);
        }

        [TestMethod]
        public void GrammarModelShouldAddIgnoreLexerRuleModel()
        {
            var grammar = new GrammarModel();
            var lexerRuleModel = new LexerRuleModel(new StringLiteralLexerRule("this is a literal"));
            grammar.AddIgnoreSetting(new IgnoreSettingModel(new QualifiedName("AAA")));
            grammar.AddLexerRule(lexerRuleModel);
            Assert.AreEqual(1, grammar.LexerRuleModels.Count);
            Assert.AreEqual(1, grammar.IgnoreSettings.Count);
        }

        [TestMethod]
        public void GrammarModelToGrammarShouldCreateGrammar()
        {
            var grammarModel = new GrammarModel();

            var S = ProductionModel.From("S");
            var A = ProductionModel.From("A");
            var B = ProductionModel.From("B");

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
            grammarModel.AddLexerRule(lexerRuleModel);
            grammarModel.AddIgnoreSetting(new IgnoreSettingModel(new QualifiedName(lexerRuleModel.LexerRule.TokenName.Id)));

            grammarModel.Start = S;

            var grammar = grammarModel.ToGrammar();
            Assert.AreEqual(4, grammar.Productions.Count);            
            Assert.AreEqual(1, grammar.Ignores.Count);
        }

        [TestMethod]
        public void GrammarModelToGrammarShouldResolverProductionReferencesFromOtherGrammars()
        {
            var S = ProductionModel.From(new QualifiedName("ns1", "S"));
            var A = ProductionModel.From(new QualifiedName("ns1", "A"));
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

            var ns1ProductionReference = new GrammarReferenceModel(ns1GrammarModel.ToGrammar());

            var Z = ProductionModel.From(new QualifiedName("ns2", "Z"));
            var X = ProductionModel.From(new QualifiedName("ns2", "X"));
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
            var S = ProductionModel.From("S");
            var grammarModel = new GrammarModel(S);
            var grammar = grammarModel.ToGrammar();
            Assert.AreEqual(1, grammar.Productions.Count);
        }

        [TestMethod]
        public void GrammarModelConstructorGivenOnlyStartProductionShouldDiscoverLinkedProductions()
        {
            var S = ProductionModel.From("S");
            var A = ProductionModel.From("A");
            var B = ProductionModel.From("B");
            var C = ProductionModel.From("C");
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
            var S = ProductionModel.From("S");
            var A = ProductionModel.From("A");
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
            var S = ProductionModel.From("S");
            var A = ProductionModel.From("A");
            var B = ProductionModel.From("B");

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
