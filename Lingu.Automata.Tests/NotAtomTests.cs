using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lingu.Automata.Tests
{
    [TestClass]
    public class NotAtomTests
    {
        [TestMethod]
        public void NotSingleCreateString()
        {
            var sut = Atom.From('a').Not();

            Assert.AreEqual("[0-96,98-65535]", sut.ToString());
        }

        [TestMethod]
        public void NotSingleShouldMatch()
        {
            var sut = Atom.From('a').Not();

            Assert.IsFalse(sut.Match('a'));
        }

        [TestMethod]
        public void NotSingleShouldntMatch()
        {
            var sut = Atom.From('a').Not();

            Assert.IsTrue(sut.Match('b'));
        }

        [TestMethod]
        public void NotRangeCreateString()
        {
            var sut = Atom.From('u', 'w').Not();

            Assert.AreEqual("[0-116,120-65535]", sut.ToString());
        }

        [TestMethod]
        public void NotRangeShouldMatch()
        {
            var sut = Atom.From('u', 'w').Not();

            Assert.IsFalse(sut.Match('u'));
            Assert.IsFalse(sut.Match('v'));
            Assert.IsFalse(sut.Match('w'));
        }

        [TestMethod]
        public void NotRangeShouldntMatch()
        {
            var sut = Atom.From('u', 'w').Not();

            Assert.IsTrue(sut.Match('a'));
            Assert.IsTrue(sut.Match('z'));
        }
    }
}