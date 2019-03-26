using Lingu.Grammars;

namespace Lingu.Builders
{
    public class TerminalExpr : SymbolExpr
    {
        public TerminalExpr(Provision provision)
            : base(provision.Name)
        {
            Provision = provision;
        }

        public Provision Provision { get; }

        public static implicit operator TerminalExpr(char @char)
        {
            return new TerminalExpr((DfaProvision)@char);
        }

        public static implicit operator TerminalExpr(string chars)
        {
            return new TerminalExpr((DfaProvision)chars);
        }

        public static implicit operator TerminalExpr(DfaProvision provision)
        {
            return new TerminalExpr(provision);
        }
    }
}