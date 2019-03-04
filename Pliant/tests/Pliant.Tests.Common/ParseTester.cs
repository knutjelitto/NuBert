using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.Runtime;
using Pliant.Tokens;

namespace Pliant.Tests.Common
{
    public class ParseTester
    {
        public ParseTester(GrammarExpression expression)
            : this(expression.ToGrammar())
        {
        }

        public ParseTester(Grammar grammar)
        {
            Grammar = grammar;
            ParseEngine = new ParseEngine(Grammar);
        }

        public ParseTester(IParseEngine parseEngine)
        {
            Grammar = parseEngine.Grammar;
            ParseEngine = parseEngine;
        }

        private Grammar Grammar { get; }
        public IParseEngine ParseEngine { get; }
        private IParseRunner ParseRunner { get; set; }

        public void RunParse(string input)
        {
            ParseRunner = new ParseRunner(ParseEngine, input);
            InternalRunParse(ParseRunner);
        }

        public void RunParse(TextReader reader)
        {
            ParseRunner = new ParseRunner(ParseEngine, reader);
            InternalRunParse(ParseRunner);
        }

        public void RunParse(IReadOnlyList<IToken> tokens)
        {
            foreach (var token in tokens)
            {
                if (!ParseEngine.Pulse(token))
                {
                    Assert.Fail($"Parse Failed at Position {ParseEngine.Location}");
                }
            }

            if (!ParseEngine.IsAccepted())
            {
                Assert.Fail("Parse was not accepted");
            }
        }

        private static void InternalRunParse(IParseRunner parseRunner)
        {
            while (!parseRunner.EndOfStream())
            {
                if (!parseRunner.Read())
                {
                    Assert.Fail(
                        $"Parse Failed at Line: {parseRunner.Line}, Column: {parseRunner.Column}, Position: {parseRunner.Position}");
                }
            }

            if (!parseRunner.ParseEngine.IsAccepted())
            {
                Assert.Fail("Parse was not accepted");
            }
        }
    }
}