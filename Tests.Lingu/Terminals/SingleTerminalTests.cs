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
            var sut = Terminal.From('a');

            Assert.AreEqual("'a'", sut.ToString());
        }

        [TestMethod]
        public void SingleShouldMatch()
        {
            var sut = Terminal.From('a');

            Assert.IsTrue(sut.Match('a'));

            Assert.IsFalse(sut.NotMatch('a'));
        }

        [TestMethod]
        public void SingleShouldntMatch()
        {
            var sut = Terminal.From('a');

            Assert.IsFalse(sut.Match('b'));

            Assert.IsTrue(sut.NotMatch('b'));
        }
    }
}