using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using Pliant.Terminals;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class DigitTerminalTests
    {
        [TestMethod]
        public void DigitTerminalGivenNumberShouldMatch()
        {
            Assert.IsTrue(DigitTerminal.Instance.CanApply('0'));
        }

        [TestMethod]
        public void DigitTerminalGivenLetterShouldFailMatch()
        {
            Assert.IsFalse(DigitTerminal.Instance.CanApply('a'));
        }

        [TestMethod]
        public void DigitTerminalGetIntervalsShouldReturnSingleIntervalWithZeroToNineRange()
        {
            var intervals = DigitTerminal.Instance.GetIntervals();
            Assert.AreEqual(1, intervals.Count);
            Assert.AreEqual('0', intervals[0].Min);
            Assert.AreEqual('9', intervals[0].Max);
        }
    }
}