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

            Assert.AreEqual("[97]", sut.ToString());
        }

        [TestMethod]
        public void SingleShouldMatch()
        {
            var sut = Terminal.From('a');

            Assert.IsTrue(sut.Match('a'));
        }

        [TestMethod]
        public void SingleShouldntMatch()
        {
            var sut = Terminal.From('a');

            Assert.IsFalse(sut.Match('b'));
        }
    }
}