namespace Pliant.Grammars
{
    public sealed class NonTerminal : Symbol
    {
        public NonTerminal(QualifiedName qualifiedName)
        {
            QualifiedName = qualifiedName;
            this.hashCode = Value.GetHashCode();
        }

        public static NonTerminal From(string name)
        {
            return new NonTerminal(new QualifiedName(string.Empty, name));
        }

        public QualifiedName QualifiedName { get; }

        public string Value => QualifiedName.FullName;

        public bool Is(string otherFullName)
        {
            return Value == otherFullName;
        }

        public bool Is(QualifiedName otherName)
        {
            return QualifiedName.Equals(otherName);
        }

        public bool Is(NonTerminal other)
        {
            return Is(other.QualifiedName);
        }

        public bool Is(Production production)
        {
            return Is(production.LeftHandSide);
        }

        public override bool Equals(object obj)
        {
            return obj is NonTerminal other && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override string ToString()
        {
            return Value;
        }

        private readonly int hashCode;
    }
}