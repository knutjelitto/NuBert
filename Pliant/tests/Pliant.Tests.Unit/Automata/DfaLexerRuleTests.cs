using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.Grammars;
using Pliant.Terminals;
using Pliant.Tokens;

namespace Pliant.Tests.Unit.Automata
{
    [TestClass]
    public class DfaLexerRuleTests
    {
        [TestMethod]
        public void DfaLexerRuleShouldApplyToCharacterIfFirstStateHasTransition()
        {
            var states = new DfaState[2]
            {
                DfaState.Inner(),
                DfaState.Final()
            };

            states[0].AddTransition(WhitespaceTerminal.Instance, states[1]);
            states[1].AddTransition(WhitespaceTerminal.Instance, states[1]);

            var dfaLexerRule = new DfaLexerRule(states[0], new TokenName(@"\s+"));

            Assert.IsTrue(dfaLexerRule.CanApply(' '));
            Assert.IsTrue(dfaLexerRule.CanApply('\t'));
            Assert.IsTrue(dfaLexerRule.CanApply('\r'));
            Assert.IsFalse(dfaLexerRule.CanApply('a'));
        }
    }
}
