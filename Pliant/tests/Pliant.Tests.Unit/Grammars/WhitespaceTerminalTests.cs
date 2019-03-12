using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using System;
using Pliant.Terminals;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class WhitespaceTerminalTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void WhitespaceTerminalShouldMatchTabCharacter()
        {
            var whitespaceTerminal = WhitespaceTerminal.Instance;
            Assert.IsTrue(whitespaceTerminal.CanApply('\t'));
        }

        [TestMethod]
        public void WhitespaceTerminalShouldMatchNewLineCharacter()
        {
            var whitespaceTerminal = WhitespaceTerminal.Instance;
            Assert.IsTrue(whitespaceTerminal.CanApply('\r'));
        }

        [TestMethod]
        public void WhitespaceTerminalShouldMatchLineFeed()
        {
            var whitespaceTerminal = WhitespaceTerminal.Instance;
            Assert.IsTrue(whitespaceTerminal.CanApply('\n'));
        }

        [TestMethod]
        public void WhitespaceShouldMatchSpaceCharacter()
        {
            var whitespaceTerminal = WhitespaceTerminal.Instance;
            Assert.IsTrue(whitespaceTerminal.CanApply(' '));
        }

        [TestMethod]
        public void WhitespaceTerminalGetIntervalsShouldReturnAllWhitespaceRanges()
        {
            var whitespaceTerminal = WhitespaceTerminal.Instance;
            var intervals = whitespaceTerminal.GetIntervals();
            Assert.AreEqual(9, intervals.Count);
        }
        
    }
}