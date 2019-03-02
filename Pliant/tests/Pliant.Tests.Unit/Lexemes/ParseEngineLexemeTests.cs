using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.Runtime;
using Pliant.Tokens;
using System.Collections.Generic;
using System.Linq;
using Pliant.LexerRules;
using Pliant.Terminals;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class ParseEngineLexemeTests
    {
        [TestMethod]
        public void ParseEngineLexemeShouldConsumeWhitespace()
        {
            ProductionExpression
                // ReSharper disable once InconsistentNaming
                S = "S",
                // ReSharper disable once InconsistentNaming
                W = "W";

            S.Rule = W | (W + S);
            W.Rule = WhitespaceTerminal.Instance;

            var grammar = new GrammarExpression(S, new[] { S, W }).ToGrammar();

            var lexerRule = new GrammarLexer("whitespace", grammar);

            var lexeme = new ParseEngineLexeme(lexerRule);
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
            ProductionExpression sequence = "sequence";
            sequence.Rule = (Expr)'a' + 'b' + 'c' + '1' + '2' + '3';

            var grammar = new GrammarExpression(sequence, new[] { sequence }).ToGrammar();
            var lexerRule = new GrammarLexer(new TokenType("sequence"), grammar);
            var lexeme = new ParseEngineLexeme(lexerRule);
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
            var lexemeList = new List<ParseEngineLexeme>();

            ProductionExpression
                // ReSharper disable once InconsistentNaming
                There = "there";
            There.Rule = (Expr)'t' + 'h' + 'e' + 'r' + 'e';
            var thereGrammar = new GrammarExpression(There, new[] { There })
                .ToGrammar();
            var thereLexerRule = new GrammarLexer(new TokenType(There.ProductionModel.LeftHandSide.NonTerminal.Value), thereGrammar);
            var thereLexeme = new ParseEngineLexeme(thereLexerRule);
            lexemeList.Add(thereLexeme);

            ProductionExpression
                // ReSharper disable once InconsistentNaming
                Therefore = "therefore";
            Therefore.Rule = (Expr)'t' + 'h' + 'e' + 'r' + 'e' + 'f' + 'o' + 'r' + 'e';
            var thereforeGrammar = new GrammarExpression(Therefore, new[] { Therefore })
                .ToGrammar();
            var thereforeLexerRule = new GrammarLexer(new TokenType(Therefore.ProductionModel.LeftHandSide.NonTerminal.Value), thereforeGrammar);
            var thereforeLexeme = new ParseEngineLexeme(thereforeLexerRule);
            lexemeList.Add(thereforeLexeme);

            var input = "therefore";
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