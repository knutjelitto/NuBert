using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lingu.Automata.Tests
{
    [TestClass]
    public class ATests
    {
        [TestMethod]
        public void ThereShouldBeThreeDfaStates()
        {
            // a?[ab]
            var matcher = MakeMatcher();

            Assert.AreEqual(3, new DfaPlumber(matcher.Dfa).StateCount);
        }


        [TestMethod]
        public void CheckLength1()
        {
            // a?[ab]
            var matcher = MakeMatcher();

            Assert.IsTrue(matcher.FullMatch("a"), "should match 'a'");
            Assert.IsTrue(matcher.FullMatch("b"), "should match 'b'");
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
            // a?[ab]
            var matcher = MakeMatcher();

            Assert.IsFalse(matcher.FullMatch(""));
            Assert.IsFalse(matcher.FullMatch("aaa"));
            Assert.IsFalse(matcher.FullMatch("abc"));
        }

        private static DfaMatcher MakeMatcher()
        {
            // a?[ab]
            var first = new NfaState();
            var last = new NfaState();
            var end = new NfaState();

            var a1 = Atom.From('a');
            var a2 = Atom.From('a', 'b');

            first.Add(a1, last);
            first.Add(last);
            last.Add(a2, end);

            var nfa = new Nfa(first, end);

            return new DfaMatcher(nfa.ToDfa().Minimize());
        }
    }
}
