using Pliant.Grammars;

namespace Pliant.Builders.Expressions
{
    public sealed class NamespaceExpression
    {
        public NamespaceExpression(string @namespace)
        {
            Namespace = @namespace;
        }

        public string Namespace { get; }

        public static QualifiedName operator +(NamespaceExpression @namespace, string name)
        {
            return new QualifiedName(@namespace.Namespace, name);
        }

        public static implicit operator NamespaceExpression(string @namespace)
        {
            return new NamespaceExpression(@namespace);
        }
    }
}