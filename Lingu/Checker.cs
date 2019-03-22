using System;
using Lingu.Automata;

namespace Lingu
{
    public class Checker
    {
        public void Check()
        {
            Check1();
        }

        private void Check1()
        {
            // [0]|[1-9][0-9]*
            var zero = (Nfa)'0';
            var nonZero = (Nfa)('1', '9');
            var digit = (Nfa)('0', '9');

            var nfa = zero | (nonZero + digit.Star);

            var dfa = nfa.ToDfa().Minimize();

            var plumber = new DfaPlumber(dfa);

            plumber.Dump(Console.Out);
        }

        private void Check2()
        {
            // .*[A-Z]|.*[0-9]
            var dotPlus = NfaBuilder.Any().Plus();
            var dotStar = NfaBuilder.Any().Star();
            var uLetter = NfaBuilder.From('A', 'Z');
            var digits = NfaBuilder.From('0', '9');

            var x1 = dotStar.Concat(uLetter).Concat(dotStar).Concat(dotPlus).Concat(dotStar);
            var x2 = dotStar.Concat(digits).Concat(dotStar).Concat(dotPlus).Concat(dotStar);
            var nfa = x1.Or(x2);

            new NfaPlumber(nfa).Dump(Console.Out);

            var dfa = nfa.ToDfa().Minimize();

            var plumber = new DfaPlumber(dfa);

            Console.WriteLine("---------------");
            plumber.Dump(Console.Out);
        }

        private void Check3()
        {
            // .*[A-Z].+|.*[0-9].+
            var dotPlus = NfaBuilder.Any().Plus();
            var dotStar = NfaBuilder.Any().Star();
            var uLetter = NfaBuilder.From('A', 'Z');
            var digits = NfaBuilder.From('0', '9');

            var x1 = dotStar.Concat(uLetter).Concat(dotPlus);
            var x2 = dotStar.Concat(digits).Concat(dotPlus);
            var nfa = x1.Or(x2);

            new NfaPlumber(nfa).Dump(Console.Out);

            var dfa = nfa.ToDfa();
            Console.WriteLine("---------------");
            dfa.Dump(Console.Out);
            dfa = dfa.Minimize();
            Console.WriteLine("---------------");
            dfa.Dump(Console.Out);
        }

        private void Check4()
        {
            // .*[0-5]|.*[4-9]
            var dotPlus = NfaBuilder.Any().Plus();
            var dotStar = NfaBuilder.Any().Star();
            var digits1 = NfaBuilder.From('0', '5');
            var digits2 = NfaBuilder.From('4', '9');

            var x1 = dotStar.Concat(digits1).Concat(dotPlus);
            var x2 = dotStar.Concat(digits2).Concat(dotPlus);
            var nfa = x1.Or(x2);

            new NfaPlumber(nfa).Dump(Console.Out);

            var dfa = nfa.ToDfa();
            Console.WriteLine("---------------");
            dfa.Dump(Console.Out);
            dfa = dfa.Minimize();
            Console.WriteLine("---------------");
            dfa.Dump(Console.Out);
        }

        private void Check5()
        {
            var a = NfaBuilder.From('a');
            var b = NfaBuilder.From('b');
            var c = NfaBuilder.From('c');

            var nfa = a.Plus().Concat(b.Plus()).Concat(c.Plus()).Plus().Or(a.Concat(b).Concat(c));

            new NfaPlumber(nfa).Dump(Console.Out);

            var dfa = nfa.ToDfa();
            Console.WriteLine("---------------");
            dfa.Dump(Console.Out);
            dfa = dfa.Minimize();
            Console.WriteLine("---------------");
            dfa.Dump(Console.Out);
        }
    }
}