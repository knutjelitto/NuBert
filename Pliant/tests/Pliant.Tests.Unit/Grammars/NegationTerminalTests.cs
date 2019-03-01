using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Terminals;

namespace Pliant.Tests.Unit.Grammars
{
    [TestClass]
    public class NegationTerminalTests
    {
        [TestMethod]
        public void NegationTerminalShouldNegateAnyTerminal()
        {
            var negationTerminal = new NegationTerminal(AnyTerminal.Instance);
            Assert.IsFalse(negationTerminal.IsMatch('a'));
            Assert.IsFalse(negationTerminal.IsMatch(char.MaxValue));
            Assert.IsFalse(negationTerminal.IsMatch('0'));
        }

        [TestMethod]
        public void NegationTerminalShouldReturnInverseIntervals()
        {
            var negationTerminal = new NegationTerminal(AnyTerminal.Instance);
            var intervals = negationTerminal.GetIntervals();
            Assert.AreEqual(0, intervals.Count);            
        }
    }
}
