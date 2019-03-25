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
            var sut = Terminal.From('a').Not();

            Assert.AreEqual("!([97])", sut.ToString());
        }

        [TestMethod]
        public void NotSingleShouldMatch()
        {
            var sut = Terminal.From('a').Not();

            Assert.IsFalse(sut.Match('a'));
        }

        [TestMethod]
        public void NotSingleShouldntMatch()
        {
            var sut = Terminal.From('a').Not();

            Assert.IsTrue(sut.Match('b'));
        }

        [TestMethod]
        public void NotRangeCreateString()
        {
            var sut = Terminal.From('u', 'w').Not();

            Assert.AreEqual("!([117-119])", sut.ToString());
        }

        [TestMethod]
        public void NotRangeShouldMatch()
        {
            var sut = Terminal.From('u', 'w').Not();

            Assert.IsFalse(sut.Match('u'));
            Assert.IsFalse(sut.Match('v'));
            Assert.IsFalse(sut.Match('w'));
        }

        [TestMethod]
        public void NotRangeShouldntMatch()
        {
            var sut = Terminal.From('u', 'w').Not();

            Assert.IsTrue(sut.Match('a'));
            Assert.IsTrue(sut.Match('z'));
        }
    }
}