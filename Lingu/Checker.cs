using System;
using System.Diagnostics;
using System.Globalization;
using Lingu.Automata;
using Lingu.Tools;

namespace Lingu
{
    public class Checker
    {
        public void Check()
        {
            // [0]|[1-9][0-9]*
            var zero = NfaBuilder.From('0');
            var nonZero = NfaBuilder.From('1', '9');
            var all = NfaBuilder.From('0', '9');

            var nfa = zero.Or(nonZero.Concat(all.Star()));
            var dfa = nfa.ToDfa();

            dfa.Dump(Console.Out);
        }
    }
}
