using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using Pliant.Json;
using Pliant.Runtime;
using Pliant.Tests.Common;

namespace Pliant.Tests.Integration.Runtime
{
    [TestClass]
    public class LargeFileParseTests
    {
        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            grammar = new JsonGrammar();
        }

        [TestInitialize]
        public void InitializeTest()
        {
            this.parseTester = new ParseTester(grammar);
        }

        [TestMethod]
        public void TestCanParseJsonArray()
        {
            var json = @"[""one"", ""two""]";
            this.parseTester.RunParse(json);
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
        public void TestCanParseLargeJsonFileWithCustomLexer()
        {
            var parser = this.parseTester.ParseEngine;
            RunParseWithCustomLexer(parser);
        }

        private static void RunParseWithCustomLexer(IParseEngine parser)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "10000.json");
            var jsonLexer = new JsonLexer();
            var tokens = jsonLexer.Lex(File.ReadAllText(path));
            foreach (var token in tokens)
            {
                if (!Equals(token.TokenName, JsonLexer.Whitespace))
                {
                    if (!parser.Pulse(token))
                    {
                        Assert.Fail($"unable to parse token {token.TokenName} at {token.Position}");
                    }
                }
            }

            if (!parser.IsAccepted())
            {
                Assert.Fail("Parse was not accepted");
            }
        }

        private static Grammar grammar;
        private ParseTester parseTester;
    }
}