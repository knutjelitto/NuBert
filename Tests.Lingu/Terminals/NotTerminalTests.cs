using Lingu.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Lingu.Terminals
{
    [TestClass]
    public class NotTerminalTests
    {
        [TestMethod]
        public void NotSingleCreateString()
        {
            var sut = new RangeTerminal('a').Not();

            Assert.AreEqual("!('a')", sut.ToString());
        }

        [TestMethod]
        public void NotSingleShouldMatch()
        {
            var sut = new RangeTerminal('a').Not();

            Assert.IsFalse(sut.Match('a'));

            Assert.IsTrue(sut.NotMatch('a'));
        }

        [TestMethod]
        public void NotSingleShouldntMatch()
        {
            var sut = new RangeTerminal('a').Not();

            Assert.IsTrue(sut.Match('b'));

            Assert.IsFalse(sut.NotMatch('b'));
        }

        [TestMethod]
        public void NotRangeCreateString()
        {
            var sut = new RangeTerminal('u', 'w').Not();

            Assert.AreEqual("!('u'-'w')", sut.ToString());
        }

        [TestMethod]
        public void NotRangeShouldMatch()
        {
            var sut = new RangeTerminal('u', 'w').Not();

            Assert.IsFalse(sut.Match('u'));
            Assert.IsFalse(sut.Match('v'));
            Assert.IsFalse(sut.Match('w'));

            Assert.IsTrue(sut.NotMatch('u'));
            Assert.IsTrue(sut.NotMatch('v'));
            Assert.IsTrue(sut.NotMatch('w'));
        }

        [TestMethod]
        public void NotRangeShouldntMatch()
        {
            var sut = new RangeTerminal('u', 'w').Not();

            Assert.IsTrue(sut.Match('a'));
            Assert.IsTrue(sut.Match('z'));

            Assert.IsFalse(sut.NotMatch('a'));
            Assert.IsFalse(sut.NotMatch('z'));
        }
    }
}