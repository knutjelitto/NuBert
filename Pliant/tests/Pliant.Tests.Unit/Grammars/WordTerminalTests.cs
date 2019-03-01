using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using Pliant.Terminals;

namespace Pliant.Tests.Unit.Grammars
{
    [TestClass]
    public class WordTerminalTests
    {
        [TestMethod]
        public void WordTerminalShouldMatchWordCharacters()
        {
            Assert.IsTrue(WordTerminal.Instance.IsMatch('a'));
            Assert.IsTrue(WordTerminal.Instance.IsMatch('0'));
            Assert.IsTrue(WordTerminal.Instance.IsMatch('M'));
            Assert.IsTrue(WordTerminal.Instance.IsMatch('_'));
        }
    }
}
