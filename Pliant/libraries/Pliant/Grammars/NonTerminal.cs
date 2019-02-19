namespace Pliant.Grammars
{
    public sealed class NonTerminal : Symbol
    {
        public NonTerminal(string @namespace, string name)
            : this(new QualifiedName(@namespace, name))
        {
        }

        public NonTerminal(string name)
            : this(string.Empty, name)
        {
        }

        public NonTerminal(QualifiedName fullyQualifiedName)
        {
            FullyQualifiedName = fullyQualifiedName;
        }

        public QualifiedName FullyQualifiedName { get; }

        public string Value => FullyQualifiedName.FullName;

        public override bool Equals(object obj)
        {
            return obj is NonTerminal other && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}