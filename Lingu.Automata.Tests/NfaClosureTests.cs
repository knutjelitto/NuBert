using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lingu.Automata.Tests
{
    [TestClass]
    public class NfaClosureTests
    {
        [TestMethod]
        public void NfaClosureShouldNewWithEmptyStates()
        {
            var state = new NfaState();

            var sut = new NfaClosure(Enumerable.Empty<NfaState>(), state);

            Assert.IsNotNull(sut);
            Assert.IsNotNull(sut.Set);
            Assert.AreEqual(0, sut.Set.Count);
            Assert.IsNotNull(sut.State);
            Assert.AreEqual(false, sut.State.IsFinal);
        }

        [TestMethod]
        public void NfaClosureShouldNewWithOneState()
        {
            var state = new NfaState();

            var sut = new NfaClosure(Enumerable.Repeat(state, 1), state);

            Assert.IsNotNull(sut);
            Assert.IsNotNull(sut.Set);
            Assert.AreEqual(1, sut.Set.Count);
            Assert.IsNotNull(sut.State);
            Assert.AreEqual(true, sut.State.IsFinal);
        }

        [TestMethod]
        public void NfaClosureShouldNewWithTwoStates()
        {
            var state1 = new NfaState();
            var state2 = new NfaState();

            var sut = new NfaClosure(new [] { state1, state2 }, state1);

            Assert.IsNotNull(sut);
            Assert.IsNotNull(sut.Set);
            Assert.AreEqual(2, sut.Set.Count);
            Assert.IsNotNull(sut.State);
            Assert.AreEqual(true, sut.State.IsFinal);
        }


        [TestMethod]
        public void NfaClosuresFromSameSetShouldBeEqual()
        {
            var state1 = new NfaState();
            var state2 = new NfaState();

            var sut1 = new NfaClosure(new[] { state1, state2 }, state1);
            var sut2 = new NfaClosure(new[] { state2, state1 }, state1);

            Assert.IsTrue(sut1.Equals(sut2));
            Assert.AreEqual(sut1.GetHashCode(), sut1.GetHashCode());
        }
    }
}
