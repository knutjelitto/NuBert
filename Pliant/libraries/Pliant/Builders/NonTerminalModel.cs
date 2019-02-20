using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class NonTerminalModel : SymbolModel
    {
        public NonTerminalModel(NonTerminal value)
        {
            NonTerminal = value;
        }

        public NonTerminalModel(string value)
            : this(new NonTerminal(value))
        {
        }

        public NonTerminalModel(QualifiedName fullyQualifiedName)
            : this(new NonTerminal(fullyQualifiedName.Qualifier, fullyQualifiedName.Name))
        {
        }

        public NonTerminal NonTerminal { get; }

        public override Symbol Symbol => NonTerminal;

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