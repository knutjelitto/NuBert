using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class NonTerminalModel : SymbolModel
    {
        public NonTerminalModel(NonTerminal value)
            : base(value)
        {
        }

        public NonTerminal NonTerminal => (NonTerminal) Symbol;

        public override bool Equals(object obj)
        {
            return obj is NonTerminalModel other && NonTerminal.Equals(other.NonTerminal);
        }

        public override int GetHashCode()
        {
            return NonTerminal.GetHashCode();
        }

        public override string ToString()
        {
            return NonTerminal.ToString();
        }
    }
}