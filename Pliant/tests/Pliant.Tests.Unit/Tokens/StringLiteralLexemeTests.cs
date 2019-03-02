﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Tokens;
using Pliant.Grammars;
using Pliant.LexerRules;

namespace Pliant.Tests.Unit.Tokens
{
    [TestClass]
    public class StringLiteralLexemeTests
    {
        [TestMethod]
        public void StringLiteralLexemeResetShouldResetLexemeValues()
        {
            var abc123LexerRule = new StringLiteralLexer("abc123");
            
            var lexeme = new StringLiteralLexeme(abc123LexerRule, 0);
            const string input = "abc123";
            foreach (var character in input)
            {
                var result = lexeme.Scan(character);
                if (!result)
                {
                    Assert.Fail($"Did not recognize character {character}");
                }
            }
        }
    }
}
