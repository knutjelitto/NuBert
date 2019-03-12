using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.LexerRules;
using Pliant.Terminals;
using Pliant.Tokens;

namespace Pliant.Tests.Unit.Automata
{
    [TestClass]
    public class DfaLexemeTests
    {
        [TestMethod]
        public void DfaLexemeShouldMatchOneOrMoreRandomWhitespaceCharacters()
        {
            var randomWhitespace = "\t\f\v \r\n";
            var dfa = DfaState.Inner();
            var final = DfaState.Final();
            dfa.AddTransition(WhitespaceTerminal.Instance, final);
            final.AddTransition(WhitespaceTerminal.Instance, final);

            var dfaLexerRule = new DfaLexerRule(dfa, new TokenName("whitespace"));
            var whitespaceLexeme = dfaLexerRule.CreateLexeme(0);
            foreach (var character in randomWhitespace)
            {
                Assert.IsTrue(whitespaceLexeme.Scan(character));
            }
        }

        [TestMethod]
        public void DfaLexemeShouldMatchMixedCaseWord()
        {
            var wordInput = "t90vAriabl3";
            var dfa = DfaState.Inner();
            var final = DfaState.Final();
            dfa.AddTransition(new RangeTerminal('a', 'z'), final);
            dfa.AddTransition(new RangeTerminal('A', 'Z'), final);
            final.AddTransition(new RangeTerminal('a', 'z'), final);
            final.AddTransition(new RangeTerminal('A', 'Z'), final);
            final.AddTransition(DigitTerminal.Instance, final);

            var dfaLexerRule = new DfaLexerRule(dfa, new TokenName("Identifier"));
            var indentifierLexeme = dfaLexerRule.CreateLexeme(0);
            foreach (var character in wordInput)
            {
                Assert.IsTrue(indentifierLexeme.Scan(character));
            }
        }

        [TestMethod]
        public void DfaLexemeGivenCharacerLexemeNumberShouldFail()
        {
            var numberInput = "0";
            var dfa = DfaState.Inner();
            var final = DfaState.Final();
            dfa.AddTransition(new RangeTerminal('a', 'z'), final);
            final.AddTransition(new RangeTerminal('a', 'z'), final);

            var dfaLexerRule = new DfaLexerRule(dfa, new TokenName("lowerCase"));
            var letterLexeme = dfaLexerRule.CreateLexeme(0);
            Assert.IsFalse(letterLexeme.Scan(numberInput[0]));
            Assert.AreEqual(string.Empty, letterLexeme.Value);
        }

        [TestMethod]
        public void DfaLexemeResetShouldResetLexemeValues()
        {
            var numberLexerRule = new NumberLexerRule();

            var lexeme = numberLexerRule.CreateLexeme(0);
            const string numberInput = "0123456";
            foreach (var character in numberInput)
            {
                var result = lexeme.Scan(character);
                if (!result)
                {
                    Assert.Fail($"Did not recognize number {character}");
                }
            }
        }
    }
}