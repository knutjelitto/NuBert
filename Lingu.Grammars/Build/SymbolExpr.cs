namespace Lingu.Grammars.Build
{
    public abstract class SymbolExpr
    {
        protected SymbolExpr(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override bool Equals(object obj)
        {
            return obj is SymbolExpr other && Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static implicit operator ChainExpr(SymbolExpr symbol)
        {
            return new ChainExpr { symbol };
        }

        public static BodyExpr operator |(SymbolExpr symbol1, SymbolExpr symbol2)
        {
            return new BodyExpr { symbol1, symbol2 };
        }


        public static ChainExpr operator +(SymbolExpr terminal, char @char)
        {
            return new ChainExpr { terminal, TerminalExpr.From(DfaProvision.From(@char.ToString(), @char)) };
        }

        public static ChainExpr operator +(SymbolExpr terminal, string chars)
        {
            return new ChainExpr { terminal, TerminalExpr.From(DfaProvision.From(chars, chars)) };
        }
    }
}