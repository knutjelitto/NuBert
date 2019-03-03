using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Ebnf;
using Pliant.RegularExpressions;
using Pliant.Runtime;
using Pliant.Tests.Common.Forest;
using System;
using System.Text;

namespace Pliant.Tests.Unit.Ebnf
{
    [TestClass]
    public class EbnfParserTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void EbnfParserShouldNotParseEmptyRule()
        {
            var ebnf = Parse(@"");
            Assert.IsNotNull(ebnf);
        }

        [TestMethod]
        public void EbnfParserShouldParseCharacterProduction()
        {
            var expected = new EbnfDefinitionSimple(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifier("Rule"),
                            new EbnfExpressionSimple(
                                new EbnfTermSimple(
                                    new EbnfFactorLiteral("a"))))));

            var actual = Parse(@"Rule = 'a';");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseConcatenation()
        {
            var expected = new EbnfDefinitionSimple(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("Rule"),
                        new EbnfExpressionSimple(
                            new EbnfTermConcatenation(
                                new EbnfFactorLiteral("a"),
                                new EbnfTermSimple(
                                    new EbnfFactorLiteral("b")))))));

            var actual =  Parse(@"Rule = 'a' 'b';");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseAlteration()
        {
            var expected = new EbnfDefinitionSimple(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("Rule"),
                        new EbnfExpressionAlteration(
                            new EbnfTermSimple(
                                new EbnfFactorLiteral("a")),
                            new EbnfExpressionSimple(
                                new EbnfTermSimple(
                                    new EbnfFactorLiteral("b")))))));
            var actual =  Parse(@"Rule = 'a' | 'b';");
            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void EbnfParserShouldParseAlterationAndConcatenation()
        {
            var expected = new EbnfDefinitionSimple(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("Rule"),
                        new EbnfExpressionAlteration(
                            new EbnfTermConcatenation(
                                new EbnfFactorLiteral("a"),
                                new EbnfTermSimple(
                                    new EbnfFactorLiteral("b"))),
                            new EbnfExpressionSimple(
                                new EbnfTermSimple(
                                    new EbnfFactorLiteral("c")))))));
            var actual =  Parse(@"Rule = 'a' 'b' | 'c';");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseRegularExpression()
        {
            var expected = new EbnfDefinitionSimple(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifier("Rule"),
                            new EbnfExpressionSimple(
                                new EbnfTermSimple(
                                    new EbnfFactorRegex(
                                        new Regex(
                                            false, 
                                            new RegexExpressionTerm(
                                                new RegexTermFactor(
                                                    new RegexFactorAtom(
                                                        new RegexAtomSet(
                                                            new RegexSet(new RegexCharacterClass(
                                                                             new RegexCharactersRange(
                                                                                 new RegexCharacterClassCharacter('a'),
                                                                                 new RegexCharacterClassCharacter('z'))),
                                                                         false))))), 
                                            false)))))));

            var actual = Parse(@"Rule = /[a-z]/;");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseRepetition()
        {
            var actual =  Parse(@"Rule = { 'a' };");

            var expected = new EbnfDefinitionSimple(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifier("Rule"),
                            new EbnfExpressionSimple(
                                new EbnfTermSimple(
                                    new EbnfFactorRepetition(
                                        new EbnfExpressionSimple(
                                            new EbnfTermSimple(
                                                new EbnfFactorLiteral("a")))))))));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseOptional()
        {
            var actual =  Parse(@"Rule = [ 'a' ];");

            var expected = new EbnfDefinitionSimple(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifier("Rule"),
                            new EbnfExpressionSimple(
                                new EbnfTermSimple(
                                    new EbnfFactorOptional(
                                        new EbnfExpressionSimple(
                                            new EbnfTermSimple(
                                                new EbnfFactorLiteral("a")))))))));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseGrouping()
        {
            var actual =  Parse(@"Rule = ('a');");

            var expected = new EbnfDefinitionSimple(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifier("Rule"),
                            new EbnfExpressionSimple(
                                new EbnfTermSimple(
                                    new EbnfFactorGrouping(
                                        new EbnfExpressionSimple(
                                            new EbnfTermSimple(
                                                new EbnfFactorLiteral("a")))))))));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseNamespace()
        {
            var expected = new EbnfDefinitionSimple(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifier("This", "Is", "A", "Qualifier", "Rule"),
                            new EbnfExpressionSimple(
                                new EbnfTermSimple(
                                    new EbnfFactorLiteral("a"))))));

            var actual = Parse(@"This.Is.A.Qualifier.Rule = 'a'; ");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseMultipleRules()
        {
            var expected = new EbnfDefinitionConcatenation(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("S"),
                        new EbnfExpressionSimple(
                            new EbnfTermConcatenation(
                                new EbnfFactorIdentifier(
                                    new EbnfQualifiedIdentifier("A")),
                                new EbnfTermSimple(
                                    new EbnfFactorIdentifier(
                                        new EbnfQualifiedIdentifier("B"))))))),
                new EbnfDefinitionConcatenation(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifier("A"),
                            new EbnfExpressionSimple(
                                new EbnfTermSimple(
                                    new EbnfFactorLiteral("a"))))),
                    new EbnfDefinitionSimple(
                        new EbnfBlockRule(
                            new EbnfRule(
                                new EbnfQualifiedIdentifier(
                                    "B"),
                                new EbnfExpressionSimple(
                                    new EbnfTermSimple(
                                        new EbnfFactorLiteral("b"))))))));
            var actual = Parse(@"
                S = A B;
                A = 'a';
                B = 'b';
            ");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseSettings()
        {
            var actual = Parse(@"
                :ignore = whitespace; ");
            Assert.IsNotNull(actual);

            var expected = new EbnfDefinitionSimple(
                new EbnfBlockSetting(
                    new EbnfSetting(
                        new EbnfSettingIdentifier(":ignore"),
                        new EbnfQualifiedIdentifier("whitespace"))));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseLexerRule()
        {
            
            var actual = Parse(@"
                b ~ 'b' ;");
            Assert.IsNotNull(actual);

            var expected = new EbnfDefinitionSimple(
                new EbnfBlockLexerRule(
                   new EbnfLexerRule(
                       new EbnfQualifiedIdentifier("b"),
                       new EbnfLexerRuleExpressionSimple(
                            new EbnfLexerRuleTermSimple(
                                new EbnfLexerRuleFactorLiteral("b"))))));

            Assert.AreEqual(expected, actual);
        }
        
        [TestMethod]
        public void EbnfParserShouldParseComplexGrammarWithRepeat()
        {
            var stringBuilder = new StringBuilder()
            //.AppendLine("file = ws directives ws ;")
            .AppendLine("file = \"1\" { \"2\" } \"1\";");
            //.AppendLine("directives = directive { ows directive };")
            //.AppendLine("directive = \"0\" | \"1\"; ");

            var actual = Parse(stringBuilder.ToString());

            var grammar = new EbnfGrammar();
            var parseEngine = new ParseEngine(grammar, new ParseEngineOptions(false));
            var parseRunner = new ParseRunner(parseEngine, stringBuilder.ToString());
            while (!parseRunner.EndOfStream())
            {
                if (!parseRunner.Read())
                    throw new Exception(
                        $"Unable to parse Ebnf. Error at position {parseRunner.Position}.");
            }
            if (!parseEngine.IsAccepted())
                throw new Exception(
                    $"Unable to parse Ebnf. Error at position {parseRunner.Position}");

            var parseForest = parseEngine.GetParseForestRootNode();
            var visitor = new LoggingForestNodeVisitor(Console.Out);
            parseForest.Accept(visitor);
        }

        private static IEbnfDefinition Parse(string input)
        {
            var ebnfParser = new EbnfParser();
            return ebnfParser.Parse(input);
        }
    }
}
