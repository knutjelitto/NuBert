using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.LexerRules;
using Pliant.Runtime;
using Pliant.Terminals;
using Pliant.Tokens;

// ReSharper disable InconsistentNaming

namespace Pliant.Tests.Unit.Lexemes
{
    [TestClass]
    public class ParseEngineLexemeTests
    {
        [TestMethod]
        public void ParseEngineLexemeShouldConsumeWhitespace()
        {
            ProductionExpression
                S = "S",
                W = "W";

            S.Rule = W | (W + S);
            W.Rule = WhitespaceTerminal.Instance;

            var grammar = new GrammarExpression(S, new[] { S, W }).ToGrammar();

            var lexerRule = new GrammarLexerRule("whitespace", grammar);
            var lexeme = lexerRule.CreateLexeme(0);

            var input = "\t\r\n\v\f ";
            for (var i = 0; i < input.Length; i++)
            {
                Assert.IsTrue(lexeme.Scan(input[i]), $"Unable to recognize input[{i}]");
            }

            Assert.IsTrue(lexeme.IsAccepted());
        }

        [TestMethod]
        public void ParseEngineLexemeShouldConsumeCharacterSequence()
        {
            ProductionExpression
                sequence = "sequence";

            sequence.Rule = (Expr)'a' + 'b' + 'c' + '1' + '2' + '3';

            var grammar = new GrammarExpression(sequence, new[] { sequence }).ToGrammar();
            var lexerRule = new GrammarLexerRule(new TokenClass("sequence"), grammar);
            var lexeme = lexerRule.CreateLexeme(0);
            var input = "abc123";
            foreach (var ch in input)
            {
                Assert.IsTrue(lexeme.Scan(ch));
            }

            Assert.IsTrue(lexeme.IsAccepted());
        }

        [TestMethod]
        public void ParseEngineLexemeShouldMatchLongestAcceptableTokenWhenGivenAmbiguity()
        {
            var lexemeList = new List<Lexeme>();

            ProductionExpression
                There = "there";

            There.Rule = (Expr)'t' + 'h' + 'e' + 'r' + 'e';

            var thereGrammar = new GrammarExpression(There, new[] { There }).ToGrammar();
            var thereLexerRule = new GrammarLexerRule(nameof(There), thereGrammar);
            var thereLexeme = thereLexerRule.CreateLexeme(0);
            lexemeList.Add(thereLexeme);

            ProductionExpression
                Therefore = "therefore";

            Therefore.Rule = (Expr)'t' + 'h' + 'e' + 'r' + 'e' + 'f' + 'o' + 'r' + 'e';

            var thereforeGrammar = new GrammarExpression(Therefore, new[] { Therefore }).ToGrammar();
            var thereforeLexerRule = new GrammarLexerRule(nameof(Therefore), thereforeGrammar);
            var thereforeLexeme = thereforeLexerRule.CreateLexeme(0);
            lexemeList.Add(thereforeLexeme);

            const string input = "therefore";
            var i = 0;
            for (; i < input.Length; i++)
            {
                var passedLexemes = lexemeList
                    .Where(l => l.Scan(input[i]))
                    .ToList();

                // all existing lexemes have failed
                // fall back onto the lexemes that existed before
                // we read this character
                if (passedLexemes.Count == 0)
                {
                    break;
                }

                lexemeList = passedLexemes;
            }

            Assert.AreEqual(i, input.Length);
            Assert.AreEqual(1, lexemeList.Count);
            var remainingLexeme = lexemeList[0];
            Assert.IsNotNull(remainingLexeme);
            Assert.IsTrue(remainingLexeme.IsAccepted());
        }
    }
}