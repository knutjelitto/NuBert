using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using Pliant.Terminals;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class AnyTerminalTests
    {
        [TestMethod]
        public void AnyTerminalIsMatchShouldReturnTrueWhenAnyCharacterSpecified()
        {
            for (var c = char.MinValue; c < char.MaxValue; c++)
            {
                Assert.IsTrue(AnyTerminal.Instance.CanApply(c));
            }
        }
    }
}