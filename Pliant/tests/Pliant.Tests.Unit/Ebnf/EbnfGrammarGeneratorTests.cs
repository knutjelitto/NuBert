using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Ebnf;
using Pliant.Grammars;
using Pliant.Builders.Expressions;
using Pliant.Runtime;
using Pliant.Tests.Common;
using Pliant.RegularExpressions;

namespace Pliant.Tests.Unit.Ebnf
{
    [TestClass]
    public class EbnfGrammarGeneratorTests
    {
        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForSimpleRule()
        {
            // S = 'a';
            var definition = new EbnfDefinitionSimple(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("S"),
                        new EbnfExpressionSimple(
                            new EbnfTermSimple(
                                new EbnfFactorLiteral("a"))))));
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(1, grammar.Productions.Count);
            Assert.AreEqual(1, grammar.Productions[0].Count);
        }

        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateQualifiedName()
        {
            // X.Y.Z = 'a';
            var definition = new EbnfDefinitionSimple(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("X", "Y", "Z"),
                        new EbnfExpressionSimple(
                            new EbnfTermSimple(
                                new EbnfFactorLiteral("a"))))));
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(1, grammar.Productions.Count);
            Assert.AreEqual(1, grammar.Productions[0].Count);
            Assert.IsTrue(grammar.Productions[0].LeftHandSide.QualifiedName.Is("X.Y.Z"));
        }

        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForMultipleProductions()
        {
            // S = 'a';
            // S = 'b';
            var definition = new EbnfDefinitionConcatenation(                
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("S"),
                        new EbnfExpressionSimple(
                            new EbnfTermSimple(
                                new EbnfFactorLiteral("a"))))),
                new EbnfDefinitionSimple(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifier("S"),
                            new EbnfExpressionSimple(
                                new EbnfTermSimple(
                                    new EbnfFactorLiteral("b")))))));
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(2, grammar.Productions.Count);
        }

        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForProductionAlteration()
        {
            // S = 'a' | 'b';
            var definition = new EbnfDefinitionSimple(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("S"),
                        new EbnfExpressionAlteration(
                            new EbnfTermSimple(
                                new EbnfFactorLiteral("a")),
                            new EbnfExpressionSimple(
                                new EbnfTermSimple(
                                    new EbnfFactorLiteral("d")))))));
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(2, grammar.Productions.Count);
        }

        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForMultipleProductionsWithAlterations()
        {
            // S = 'a' | 'd';
            // S = 'b' | 'c';
            var definition = new EbnfDefinitionConcatenation(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("S"),
                        new EbnfExpressionAlteration(
                            new EbnfTermSimple(
                                new EbnfFactorLiteral("a")),
                            new EbnfExpressionSimple(
                                new EbnfTermSimple(
                                    new EbnfFactorLiteral("d")))))),
                new EbnfDefinitionSimple(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifier("S"),
                            new EbnfExpressionAlteration(
                                new EbnfTermSimple(
                                    new EbnfFactorLiteral("b")),
                                new EbnfExpressionSimple(
                                    new EbnfTermSimple(
                                        new EbnfFactorLiteral("c"))))))));
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(4, grammar.Productions.Count);
        } 

        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForRepetition()
        {
            // R = { 'a' } ;
            var definition = new EbnfDefinitionSimple(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("R"),
                        new EbnfExpressionSimple(
                            new EbnfTermSimple(
                                new EbnfFactorRepetition(
                                    new EbnfExpressionSimple(
                                        new EbnfTermSimple(
                                            new EbnfFactorLiteral("a")))))))));

            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(3, grammar.Productions.Count);

            Assert.AreEqual(2, grammar.Productions[0].Count);
            Assert.AreEqual(0, grammar.Productions[1].Count);
            Assert.AreEqual(1, grammar.Productions[2].Count);
        }

        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForGrouping()
        {
            // R = ( 'a' );
            var definition = new EbnfDefinitionSimple(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("R"),
                        new EbnfExpressionSimple(
                            new EbnfTermSimple(
                                new EbnfFactorGrouping(
                                    new EbnfExpressionSimple(
                                        new EbnfTermSimple(
                                            new EbnfFactorLiteral("a")))))))));

            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(2, grammar.Productions.Count);
            Assert.AreEqual(1, grammar.Productions[0].Count);
            Assert.AreEqual(1, grammar.Productions[1].Count);
        }


        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForOptional()
        {
            // R = ['a']
            var definition = new EbnfDefinitionSimple(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("R"),
                        new EbnfExpressionSimple(
                            new EbnfTermSimple(
                                new EbnfFactorOptional(
                                    new EbnfExpressionSimple(
                                        new EbnfTermSimple(
                                            new EbnfFactorLiteral("a")))))))));

            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            
            ProductionExpression 
                R = "R",
                optA = "[a]";

            R.Rule = optA;
            optA.Rule = 'a'
                | Expr.Epsilon;

            var expectedGrammar = new GrammarExpression(R, new[] { R, optA }).ToGrammar();
            Assert.AreEqual(expectedGrammar.Productions.Count, grammar.Productions.Count);
        }

        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForMultipleOptionals()
        {
            // R = 'b' ['a'] 'c' ['d']
            var definition = new EbnfDefinitionSimple(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("R"),
                        new EbnfExpressionSimple(
                            new EbnfTermConcatenation(
                                new EbnfFactorLiteral("b"),
                                new EbnfTermConcatenation(
                                    new EbnfFactorOptional(
                                        new EbnfExpressionSimple(
                                            new EbnfTermSimple(
                                                new EbnfFactorLiteral("a")))),
                                    new EbnfTermConcatenation(
                                        new EbnfFactorLiteral("c"),
                                        new EbnfTermSimple(
                                            new EbnfFactorOptional(
                                                new EbnfExpressionSimple(
                                                    new EbnfTermSimple(
                                                        new EbnfFactorLiteral("d"))))))))))));
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);

            ProductionExpression 
                R = "R",
                optA = "[a]",
                optD = "[d]";

            R.Rule =
                (Expr)'b' + optA + 'c' + optD ;
            optA.Rule = 'a' | Expr.Epsilon;
            optD.Rule = 'd' | Expr.Epsilon;

            var expectedGrammar = new GrammarExpression(R, new[] { R, optA, optD }).ToGrammar();
            Assert.AreEqual(expectedGrammar.Productions.Count, grammar.Productions.Count);
        }

        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForComplexDefinition()
        {
            var ebnf = 
                @"file = ws directives ws ;
                ws = [ ows ] ; /* white space */
                ows = ""_""; /* obligatory white space */
                directives = directive { ows directive };
                directive = ""0"" | ""1""; ";

            var parser = new EbnfParser();
            var ebnfDefinition = parser.Parse(ebnf);
            var generatedGrammar = GenerateGrammar(ebnfDefinition);
            Assert.IsNotNull(generatedGrammar);
            var parseEngine = new ParseEngine(generatedGrammar, new ParseEngineOptions());
            var parseTester = new ParseTester(parseEngine);
            parseTester.RunParse("_0_1_0_0_1_1_");
        }


        [TestMethod]
        public void EbnfGeneratorShouldGenerateIgnores()
        {
            var whiteSpaceRegex = new Regex(
                false, 
                new RegexExpressionTerm(
                    new RegexTermFactor(
                        new RegexFactorIterator(                            
                            new RegexAtomSet(
                                new RegexSet(new RegexCharacterClass(
                                                 new RegexCharactersUnit(
                                                     new RegexCharacterClassCharacter(' '))),
                                             false)),
                            RegexIterator.OneOrMany))),
                false);
            var definition = new EbnfDefinitionConcatenation(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("S"),
                        new EbnfExpressionSimple(
                            new EbnfTermSimple(
                                new EbnfFactorLiteral("a"))))),
                new EbnfDefinitionConcatenation(
                    new EbnfBlockLexerRule(
                        new EbnfLexerRule(
                            new EbnfQualifiedIdentifier("whitespace"), 
                            new EbnfLexerRuleExpressionSimple(
                                new EbnfLexerRuleTermSimple(
                                    new EbnfLexerRuleFactorRegex(
                                        whiteSpaceRegex))))),
                    new EbnfDefinitionSimple(
                        new EbnfBlockSetting(
                            new EbnfSetting(
                                new EbnfSettingIdentifier(":ignore"),
                                new EbnfQualifiedIdentifier("whitespace"))))));
            
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar.Ignores);
            Assert.AreEqual(1, grammar.Ignores.Count);
        }

        [TestMethod]
        public void EbnfGeneratorShouldGenerateTrivia()
        {
            var whiteSpaceRegex = new Regex(
                false,
                new RegexExpressionTerm(
                    new RegexTermFactor(
                        new RegexFactorIterator(
                            new RegexAtomSet(
                                new RegexSet(new RegexCharacterClass(
                                                 new RegexCharactersUnit(
                                                     new RegexCharacterClassCharacter(' '))),
                                             false)),
                            RegexIterator.OneOrMany))),
                false);
            var definition = new EbnfDefinitionConcatenation(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("S"),
                        new EbnfExpressionSimple(
                            new EbnfTermSimple(
                                new EbnfFactorLiteral("a"))))),
                new EbnfDefinitionConcatenation(
                    new EbnfBlockLexerRule(
                        new EbnfLexerRule(
                            new EbnfQualifiedIdentifier("whitespace"),
                            new EbnfLexerRuleExpressionSimple(
                                new EbnfLexerRuleTermSimple(
                                    new EbnfLexerRuleFactorRegex(
                                        whiteSpaceRegex))))),
                    new EbnfDefinitionSimple(
                        new EbnfBlockSetting(
                            new EbnfSetting(
                                new EbnfSettingIdentifier(":trivia"),
                                new EbnfQualifiedIdentifier("whitespace"))))));

            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar.Trivia);
            Assert.AreEqual(1, grammar.Trivia.Count);
        }

        [TestMethod]
        public void EbnfGrammarGeneratorStartSettingShouldSetStartProduction()
        {
            // :start S;
            // S = 'a';
            var definition = new EbnfDefinitionConcatenation(
                new EbnfBlockSetting(
                    new EbnfSetting(
                        new EbnfSettingIdentifier(":start"),
                        new EbnfQualifiedIdentifier("S"))), 
                new EbnfDefinitionSimple(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("S"),
                        new EbnfExpressionSimple(
                            new EbnfTermSimple(
                                new EbnfFactorLiteral("a")))))));


            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(grammar.Start.QualifiedName.Name, "S");
        }

        private static Grammar GenerateGrammar(IEbnfDefinition definition)
        {
            var generator = new EbnfGrammarGenerator();
            return generator.Generate(definition);
        }

        private class GuidEbnfProductionNamingStrategy : IEbnfProductionNamingStrategy
        {
            public NonTerminal GetSymbolForOptional(EbnfFactorOptional optional)
            {
                return NonTerminal.From(Guid.NewGuid().ToString());
            }

            public NonTerminal GetSymbolForRepetition(EbnfFactorRepetition repetition)
            {
                return NonTerminal.From(Guid.NewGuid().ToString());
            }
        }
    }
}
