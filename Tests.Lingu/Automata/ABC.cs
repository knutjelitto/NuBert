using Lingu.Automata;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Lingu.Automata
{
    [TestClass]
    public class ABCTests
    {
        [TestMethod]
        public void ThereShouldBeFourDfaStates()
        {
            // a?b?c?
            var matcher = MakeMatcher();

            Assert.AreEqual(4, matcher.Dfa.StateCount);
        }

        [TestMethod]
        public void CheckMatches()
        {
            // a?b?c?
            var matcher = MakeMatcher();

            Assert.IsTrue(matcher.FullMatch(""));
            Assert.IsTrue(matcher.FullMatch("a"));
            Assert.IsTrue(matcher.FullMatch("ab"));
            Assert.IsTrue(matcher.FullMatch("ac"));
            Assert.IsTrue(matcher.FullMatch("b"));
            Assert.IsTrue(matcher.FullMatch("bc"));
            Assert.IsTrue(matcher.FullMatch("c"));
        }

        [TestMethod]
        public void CheckMisMatches()
        {
            // a?b?c?
            var matcher = MakeMatcher();

            Assert.IsFalse(matcher.FullMatch("aa"));
            Assert.IsFalse(matcher.FullMatch("ba"));
            Assert.IsFalse(matcher.FullMatch("x"));
        }

        private static DfaMatcher MakeMatcher()
        {
            // a?b?c?
            var nfa = NfaBuilder
                    .From('a').Optional()
                    .Concat(NfaBuilder.From('b').Optional())
                    .Concat(NfaBuilder.From('c').Optional());

            return new DfaMatcher(nfa.ToDfa());
        }
    }
}
