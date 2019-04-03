using Lingu.Automata;
using Lingu.Grammars;

namespace Lingu.Grammars
{
    public class DfaProvision : Provision
    {
        private DfaProvision(string name, Nfa nfa)
            : base(name)
        {
            Dfa = nfa.ToDfa().Minimize();
            Terminal = new Terminal(this);
        }

        public Dfa Dfa { get; }
        public override Terminal Terminal { get; }

        public static DfaProvision From(string name, Nfa nfa)
        {
            return new DfaProvision(name, nfa);
        }

        public override Lexer MakeLexer(int offset)
        {
            return new DfaLexer(Terminal, offset);
        }
    }
}