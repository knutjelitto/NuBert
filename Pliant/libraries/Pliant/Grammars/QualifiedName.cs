using Pliant.Utilities;

namespace Pliant.Grammars
{
    public sealed class QualifiedName : ValueEqualityBase<QualifiedName>
    {
        public QualifiedName(string qualifier, string name)
            : base((qualifier, name))
        {
            Qualifier = (qualifier ?? string.Empty).Trim();
            Name = name;
            FullName = Qualifier == string.Empty
                ? $"{Name}"
                : $"{Qualifier}.{Name}";
        }

        public QualifiedName(string name)
            : this(string.Empty, name)
        {
        }

        public string FullName { get; }
        public string Name { get; }
        public string Qualifier { get; }

        public override bool ThisEquals(QualifiedName other)
        {
            return FullName.Equals(other.FullName);
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}