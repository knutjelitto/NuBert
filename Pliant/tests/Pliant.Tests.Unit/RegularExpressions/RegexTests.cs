using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using Pliant.RegularExpressions;
using Pliant.Runtime;

namespace Pliant.Tests.Unit.RegularExpressions
{
    /// <summary>
    /// Summary description for RegexTests
    /// </summary>
    [TestClass]
    public class RegexTests
    {
        private IParseEngine parseEngine;
        private readonly Grammar regexGrammar;

        [TestInitialize]
        public void Initialize_Regex_Tests()
        {
            this.parseEngine = new ParseEngine(this.regexGrammar);
        }

        public RegexTests()
        {
            this.regexGrammar = new RegexGrammar();
        }

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void RegexShouldParseSingleCharacter()
        {
            var input = "a";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldParseStringLiteral()
        {
            var input = "abc";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldParseOptionalCharacter()
        {
            var input = "a?";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldParseWhitespaceCharacterClass()
        {
            var input = @"[\s]+";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldParseRangeCharacterClass()
        {
            var input = @"[a-z]";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldParseAnyCharacterClass()
        {
            var input = @".*";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void RegexFailOnMisMatchedParenthesis()
        {
            var input = "(a";
            ParseAndNotAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldFailOnMisMatchedBrackets()
        {
            var input = "[abc";
            ParseAndNotAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldFailEmptyGroup()
        {
            var input = "()";
            FailParseAtPosition(input, 1);
        }

        [TestMethod]
        public void RegexShouldFailEmptyInput()
        {
            var input = "";
            ParseAndNotAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldParseNegativeCharacterClass()
        {
            var input = "[^abc]";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldParseOpenBracketInCharacterClass()
        {
            var input = "[\\]]";
            ParseAndAcceptInput(input);
        }
        
        private void ParseAndAcceptInput(string input)
        {
            ParseInput(input);
            Accept();
        }

        private void ParseAndNotAcceptInput(string input)
        {
            ParseInput(input);
            NotAccept();
        }

        private void FailParseAtPosition(string input, int position)
        {
            var parseRunner = new ParseRunner(this.parseEngine, input);
            for (int i = 0; i < input.Length; i++)
                if (i < position)
                    Assert.IsTrue(parseRunner.Read(),
                        $"Line 0, Column {this.parseEngine.Location} : Invalid Character {input[i]}");
                else
                    Assert.IsFalse(parseRunner.Read());
        }

        private void ParseInput(string input)
        {
            var parseRunner = new ParseRunner(this.parseEngine, input);
            for (int i = 0; i < input.Length; i++)
                if(!parseRunner.Read())
                    Assert.Fail($"Line 0, Column {this.parseEngine.Location} : Invalid Character {input[i]}");
        }

        private void Accept()
        {
            Assert.IsTrue(this.parseEngine.IsAccepted(), "input was not recognized");
        }

        private void NotAccept()
        {
            Assert.IsFalse(this.parseEngine.IsAccepted(), "input was recognized");
        }
    }
}