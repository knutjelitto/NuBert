using Pliant.Utilities;

namespace Pliant.Grammars
{
    public sealed class QualifiedName : ValueEqualityBase<QualifiedName>
    {
        public QualifiedName(string qualifier, string name)
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

        protected override bool ThisEquals(QualifiedName other)
        {
            return FullName.Equals(other.FullName);
        }

        protected override object ThisHashCode => FullName;

        public override string ToString()
        {
            return FullName;
        }
    }
}