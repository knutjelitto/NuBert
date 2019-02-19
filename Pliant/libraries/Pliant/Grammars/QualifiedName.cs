using System.Diagnostics;

namespace Pliant.Grammars
{
    public sealed class QualifiedName
    {
        public QualifiedName(string qualifier, string name)
        {
            Qualifier = (qualifier ?? string.Empty).Trim();
            Name = name;
            FullName = qualifier == string.Empty
                           ? $"{name}"
                           : $"{qualifier}.{name}";
        }

        public string FullName { get; }
        public string Name { get; }
        public string Qualifier { get; }

        public static bool operator ==(QualifiedName qualifiedName, string value)
        {
            Debug.Assert(!ReferenceEquals(qualifiedName, null), nameof(qualifiedName) + " != null");
            return qualifiedName.FullName.Equals(value);
        }

        public static bool operator !=(QualifiedName qualifiedName, string value)
        {
            Debug.Assert(!ReferenceEquals(qualifiedName, null), nameof(qualifiedName) + " != null");
            return !qualifiedName.FullName.Equals(value);
        }

        public override bool Equals(object obj)
        {
            return obj is QualifiedName other && FullName.Equals(other.FullName);
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}