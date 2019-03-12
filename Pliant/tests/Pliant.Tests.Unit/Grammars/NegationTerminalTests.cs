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
            Assert.IsFalse(negationTerminal.CanApply('a'));
            Assert.IsFalse(negationTerminal.CanApply(char.MaxValue));
            Assert.IsFalse(negationTerminal.CanApply('0'));
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
