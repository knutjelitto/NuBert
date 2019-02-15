using Pliant.Grammars;

namespace Pliant.Builders
{
    public class NonTerminalModel
        : SymbolModel
    {
        public NonTerminalModel(INonTerminal value)
        {
            NonTerminal = value;
        }

        public NonTerminalModel(string value)
            : this(new NonTerminal(value))
        {
        }

        public NonTerminalModel(FullyQualifiedName fullyQualifiedName)
            : this(new NonTerminal(fullyQualifiedName.Namespace, fullyQualifiedName.Name))
        {
        }

        public override SymbolModelType ModelType => SymbolModelType.NonTerminal;

        public override ISymbol Symbol => NonTerminal;

        public INonTerminal NonTerminal { get; }

        public override int GetHashCode()
        {
            return NonTerminal.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is NonTerminalModel nonTerminalModel &&
                   NonTerminal.Equals(nonTerminalModel.NonTerminal);
        }

        public override string ToString()
        {
            return NonTerminal.ToString();
        }
    }
}