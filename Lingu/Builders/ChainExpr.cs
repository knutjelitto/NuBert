using System.Collections.Generic;
using System.Linq;

namespace Lingu.Builders
{
    public class ChainExpr : List<SymbolExpr>
    {
        public static readonly ChainExpr Epsilon = new ChainExpr();

        private ChainExpr(IEnumerable<SymbolExpr> symbols)
            : base(symbols)
        {
        }

        public ChainExpr()
        {
        }

        public static implicit operator ChainExpr(string chars)
        {
            return new ChainExpr { (TerminalExpr)chars };
        }

        public static implicit operator ChainExpr(char @char)
        {
            return new ChainExpr { (TerminalExpr)@char };
        }

        public static ChainExpr operator +(ChainExpr chain, SymbolExpr symbol)
        {
            return new ChainExpr(chain) { symbol };
        }

        public static ChainExpr operator +(ChainExpr chain, NonterminalExpr rule)
        {
            return new ChainExpr(chain) { rule };
        }

        public static ChainExpr operator +(ChainExpr chain, char @char)
        {
            return new ChainExpr(chain) { (TerminalExpr)@char };
        }

        public static ChainExpr operator +(char @char, ChainExpr chain)
        {
            return new ChainExpr(Enumerable.Repeat(TerminalExpr.From((DfaProvision)@char), 1).Concat(chain));
        }

        public static BodyExpr operator |(NonterminalExpr rule, ChainExpr chain)
        {
            return new BodyExpr { rule, chain };
        }

        public static BodyExpr operator |(TerminalExpr terminal, ChainExpr chain)
        {
            return new BodyExpr { terminal, chain };
        }

        public static BodyExpr operator |(ChainExpr chain, NonterminalExpr rule)
        {
            return new BodyExpr { chain, rule };
        }

    }
}
