namespace Lingu.Grammars.Build
{
    public class RuleExpr : SymbolExpr
    {
        private RuleExpr(string name)
            : base(name)
        {
        }

        public BodyExpr Body { get; set; }


        public static implicit operator RuleExpr(string name)
        {
            return new RuleExpr(name);
        }

        public static implicit operator ChainExpr(RuleExpr rule)
        {
            return new ChainExpr { rule };
        }

        public static implicit operator BodyExpr(RuleExpr rule)
        {
            return new ChainExpr { rule };
        }

        public static ChainExpr operator +(char @char, RuleExpr rule)
        {
            return new ChainExpr { (TerminalExpr)@char, rule };
        }

        public static ChainExpr operator +(RuleExpr rule, char @char)
        {
            return new ChainExpr { rule, (TerminalExpr)@char };
        }

        public static ChainExpr operator +(RuleExpr rule1, RuleExpr rule2)
        {
            return new ChainExpr { rule1, rule2 };
        }

        public static ChainExpr operator +(RuleExpr nonterminal, TerminalExpr terminal)
        {
            return new ChainExpr { nonterminal, terminal };
        }
    }
}