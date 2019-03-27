using Lingu.Grammars;

namespace Lingu.Builders
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

        public static implicit operator TerminalExpr(char @char)
        {
            return new TerminalExpr((DfaProvision)@char);
        }

        public static implicit operator TerminalExpr(string chars)
        {
            return new TerminalExpr((DfaProvision)chars);
        }
    }
}