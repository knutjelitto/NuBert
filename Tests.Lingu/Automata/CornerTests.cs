using Lingu.Automata;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Lingu.Automata
{
    [TestClass]
    public class CornerTests
    {
        [TestMethod]
        public void Automata1()
        {
            // .*[0-5]|.*[4-9]
            var dotPlus = NfaBuilder.Any().Plus();
            var dotStar = NfaBuilder.Any().Star();
            var digits1 = NfaBuilder.From('0', '5');
            var digits2 = NfaBuilder.From('4', '9');

            var x1 = dotStar.Concat(digits1).Concat(dotPlus);
            var x2 = dotStar.Concat(digits2).Concat(dotPlus);
            var nfa = x1.Or(x2);

            var dfa = nfa.ToDfa();

            dfa = dfa.Minimize();

            Assert.AreEqual(3, new DfaPlumber(dfa).StateCount);
        }

        [TestMethod]
        public void Automata2()
        {
            // .*[A-Z].+|.*[0-9].+
            var dotPlus = NfaBuilder.Any().Plus();
            var dotStar = NfaBuilder.Any().Star();
            var letter = NfaBuilder.From('A', 'Z');
            var digits = NfaBuilder.From('0', '9');

            var x1 = dotStar.Concat(letter).Concat(dotPlus);
            var x2 = dotStar.Concat(digits).Concat(dotPlus);
            var nfa = x1.Or(x2);

            var dfa = nfa.ToDfa();

            dfa = dfa.Minimize();

            Assert.AreEqual(3, new DfaPlumber(dfa).StateCount);
        }

        [TestMethod]
        public void Automata3()
        {
            // [0]|[1-9][0-9]*
            var zero = NfaBuilder.From('0');
            var nonZero = NfaBuilder.From('1', '9');
            var all = NfaBuilder.From('0', '9');

            var nfa = zero.Or(nonZero.Concat(all.Star()));

            var dfa = nfa.ToDfa();

            dfa = dfa.Minimize();

            Assert.AreEqual(3, new DfaPlumber(dfa).StateCount);
        }


        [TestMethod]
        public void Automata4()
        {
            // (a+b+c+)+|abc
            var a = NfaBuilder.From('a');
            var b = NfaBuilder.From('b');
            var c = NfaBuilder.From('c');

            var nfa = (a.Plus().Concat(b.Plus()).Concat(c.Plus()).Plus()).Or(a.Concat(b).Concat(c));

            var dfa = nfa.ToDfa();

            dfa = dfa.Minimize();

            Assert.AreEqual(4, new DfaPlumber(dfa).StateCount);
        }
    }
}