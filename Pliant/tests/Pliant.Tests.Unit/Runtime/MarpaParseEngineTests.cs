using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.LexerRules;
using Pliant.RegularExpressions;
using Pliant.Runtime;
using Pliant.Tests.Common.Grammars;
using Pliant.Tokens;
using System.Linq;

namespace Pliant.Tests.Unit.Runtime
{
    [TestClass]
    public class MarpaParseEngineTests
    {
        [TestMethod]
        public void MarpaParseEngineShouldParseRightRecursiveGrammarInLinearTime()
        {
            var parseEngine = new MarpaParseEngine(new RightRecursionGrammar());
            var input = "aaaaaaaaa";
            var tokens = input.Select((a, i) => 
            {
                var value = a.ToString();
                return new VerbatimToken(i, value, new TokenClass(value));
            }).ToArray();

            for (var t = 0; t < tokens.Length; t++)
            {
                var token = tokens[t];
                var result = parseEngine.Pulse(token);
                if (!result)
                {
                    Assert.Fail($"Error parsing at token position {t}");
                }
            }
            var accepted = parseEngine.IsAccepted();
            if (!accepted)
            {
                Assert.Fail("Parse is not accepted. ");
            }

            var deterministicChart = parseEngine.Chart;
            var lastDeterministicSet = deterministicChart.Sets[deterministicChart.Sets.Count - 1];
            Assert.AreEqual(4, lastDeterministicSet.States.Count);
        }

        [TestMethod]
        public void MarpaParseEngineCanParseRegex()
        {
            var regexGrammar = new RegexGrammar();
            var preComputedRegexGrammar = new PreComputedGrammar(regexGrammar);
            var parseEngine = new MarpaParseEngine(preComputedRegexGrammar);

            var pattern = "[a-z][0-9]abc123";

            var openBracket = new TokenClass("[");
            var notMeta = new TokenClass("NotMeta"); // maybe make this token type a readonly property on the regex grammar?
            var notCloseBracket = new TokenClass("NotCloseBracket"); // maybe make this token type a readonly property on the regex grammar?            
            var closeBracket = new TokenClass("]");
            var dash = new TokenClass("-");

            for (var i = 0; i < pattern.Length; i++)
            {
                TokenClass tokenType;
                switch (pattern[i])
                {
                    case '[':
                        tokenType = openBracket;
                        break;

                    case ']':
                        tokenType = closeBracket;
                        break;

                    case '-':
                        tokenType = dash;
                        break;

                    default:
                        tokenType = i < 10 ? notCloseBracket : notMeta;

                        break;
                }
                var token = new VerbatimToken(i, pattern[i].ToString(), tokenType);
                var result = parseEngine.Pulse(token);
                Assert.IsTrue(result, $"Error at position {i}");
            }
            Assert.IsTrue(parseEngine.IsAccepted(), "Parse was not accepted");
        }

        [TestMethod]
        public void MarpaParseEngineShouldParseEncapsulatedRepeatingRightRecursiveRule()
        {
            var number = new NumberLexerRule();
            var openBracket = new TerminalLexer('[');
            var closeBracket = new TerminalLexer(']');
            var comma = new TerminalLexer(',');

            ProductionExpression
                A = "A",
                V = "V",
                VR = "VR";

            A.Rule = openBracket + VR + closeBracket;
            VR.Rule = V 
                | (V + comma + VR) 
                | Expr.Epsilon;
            V.Rule = number;

            var grammar = new GrammarExpression(
                A, new []{ A, V, VR }).ToGrammar();

            var marpaParseEngine = new MarpaParseEngine(grammar);

            var tokens = new [] 
            {
                new VerbatimToken(0, "[", openBracket.TokenType),
                new VerbatimToken(1, "1", number.TokenType),
                new VerbatimToken(2, ",", comma.TokenType),
                new VerbatimToken(3, "2", number.TokenType),
                new VerbatimToken(4, "]", closeBracket.TokenType)
            };

            foreach (var token in tokens)
            {
                var result = marpaParseEngine.Pulse(token);
                if (!result)
                {
                    Assert.Fail($"Failure parsing at position {marpaParseEngine.Location}");
                }
            }

            var accepted = marpaParseEngine.IsAccepted();
            if (!accepted)
            {
                Assert.Fail("Input was not accepted.");
            }
        }

        [TestMethod]
        public void MarpaParseEngineShouldNotMemoizeRuleWhenSiblingIsRightRecursiveAndCurrentRuleIsNot()
        {
            ProductionExpression
                S = "S",
                A = "A";

            S.Rule = A;
            A.Rule = ((Expr)'a' + 'a' + A)
                | ((Expr) 'b' + 'b');

            var grammar = new GrammarExpression(S, new[] { S, A }).ToGrammar();
            var marpaParseEngine = new MarpaParseEngine(grammar);

            var bTokenType = new TokenClass("b");
            var tokens = new[] 
            {
                new VerbatimToken(0, "b", bTokenType),
                new VerbatimToken(1, "b", bTokenType)
            };
            
            foreach (var token in tokens)
            {
                var result = marpaParseEngine.Pulse(token);
                if (!result)
                {
                    Assert.Fail($"Failure parsing at position {marpaParseEngine.Location}");
                }
            }

            var accepted = marpaParseEngine.IsAccepted();
            if (!accepted)
            {
                Assert.Fail("Input was not accepted.");
            }
        }     
    }
}
