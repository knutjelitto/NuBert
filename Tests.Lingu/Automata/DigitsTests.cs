using System;
using System.Collections.Generic;
using System.Text;
using Lingu.Automata;
using Lingu.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Lingu.Automata
{
    [TestClass]
    public class DigitsTests
    {
        [TestMethod]
        public void DigitsZero()
        {
            var matcher = MakeDigitsMatcher();

            Assert.IsTrue(matcher.FullMatch("0"));
            Assert.IsFalse(matcher.FullMatch("00"));
            Assert.IsFalse(matcher.FullMatch("01"));
        }

        [TestMethod]
        public void DigitsOther()
        {
            var matcher = MakeDigitsMatcher();

            Assert.IsTrue(matcher.FullMatch("1"));
            Assert.IsTrue(matcher.FullMatch("12"));
            Assert.IsTrue(matcher.FullMatch("123"));
        }

        [TestMethod]
        public void DigitsFail()
        {
            var matcher = MakeDigitsMatcher();

            Assert.IsFalse(matcher.FullMatch(" 0"));
            Assert.IsFalse(matcher.FullMatch("0 "));
            Assert.IsFalse(matcher.FullMatch(" 1"));
            Assert.IsFalse(matcher.FullMatch("1 "));
        }

        private static DfaMatcher MakeDigitsMatcher()
        {
            // [0]|[1-9][0-9]*
            var start = new NfaState();
            var more = new NfaState();
            var end = new NfaState();

            var nonZeroDigits = new RangeTerminal('1', '9');
            var allDigits = new RangeTerminal('0', '9');
            var zeroDigit = new RangeTerminal('0');

            start.Add(nonZeroDigits, more);
            start.Add(zeroDigit, end);
            more.Add(allDigits, more);
            more.Add(end);

            var nfa = new Nfa(start, end);

            return new DfaMatcher(nfa.ToDfa());
        }
    }
}
