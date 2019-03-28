using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lingu.Automata.Tests
{
    [TestClass]
    public class TransformTests
    {
        [TestMethod]
        public void TransformShouldCreateDfa()
        {
            var state1 = new NfaState();
            var state2 = new NfaState();
            var state3 = new NfaState();

            state1.Add(Atom.From('1'), state2);
            state2.Add(Atom.From('2'), state3);
            state3.Add(Atom.From('3'), state1);

            var nfa = new Nfa(state1, state3);

            var dfa = nfa.ToDfa().Minimize();

            Assert.IsNotNull(dfa);
            Assert.AreEqual(3, new DfaPlumber(dfa).StateCount);
        }
    }
}
