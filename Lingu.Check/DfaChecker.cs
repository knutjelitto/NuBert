using System;
using System.Collections.Generic;
using System.Text;
using Lingu.Automata;

namespace Lingu.Check
{
    public class DfaChecker
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
            var dot = Nfa.Any;

            var nfa = (dot.Star + ('A', 'Z')) | (dot.Star + ('0', '9'));

            nfa.Dump();

            var dfa = nfa.ToDfa().Minimize();

            var plumber = new DfaPlumber(dfa);

            Console.WriteLine("---------------");
            plumber.Dump(Console.Out);
        }

        private void Check3()
        {
            // .*[A-Z].+|.*[0-9].+
            var dot = Nfa.Any;

            var nfa = (dot.Star + ('A', 'Z') + dot.Plus) | (dot.Star + ('0', '9') + dot.Plus);

            nfa.Dump();

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
            var dot = Nfa.Any;

            var nfa = (dot.Star + ('0', '5') + dot.Plus) | (dot.Star + ('4', '9') + dot.Plus);

            nfa.Dump();

            var dfa = nfa.ToDfa();
            Console.WriteLine("---------------");
            dfa.Dump(Console.Out);
            dfa = dfa.Minimize();
            Console.WriteLine("---------------");
            dfa.Dump(Console.Out);
        }

        private void Check5()
        {
            var a = (Nfa)'a';
            var b = (Nfa)'b';
            var c = (Nfa)'c';

            var nfa = (a.Plus + b.Plus + c.Plus).Plus | (a + b + c);

            nfa.Dump();

            var dfa = nfa.ToDfa();
            Console.WriteLine("---------------");
            dfa.Dump(Console.Out);
            dfa = dfa.Minimize();
            Console.WriteLine("---------------");
            dfa.Dump(Console.Out);
        }

        private void Check6()
        {
            var nfa = (Nfa)"abc";

            nfa.Dump();

            var dfa = nfa.ToDfa();
            Console.WriteLine("---------------");
            dfa.Dump(Console.Out);
            dfa = dfa.Minimize();
            Console.WriteLine("---------------");
            dfa.Dump(Console.Out);
        }
    }
}
