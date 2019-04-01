using System.Collections.Generic;
using System.Diagnostics;

namespace Lingu.Grammars.Build
{
    public class BodyExpr : List<ChainExpr>
    {
        public BodyExpr(IEnumerable<ChainExpr> chains)
            : base(chains)
        {
        }

        public BodyExpr()
        {
        }

        public static implicit operator BodyExpr(char @char)
        {
            return (ChainExpr) @char;
        }

        public static implicit operator BodyExpr(ChainExpr chain)
        {
            return new BodyExpr { chain };
        }

        public static implicit operator BodyExpr(TerminalExpr terminal)
        {
            return new BodyExpr { terminal };
        }

        public static BodyExpr operator |(BodyExpr body, ChainExpr chain)
        {
            Debug.Assert(chain != null);

            return new BodyExpr(body) { chain };
        }

    }
}