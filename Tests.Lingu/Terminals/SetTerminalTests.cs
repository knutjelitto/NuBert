using Lingu.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Lingu.Terminals
{
    [TestClass]
    public class SetTerminalTests
    {
        [TestMethod]
        public void SetCreateString()
        {
            var sut = new SetTerminal(new SingleTerminal('a'), new RangeTerminal('u', 'w'));

            Assert.AreEqual("'a'|'u'-'w'", sut.ToString());
        }

        [TestMethod]
        public void SetShouldMatch()
        {
            var sut = new SetTerminal(new SingleTerminal('a'), new RangeTerminal('u', 'w'));

            Assert.IsTrue(sut.Match('a'));
            Assert.IsTrue(sut.Match('u'));
            Assert.IsTrue(sut.Match('v'));
            Assert.IsTrue(sut.Match('w'));

            Assert.IsFalse(sut.NotMatch('a'));
            Assert.IsFalse(sut.NotMatch('u'));
            Assert.IsFalse(sut.NotMatch('v'));
            Assert.IsFalse(sut.NotMatch('w'));
        }

        [TestMethod]
        public void SetShouldntMatch()
        {
            var sut = new SetTerminal(new SingleTerminal('a'), new RangeTerminal('u', 'w'));

            Assert.IsFalse(sut.Match('b'));
            Assert.IsFalse(sut.Match('z'));

            Assert.IsTrue(sut.NotMatch('b'));
            Assert.IsTrue(sut.NotMatch('z'));
        }
    }
}