using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lingu.Automata.Tests
{
    [TestClass]
    public class CornerTests
    {
        [TestMethod]
        public void Automata1()
        {
            // .*[0-5]|.*[4-9]
            var dot = Nfa.Any;

            var nfa = (dot.Star + ('0', '5') + dot.Plus) | (dot.Star + ('4', '9') + dot.Plus);

            var dfa = nfa.ToDfa();

            dfa = dfa.Minimize();

            Assert.AreEqual(3, new DfaPlumber(dfa).StateCount);
        }

        [TestMethod]
        public void Automata2()
        {
            // .*[A-Z].+|.*[0-9].+
            var dotPlus = Nfa.Any.Plus;
            var dotStar = Nfa.Any.Star;
            var letter = (Nfa)('A', 'Z');
            var digits = (Nfa)('0', '9');

            var nfa = (dotStar + letter + dotPlus) | (dotStar + digits + dotPlus);

            var dfa = nfa.ToDfa();

            dfa = dfa.Minimize();

            Assert.AreEqual(3, new DfaPlumber(dfa).StateCount);
        }

        [TestMethod]
        public void Automata3()
        {
            // [0]|[1-9][0-9]*
            var nfa = '0' | (('1', '9') + ((Nfa)('0', '9')).Star);


            var dfa = nfa.ToDfa();

            dfa = dfa.Minimize();

            Assert.AreEqual(3, new DfaPlumber(dfa).StateCount);
        }


        [TestMethod]
        public void Automata4()
        {
            // (a+b+c+)+|abc
            var a = (Nfa)'a';
            var b = (Nfa)'b';
            var c = (Nfa)'c';

            var nfa = (a.Plus + b.Plus + c.Plus).Plus |  (a + b + c);

            var dfa = nfa.ToDfa();

            dfa = dfa.Minimize();

            Assert.AreEqual(4, new DfaPlumber(dfa).StateCount);
        }
    }
}