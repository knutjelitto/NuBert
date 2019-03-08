using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Pliant.Tests.Integration
{
    [TestClass]
    public class JsonLexerTests
    {
        [TestMethod]
        public void JsonLexerShouldReadSingleDigit()
        {
            var input = "1";
            var jsonLexer = new JsonLexer();
            var tokens = jsonLexer.Lex(input).ToArray();
            Assert.AreEqual(1, tokens.Length);
            Assert.AreEqual(input, tokens[0].Value);
            Assert.AreEqual(0, tokens[0].Position);
        }

        [TestMethod]
        public void JsonLexerShouldReadString()
        {
            var input = "\"this is a string\"";
            var jsonLexer = new JsonLexer();
            var tokens = jsonLexer.Lex(input).ToArray();
            Assert.AreEqual(1, tokens.Length);
            Assert.AreEqual(input, tokens[0].Value);
        }

        [TestMethod]
        public void JsonLexerShouldReturnTrue()
        {
            var input = "true";
            var jsonLexer = new JsonLexer();
            var tokens = jsonLexer.Lex(input).ToArray();
            Assert.AreEqual(1, tokens.Length);
            Assert.AreEqual(input, tokens[0].Value);
        }

        [TestMethod]
        public void JsonLexerShouldReturnFalse()
        {
            var input = "false";
            var jsonLexer = new JsonLexer();
            var tokens = jsonLexer.Lex(input).ToArray();
            Assert.AreEqual(1, tokens.Length);
            Assert.AreEqual(input, tokens[0].Value);
        }

        [TestMethod]
        public void JsonLexerShouldReturnNull()
        {
            var input = "true";
            var jsonLexer = new JsonLexer();
            var tokens = jsonLexer.Lex(input).ToArray();
            Assert.AreEqual(1, tokens.Length);
            Assert.AreEqual(input, tokens[0].Value);
        }

        [TestMethod]
        public void JsonLexerShouldReturnArrayTokens()
        {
            var input = "[1,2,3]";
            var jsonLexer = new JsonLexer();
            var tokens = jsonLexer.Lex(input).ToArray();
            Assert.AreEqual(7, tokens.Length);
            for (var i = 0; i < input.Length; i++)
            {
                Assert.AreEqual(input[i], tokens[i].Value[0]);
            }

            Assert.AreEqual(JsonLexer.OpenBracket, tokens[0].TokenName);
            Assert.AreEqual(JsonLexer.Number, tokens[1].TokenName);
            Assert.AreEqual(JsonLexer.Comma, tokens[2].TokenName);
            Assert.AreEqual(JsonLexer.Number, tokens[3].TokenName);
            Assert.AreEqual(JsonLexer.Comma, tokens[4].TokenName);
            Assert.AreEqual(JsonLexer.Number, tokens[5].TokenName);
            Assert.AreEqual(JsonLexer.CloseBracket, tokens[6].TokenName);
        }

        [TestMethod]
        public void JsonLexerShouldReturnObjectTokens()
        {
            var input = "{\"name\":\"something\",\"id\":12345}";
            var jsonLexer = new JsonLexer();
            var tokens = jsonLexer.Lex(input).ToArray();
            Assert.AreEqual(9, tokens.Length);
            Assert.AreEqual(JsonLexer.OpenBrace, tokens[0].TokenName);
            Assert.AreEqual(JsonLexer.String, tokens[1].TokenName);
            Assert.AreEqual(JsonLexer.Colon, tokens[2].TokenName);
            Assert.AreEqual(JsonLexer.String, tokens[3].TokenName);
            Assert.AreEqual(JsonLexer.Comma, tokens[4].TokenName);
            Assert.AreEqual(JsonLexer.String, tokens[5].TokenName);
            Assert.AreEqual(JsonLexer.Colon, tokens[6].TokenName);
            Assert.AreEqual(JsonLexer.Number, tokens[7].TokenName);
            Assert.AreEqual(JsonLexer.CloseBrace, tokens[8].TokenName);
        }
    }
}
