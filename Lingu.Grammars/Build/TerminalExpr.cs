using Lingu.Automata;
using Lingu.Grammars;

namespace Lingu.Grammars.Build
{
    public class TerminalExpr : SymbolExpr
    {
        private TerminalExpr(Provision provision)
            : base(provision.Name)
        {
            Provision = provision;
        }

        public Provision Provision { get; }

        public static TerminalExpr From(Provision provision)
        {
            return new TerminalExpr(provision);
        }

        public static ChainExpr operator +(TerminalExpr terminal1, TerminalExpr terminal2)
        {
            return new ChainExpr {terminal1, terminal2};
        }

        public static implicit operator TerminalExpr(char @char)
        {
            return new TerminalExpr(DfaProvision.From(@char.ToString(), @char));
        }

        public static implicit operator TerminalExpr(string chars)
        {
            return new TerminalExpr(DfaProvision.From(chars, chars));
        }
    }
}