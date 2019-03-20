using Lingu.Automata;
using Lingu.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Lingu.Automata
{
    [TestClass]
    public class ATests
    {
        [TestMethod]
        public void ThereShouldBeThreeDfaStates()
        {
            // a?[ab]
            var matcher = MakeMatcher();

            Assert.AreEqual(3, matcher.Dfa.StateCount);
        }


        [TestMethod]
        public void CheckLength1()
        {
            // a?[ab]
            var matcher = MakeMatcher();

            Assert.IsTrue(matcher.FullMatch("a"));
            Assert.IsTrue(matcher.FullMatch("b"));
        }

        [TestMethod]
        public void CheckLength2()
        {
            // a?[ab]
            var matcher = MakeMatcher();

            Assert.IsTrue(matcher.FullMatch("aa"));
            Assert.IsTrue(matcher.FullMatch("ab"));
        }

        [TestMethod]
        public void CheckMisMatches()
        {
        }

        private static DfaMatcher MakeMatcher()
        {
            // a?[ab]
            var first = new NfaState();
            var last = new NfaState();
            var end = new NfaState();

            var a1 = Terminal.From('a');
            var a2 = Terminal.From('a', 'b');

            first.Add(a1, last);
            first.Add(last);
            last.Add(a2, end);

            var nfa = new Nfa(first, end);

            return new DfaMatcher(nfa.ToDfa());
        }
    }
}
