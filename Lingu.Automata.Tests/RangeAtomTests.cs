using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lingu.Automata.Tests
{
    [TestClass]
    public class RangeAtomTests
    {
        [TestMethod]
        public void RangeCreateString()
        {
            var sut = Atom.From('u', 'w');

            Assert.AreEqual("[117-119]", sut.ToString());
        }

        [TestMethod]
        public void RangeShouldMatch()
        {
            var sut = Atom.From('u', 'w');

            Assert.IsTrue(sut.Match('u'));
            Assert.IsTrue(sut.Match('v'));
            Assert.IsTrue(sut.Match('w'));
        }

        [TestMethod]
        public void RangeShouldntMatch()
        {
            var sut = Atom.From('u', 'w');

            Assert.IsFalse(sut.Match('a'));
            Assert.IsFalse(sut.Match('z'));
        }
    }
}