using Lingu.Automata;
using Lingu.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Lingu.Automata
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

            state1.Add(new SingleTerminal('1'), state2);
            state2.Add(new SingleTerminal('2'), state3);
            state3.Add(new SingleTerminal('3'), state1);

            var nfa = new Nfa(state1, state3);

            var dfa = nfa.ToDfa();

            Assert.IsNotNull(dfa);
        }
    }
}
