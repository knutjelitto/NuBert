using Lingu.Automata;
using Lingu.Grammars;

namespace Lingu.Builders
{
    public class DfaProvision : Provision
    {
        private DfaProvision(string name, Dfa dfa)
            : base(name)
        {
            Dfa = dfa;
        }

        public Dfa Dfa { get; }

        public static DfaProvision From(string name, Nfa nfa)
        {
            return new DfaProvision(name, nfa.ToDfa().Minimize());
        }

        public static implicit operator DfaProvision(char @char)
        {
            return From(@char.ToString(), (Nfa)@char);
        }

        public static implicit operator DfaProvision(string chars)
        {
            return From(chars, (Nfa)chars);
        }
    }
}