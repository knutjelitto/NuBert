using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.Grammars;
using Pliant.Json;
using Pliant.RegularExpressions;
using Pliant.Runtime;
using Pliant.Tests.Common;

namespace Pliant.Tests.Integration.Runtime
{
    [TestClass]
    public class LargeFileParseTests
    {
        public TestContext TestContext { get; set; }

        [ClassInitialize]
#pragma warning disable CC0057 // Unused parameters
#pragma warning disable RECS0154 // Parameter is never used
        public static void Initialize(TestContext testContext)
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning restore CC0057 // Unused parameters
        {
            grammar = new JsonGrammar();
        }

        [TestInitialize]
        public void InitializeTest()
        {
            this.parseTester = new ParseTester(grammar);
            var preComputedGrammar = new PreComputedGrammar(grammar);
            this.compressedParseTester = new ParseTester(
                new DeterministicParseEngine(preComputedGrammar));
            this.marpaParseTester = new ParseTester(
                new MarpaParseEngine(preComputedGrammar));
        }

        [TestMethod]
        public void TestCanParseJsonArray()
        {
            var json = @"[""one"", ""two""]";
            this.parseTester.RunParse(json);
        }

        [TestMethod]
        public void TestCanParseJsonArrayWithCompression()
        {
            var json = @"[""one"", ""two""]";
            this.compressedParseTester.RunParse(json);
        }

        [TestMethod]
        public void TestCanParseJsonObject()
        {
            var json = @"
            {
                ""firstName"":""Patrick"", 
                ""lastName"": ""Huber"",
                ""id"": 12345
            }";
            this.parseTester.RunParse(json);
        }

        [TestMethod]
        public void TestCanParseJsonObjectWithCompression()
        {
            var json = @"
            {
                ""firstName"":""Patrick"", 
                ""lastName"": ""Huber"",
                ""id"": 12345
            }";
            this.compressedParseTester.RunParse(json);
        }

        [TestMethod]
        [DeploymentItem(@"10000.json")]
        public void TestCanParseLargeJsonFile()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "10000.json");
            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream))
            {
                this.parseTester.RunParse(reader);
            }
        }

        [TestMethod]
        [DeploymentItem(@"10000.json")]
        public void TestCanParseLargeJsonFileWithCompression()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "10000.json");
            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream))
            {
                this.compressedParseTester.RunParse(reader);
            }
        }

        [TestMethod]
        [DeploymentItem(@"10000.json")]
        public void TestCanParseLargeJsonFileWithCustomLexer()
        {
            var parser = this.parseTester.ParseEngine;
            RunParseWithCustomLexer(parser);
        }

        [TestMethod]
        public void TestCanParseLargeJsonFileWithCustomLexerAndCompression()
        {
            var parser = this.compressedParseTester.ParseEngine;
            RunParseWithCustomLexer(parser);
        }

        [TestMethod]
        public void TestCanParseLargeJsonFileWithCustomLexerAndMarpa()
        {
            var parser = this.marpaParseTester.ParseEngine;
            RunParseWithCustomLexer(parser);
        }

        [TestMethod]
        [DeploymentItem(@"10000.json")]
        public void TestCanParseLargeJsonFileWithMarpa()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "10000.json");
            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream))
            {
                this.marpaParseTester.RunParse(reader);
            }
        }

        private static LexerRule CreateRegexDfa(string pattern)
        {
            var regexParser = new RegexParser();
            var regex = regexParser.Parse(pattern);
            var regexCompiler = new RegexCompiler();
            var dfa = regexCompiler.Compile(regex);
            return new DfaLexerRule(dfa, pattern);
        }

        private static void RunParseWithCustomLexer(IParseEngine parser)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "10000.json");
            var jsonLexer = new JsonLexer();
#if true
            var tokens = jsonLexer.Lex(File.ReadAllText(path));
            foreach (var token in tokens)
            {
                if (!Equals(token.TokenClass, JsonLexer.Whitespace))
                {
                    if (!parser.Pulse(token))
                    {
                        Assert.Fail($"unable to parse token {token.TokenClass} at {token.Position}");
                    }
                }
            }
#else
            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream))
            {
                var tokens = jsonLexer.Lex(reader);
                foreach (var token in tokens)
                {
                    if (!Equals(token.TokenType, JsonLexer.Whitespace))
                    {
                        if (!parser.Pulse(token))
                        {
                            Assert.Fail($"unable to parse token {token.TokenType} at {token.Position}");
                        }
                    }
                }
            }
#endif

            if (!parser.IsAccepted())
            {
                Assert.Fail("Parse was not accepted");
            }
        }

        private static Grammar grammar;

        private ParseTester compressedParseTester;
        private ParseTester marpaParseTester;

        private ParseTester parseTester;
    }
}