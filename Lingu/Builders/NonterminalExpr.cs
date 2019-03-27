namespace Lingu.Builders
{
    public class NonterminalExpr : SymbolExpr
    {
        private NonterminalExpr(string name)
            : base(name)
        {
        }

        public BodyExpr Body { get; set; }


        public static implicit operator NonterminalExpr(string name)
        {
            return new NonterminalExpr(name);
        }

        public static implicit operator ChainExpr(NonterminalExpr rule)
        {
            return new ChainExpr { rule };
        }

        public static implicit operator BodyExpr(NonterminalExpr rule)
        {
            return new ChainExpr { rule };
        }

        public static ChainExpr operator +(char @char, NonterminalExpr rule)
        {
            return new ChainExpr { (TerminalExpr)@char, rule };
        }

        public static ChainExpr operator +(NonterminalExpr rule, char @char)
        {
            return new ChainExpr { rule, (TerminalExpr)@char };
        }

        public static ChainExpr operator +(NonterminalExpr nonterminal, TerminalExpr terminal)
        {
            return new ChainExpr { nonterminal, terminal };
        }
    }
}