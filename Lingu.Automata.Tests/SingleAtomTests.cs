using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lingu.Automata.Tests
{
    [TestClass]
    public class SingleAtomTests
    {
        [TestMethod]
        public void SingleCreateString()
        {
            var sut = Atom.From('a');

            Assert.AreEqual("[97]", sut.ToString());
        }

        [TestMethod]
        public void SingleShouldMatch()
        {
            var sut = Atom.From('a');

            Assert.IsTrue(sut.Match('a'));
        }

        [TestMethod]
        public void SingleShouldntMatch()
        {
            var sut = Atom.From('a');

            Assert.IsFalse(sut.Match('b'));
        }
    }
}