using Lingu.Automata;
using Lingu.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Lingu.Automata
{
    [TestClass]
    public class DigitsTests
    {
        [TestMethod]
        public void ThereShouldBeThreeDfaStates()
        {
            // [0]|[1-9][0-9]*
            var matcher = MakeMatcher();

            Assert.AreEqual(3, new DfaPlumber(matcher.Dfa).StateCount);
        }

        [TestMethod]
        public void DigitsZero()
        {
            // [0]|[1-9][0-9]*
            var matcher = MakeMatcher();

            Assert.IsTrue(matcher.FullMatch("0"));
            Assert.IsFalse(matcher.FullMatch("00"));
            Assert.IsFalse(matcher.FullMatch("01"));
        }

        [TestMethod]
        public void DigitsOther()
        {
            // [0]|[1-9][0-9]*
            var matcher = MakeMatcher();

            Assert.IsTrue(matcher.FullMatch("1"));
            Assert.IsTrue(matcher.FullMatch("12"));
            Assert.IsTrue(matcher.FullMatch("123"));
        }

        [TestMethod]
        public void DigitsFail()
        {
            // [0]|[1-9][0-9]*
            var matcher = MakeMatcher();

            Assert.IsFalse(matcher.FullMatch(" 0"));
            Assert.IsFalse(matcher.FullMatch("0 "));
            Assert.IsFalse(matcher.FullMatch(" 1"));
            Assert.IsFalse(matcher.FullMatch("1 "));
        }

        private static DfaMatcher MakeMatcher()
        {
            // [0]|[1-9][0-9]*
            var zero = NfaBuilder.From('0');
            var nonZero = NfaBuilder.From('1', '9');
            var all = NfaBuilder.From('0', '9');

            var nfa = zero.Or(nonZero.Concat(all.Star()));
            var dfa = nfa.ToDfa().Minimize();
            return new DfaMatcher(dfa);
        }
    }
}
