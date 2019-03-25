using Lingu.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Lingu.Terminals
{
    [TestClass]
    public class RangeTerminalTests
    {
        [TestMethod]
        public void RangeCreateString()
        {
            var sut = Terminal.From('u', 'w');

            Assert.AreEqual("[117-119]", sut.ToString());
        }

        [TestMethod]
        public void RangeShouldMatch()
        {
            var sut = Terminal.From('u', 'w');

            Assert.IsTrue(sut.Match('u'));
            Assert.IsTrue(sut.Match('v'));
            Assert.IsTrue(sut.Match('w'));
        }

        [TestMethod]
        public void RangeShouldntMatch()
        {
            var sut = Terminal.From('u', 'w');

            Assert.IsFalse(sut.Match('a'));
            Assert.IsFalse(sut.Match('z'));
        }
    }
}