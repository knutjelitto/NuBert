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
            var sut = new RangeTerminal('u', 'w');

            Assert.AreEqual("'u'-'w'", sut.ToString());
        }

        [TestMethod]
        public void RangeShouldMatch()
        {
            var sut = new RangeTerminal('u', 'w');

            Assert.IsTrue(sut.Match('u'));
            Assert.IsTrue(sut.Match('v'));
            Assert.IsTrue(sut.Match('w'));

            Assert.IsFalse(sut.NotMatch('u'));
            Assert.IsFalse(sut.NotMatch('v'));
            Assert.IsFalse(sut.NotMatch('w'));
        }

        [TestMethod]
        public void RangeShouldntMatch()
        {
            var sut = new RangeTerminal('u', 'w');

            Assert.IsFalse(sut.Match('a'));
            Assert.IsFalse(sut.Match('z'));

            Assert.IsTrue(sut.NotMatch('a'));
            Assert.IsTrue(sut.NotMatch('z'));
        }
    }
}