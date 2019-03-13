using Lingu.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Lingu.Terminals
{
    [TestClass]
    public class SingleTerminalTests
    {
        [TestMethod]
        public void SingleCreateString()
        {
            var sut = new SingleTerminal('a');

            Assert.AreEqual("'a'", sut.ToString());
        }

        [TestMethod]
        public void SingleShouldMatch()
        {
            var sut = new SingleTerminal('a');

            Assert.IsTrue(sut.Match('a'));

            Assert.IsFalse(sut.NotMatch('a'));
        }

        [TestMethod]
        public void SingleShouldntMatch()
        {
            var sut = new SingleTerminal('a');

            Assert.IsFalse(sut.Match('b'));

            Assert.IsTrue(sut.NotMatch('b'));
        }
    }
}