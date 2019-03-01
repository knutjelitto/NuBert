using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.RegularExpressions;

namespace Pliant.Tests.Unit.RegularExpressions
{
    /// <summary>
    /// Summary description for RegexParserTests
    /// </summary>
    [TestClass]
    public class RegexParserTests
    {
        public TestContext TestContext { get; set; }
        
        [TestMethod]
        public void RegexParserShouldParseSingleCharacter()
        {
            var regexParser = new RegexParser();
            var actual = regexParser.Parse("a");
            var expected = new Regex(
                false,
                new RegexExpressionTerm(
                    new RegexTermFactor(
                        new RegexFactorAtom(
                            new RegexAtomCharacter(
                                new RegexCharacter(
                                'a'))))),
                false);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RegexParserShouldParsePositiveSet()
        {
            var regexParser = new RegexParser();
            var actual = regexParser.Parse("[a]");
            var expected = new Regex(
                false,
                new RegexExpressionTerm(
                    new RegexTermFactor(
                        new RegexFactorAtom(
                            new RegexAtomSet(
                                new RegexSet(new RegexCharacterClass(
                                                 new RegexCharactersUnit(
                                                     new RegexCharacterClassCharacter('a'))),
                                             false))))),
                false);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RegexParserShouldParseNegativeSet()
        {
            var regexParser = new RegexParser();
            var actual = regexParser.Parse("[^a]");

            var expected = new Regex(
                false,
                new RegexExpressionTerm(
                    new RegexTermFactor(
                        new RegexFactorAtom(
                            new RegexAtomSet(
                                new RegexSet(new RegexCharacterClass(
                                                 new RegexCharactersUnit(
                                                     new RegexCharacterClassCharacter('a'))),
                                             true))))),
                false);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RegexParserShouldParseMultipleRanges()
        {
            var regexParser = new RegexParser();
            var actual = regexParser.Parse("[a-zA-Z0-9]");
            var expected = new Regex(
                false,
                endsWith: false,
                expression: new RegexExpressionTerm(
                    new RegexTermFactor(
                        new RegexFactorAtom(
                            new RegexAtomSet(
                                new RegexSet(new RegexCharacterClassAlteration(
                                                 characterClass: new RegexCharacterClassAlteration(
                                                     characterClass: new RegexCharacterClass(
                                                         new RegexCharactersRange(
                                                             new RegexCharacterClassCharacter('0'),
                                                             new RegexCharacterClassCharacter('9'))),
                                                     characterRange: new RegexCharactersRange(
                                                         new RegexCharacterClassCharacter('A'),
                                                         new RegexCharacterClassCharacter('Z'))),
                                                 characterRange: new RegexCharactersRange(
                                                     new RegexCharacterClassCharacter('a'),
                                                     new RegexCharacterClassCharacter('z'))),
                                             false))))));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RegexParserShouldParseAlteration()
        {
            var regexParser = new RegexParser();
            var actual = regexParser.Parse("a|b");
            var expected = new Regex(
                false,
                endsWith: false,
                expression: new RegexExpressionAlteration(
                    new RegexTermFactor(
                        new RegexFactorAtom(
                            new RegexAtomCharacter(
                                new RegexCharacter('a')))),
                    new RegexExpressionTerm(
                        new RegexTermFactor(
                            new RegexFactorAtom(
                                new RegexAtomCharacter(
                                    new RegexCharacter('b')))))));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RegexParserShouldParseEscape()
        {
            var regexParser = new RegexParser();
            var actual = regexParser.Parse(@"\.");

            var expected = new Regex(
                false, 
                new RegexExpressionTerm(
                    new RegexTermFactor(
                        new RegexFactorAtom(
                            new RegexAtomCharacter(
                                new RegexCharacter('.', true))))), 
                false);
            Assert.AreEqual(expected, actual);
        }
    }
}