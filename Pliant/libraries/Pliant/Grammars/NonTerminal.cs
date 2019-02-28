namespace Pliant.Grammars
{
    public sealed class NonTerminal : Symbol
    {
        private NonTerminal(string @namespace, string name)
            : this(new QualifiedName(@namespace, name))
        {
        }

        public NonTerminal(string name)
            : this(string.Empty, name)
        {
        }

        public NonTerminal(QualifiedName qualifiedName)
        {
            QualifiedName = qualifiedName;
            this.hashCode = Value.GetHashCode();
        }

        public QualifiedName QualifiedName { get; }

        public string Value => QualifiedName.FullName;

        public bool Is(string otherName)
        {
            return Value == otherName;
        }

        public bool Is(QualifiedName otherName)
        {
            return Is(otherName.FullName);
        }

        public bool Is(NonTerminal other)
        {
            return Is(other.QualifiedName);
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