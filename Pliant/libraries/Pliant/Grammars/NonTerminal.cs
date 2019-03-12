namespace Pliant.Grammars
{
    public sealed class NonTerminal : Symbol
    {
        private NonTerminal(QualifiedName qualifiedName)
        {
            QualifiedName = qualifiedName;
            this.hashCode = QualifiedName.GetHashCode();
        }

        public static NonTerminal From(QualifiedName qualifiedName)
        {
            return new NonTerminal(qualifiedName);
        }

        public static NonTerminal From(string name)
        {
            return From(new QualifiedName(string.Empty, name));
        }

        public QualifiedName QualifiedName { get; }

        public bool Is(QualifiedName otherName)
        {
            return QualifiedName.Equals(otherName);
        }

        public bool Is(NonTerminal other)
        {
            return QualifiedName.Equals(other.QualifiedName);
        }

        public override bool Equals(object obj)
        {
            return obj is NonTerminal other && QualifiedName.Equals(other.QualifiedName);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override string ToString()
        {
            return QualifiedName.ToString();
        }

        private readonly int hashCode;
    }
}